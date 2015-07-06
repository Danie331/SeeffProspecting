using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ProspectingProject
{
    public sealed partial class ProspectingCore
    {
        public static CommunicationBatchStatus SubmitEmailBatch(EmailBatch batch)
        {
            try
            {
                byte[] data = Convert.FromBase64String(batch.EmailBodyHTMLRaw);
                batch.EmailBodyHTMLRaw = Encoding.UTF8.GetString(data);

                data = Convert.FromBase64String(batch.EmailSubjectRaw);
                batch.EmailSubjectRaw = Encoding.UTF8.GetString(data);

                data = Convert.FromBase64String(batch.NameOfBatch);
                batch.NameOfBatch = Encoding.UTF8.GetString(data);

                var emailRecipients = GetRecipientsFromBatch(batch);
                EnqueueBatch(emailRecipients, batch);
                DebitUserBalanceForEmailBatch(emailRecipients.Count);

                return new CommunicationBatchStatus { SuccessfullySubmitted = true };
            }
            catch (Exception ex)
            {
                using (var prospectingDb = new ProspectingDataContext())
                {
                    var errorRec = new exception_log
                    {
                        friendly_error_msg = ex.Message,
                        exception_string = ex.ToString(),
                        user = RequestHandler.GetUserSessionObject().UserGuid,
                        date_time = DateTime.Now
                    };
                    prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                    prospectingDb.SubmitChanges();
                }
                return new CommunicationBatchStatus
                {
                    SuccessfullySubmitted = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private static void DebitUserBalanceForEmailBatch(int count)
        {
            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                decimal cost = (decimal)count * 0.01M;

                var prospectingUser = RequestHandler.GetUserSessionObject();
                prospectingAuthService.DebitUserBalance(cost, prospectingUser.UserGuid);
            }
        }

        private static List<EmailRecipient> GetRecipientsFromBatch(EmailBatch batch)
        {
            var contacts = GetEmailTargetContactPersons(batch);
            var recipients = CreateMailRecipients(contacts, batch);
            return recipients;
        }

        private static List<SmsRecipient> GetRecipientsFromBatch(SmsBatch batch)
        {
            var contacts = GetSmsTargetContactPersons(batch);
            var recipients = CreateSmsRecipients(contacts, batch);
            return recipients;
        }

        private static List<SmsRecipient> CreateSmsRecipients(List<ProspectingContactPerson> contacts, SmsBatch batch)
        {
            List<SmsRecipient> recipients = new List<SmsRecipient>();
            foreach (var contact in contacts)
            {
                string messageBody = CreateSmsMessage(batch.SmsBodyRaw, contact);

                var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                SmsRecipient recipient = new SmsRecipient
                {
                    ContactPersonId = contact.ContactPersonId.Value,
                    SMSMessage = messageBody,
                    QualifiedCellNumber = contact.TargetContactCellphoneNumber,
                    TargetLightstonePropertyId = contact.TargetLightstonePropertyIdForComms.Value,
                    Fullname = titlecaser.ToTitleCase(contact.Firstname.ToLower()) + " " + titlecaser.ToTitleCase(contact.Surname.ToLower())
                };
                recipients.Add(recipient);
            }
            return recipients;
        }

        private static string CreateSmsMessage(string rawMessageBody, ProspectingContactPerson contact)
        {
            var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;

            string personTitle = "";
            if (contact.Title.HasValue)
            {
                personTitle = ProspectingLookupData.ContactPersonTitle.First(cpt => cpt.Key == contact.Title.Value).Value;
            }
            string name = titlecaser.ToTitleCase(contact.Firstname.ToLower());
            string surname = titlecaser.ToTitleCase(contact.Surname.ToLower());
            string address = ProspectingCore.GetFormattedAddress(contact.TargetLightstonePropertyIdForComms.Value);

            rawMessageBody = rawMessageBody.Replace("*title*", personTitle)
                             .Replace("*name*", name)
                             .Replace("*surname*", surname)
                             .Replace("*address*", address);

            return rawMessageBody;
        }

        private static List<ProspectingContactPerson> GetSmsTargetContactPersons(SmsBatch batch)
        {
            List<ProspectingContactPerson> contacts = new List<ProspectingContactPerson>();
            using (var prospecting = new ProspectingDataContext())
            {
                if (batch.Recipients.Count > 0)
                {
                    foreach (var contact in batch.Recipients)
                    {
                        var contactRecord = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contact.ContactPersonId);
                        var contactDetail = contactRecord.prospecting_contact_details.First(cd => cd.contact_detail == contact.TargetContactCellphoneNumber);
                        contact.TargetContactCellphoneNumber = contactDetail.prospecting_area_dialing_code.dialing_code_id + contact.TargetContactCellphoneNumber.Remove(0, 1);
                    }
                    return batch.Recipients; // make sure the front-end populates these with all required values
                }

                if (batch.TargetAllMySuburbs)
                {
                    var targetContacts = GetSmsTargetContactPersons(batch.UserSuburbIds.ToArray());
                    contacts.AddRange(targetContacts);
                }

                if (batch.TargetCurrentSuburb)
                {
                    var targetContacts = GetSmsTargetContactPersons(new[] { batch.CurrentSuburbId.Value });
                    contacts.AddRange(targetContacts);
                }

                contacts = contacts.Distinct(new ContactPersonSmsComparer()).ToList();
                contacts = FilterContactsForTemplateType(contacts, batch.TemplateActivityTypeId);

                return contacts;
            }
        }

        private static List<ProspectingContactPerson> GetSmsTargetContactPersons(int[] suburbIDs)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var propertyOwnerContacts = from pp in prospecting.prospecting_properties
                                            join pr in prospecting.prospecting_person_property_relationships on pp.prospecting_property_id equals pr.prospecting_property_id
                                            join cp in prospecting.prospecting_contact_persons on pr.contact_person_id equals cp.contact_person_id
                                            join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
                                            where cd.contact_detail_type == ProspectingLookupData.CellPhoneTypeId && cd.is_primary_contact && !cd.deleted
                                            && suburbIDs.Contains(pp.seeff_area_id.Value) && pp.prospected == true && pp.latest_reg_date == null && !cp.optout_sms && !cp.is_popi_restricted
                                            select new ProspectingContactPerson
                                            {
                                                ContactPersonId = cp.contact_person_id,
                                                Title = cp.person_title,
                                                Firstname = cp.firstname,
                                                Surname = cp.surname,
                                                IdNumber = cp.id_number,
                                                TargetLightstonePropertyIdForComms = pp.lightstone_property_id,
                                                TargetContactCellphoneNumber = cd.prospecting_area_dialing_code.dialing_code_id + cd.contact_detail.Remove(0,1)
                                            };

                var propertyCompanyContacts = from pp in prospecting.prospecting_properties
                                              join cpr in prospecting.prospecting_company_property_relationships on pp.prospecting_property_id equals cpr.prospecting_property_id
                                              join pcr in prospecting.prospecting_person_company_relationships on cpr.contact_company_id equals pcr.contact_company_id
                                              join cp in prospecting.prospecting_contact_persons on pcr.contact_person_id equals cp.contact_person_id
                                              join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
                                              where cd.contact_detail_type == ProspectingLookupData.CellPhoneTypeId && cd.is_primary_contact && !cd.deleted
                                              && suburbIDs.Contains(pp.seeff_area_id.Value) && pp.prospected == true && pp.latest_reg_date == null && !cp.optout_sms && !cp.is_popi_restricted
                                              select new ProspectingContactPerson
                                              {
                                                  ContactPersonId = cp.contact_person_id,
                                                  Title = cp.person_title,
                                                  Firstname = cp.firstname,
                                                  Surname = cp.surname,
                                                  IdNumber = cp.id_number,
                                                  TargetLightstonePropertyIdForComms = pp.lightstone_property_id,
                                                  TargetContactCellphoneNumber = cd.prospecting_area_dialing_code.dialing_code_id + cd.contact_detail.Remove(0, 1)
                                              };

                return propertyOwnerContacts.Union(propertyCompanyContacts).ToList();
            }
        }

        /// <summary>
        /// Assumption here that all contacts have at least one email address
        /// </summary>
        private static List<EmailRecipient> CreateMailRecipients(List<ProspectingContactPerson> contacts, EmailBatch batch)
        {
            List<EmailRecipient> recipients = new List<EmailRecipient>();
            // Note: The ProspectingContactPerson's in the batch come from the front-end and ARE NOT populated with data.
            // The contact record from the front-end holds only the information needed to identify the target contact person and lightstone property.
            foreach (var contact in contacts)
            {
                string targetEmailAddress = contact.TargetContactEmailAddress;
                string emailBody = CreateEmailBody(batch.EmailBodyHTMLRaw, contact); // TAKE NOTE: contactRecord != contact
                string emailSubject = CreateEmailSubject(batch.EmailSubjectRaw);

                var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                EmailRecipient recipient = new EmailRecipient
                {
                    ContactPersonId = contact.ContactPersonId.Value,
                    EmailBody = emailBody,
                    EmailSubject = emailSubject,
                    ToEmailAddress = targetEmailAddress,
                    TargetLightstonePropertyId = contact.TargetLightstonePropertyIdForComms.Value, // Must be present in this context.
                    Fullname = titlecaser.ToTitleCase(contact.Firstname.ToLower()) + " " + titlecaser.ToTitleCase(contact.Surname.ToLower())
                };
                recipients.Add(recipient);
            }

            return recipients;
        }

        private static void EnqueueBatch(List<EmailRecipient> emailRecipients, EmailBatch batch)
        {
            var prospectingUser = RequestHandler.GetUserSessionObject();
            using (var prospecting = new ProspectingDataContext())
            {
                Guid batchId = Guid.NewGuid();
                var createdDatetime = DateTime.Now;
                var status = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "PENDING_SUBMIT_TO_API").Key;
                if (!batch.TemplateActivityTypeId.HasValue)
                {
                    batch.TemplateActivityTypeId = ProspectingLookupData.ActivityTypes.First(act => act.Value == "General").Key;
                }
                foreach (var recipient in emailRecipients)
                {
                    var contactRecord = prospecting.prospecting_contact_persons.First(c => c.contact_person_id == recipient.ContactPersonId);
                    email_communications_log record = new email_communications_log
                    {
                        batch_id = batchId,
                        batch_friendly_name = batch.NameOfBatch,
                        batch_activity_type_id = batch.TemplateActivityTypeId.Value, 
                        activity_log_id = null, // Only set when successfully sent.
                        followup_activity_id = null,
                        created_by_user_guid = prospectingUser.UserGuid,
                        created_by_user_name = prospectingUser.Fullname,
                        created_by_user_email_address = prospectingUser.EmailAddress,
                        created_datetime = createdDatetime,
                        target_contact_person_id = recipient.ContactPersonId,
                        target_email_address = recipient.ToEmailAddress,
                        target_lightstone_property_id = recipient.TargetLightstonePropertyId,
                        status = status,
                        email_body_or_link_id = recipient.EmailBody,
                        email_subject_or_link_id = recipient.EmailSubject, 
                    };
                    if (batch.Attachments.Count > 0)
                    {
                        record.attachment1_name = batch.Attachments[0].name;
                        record.attachment1_type = batch.Attachments[0].type;
                        record.attachment1_content = batch.Attachments[0].base64;
                    }
                    prospecting.email_communications_logs.InsertOnSubmit(record);
                }
                prospecting.SubmitChanges();
            }
        }

        private static void EnqueueBatch(List<SmsRecipient> recipients, SmsBatch batch)
        {
            var prospectingUser = RequestHandler.GetUserSessionObject();
            using (var prospecting = new ProspectingDataContext())
            {
                Guid batchId = Guid.NewGuid();
                var createdDatetime = DateTime.Now;
                var status = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "PENDING_SUBMIT_TO_API").Key;
                if (!batch.TemplateActivityTypeId.HasValue)
                {
                    batch.TemplateActivityTypeId = ProspectingLookupData.ActivityTypes.First(act => act.Value == "General").Key;
                }
                foreach (var recipient in recipients)
                {
                    var contactRecord = prospecting.prospecting_contact_persons.First(c => c.contact_person_id == recipient.ContactPersonId);
                    sms_communications_log record = new sms_communications_log
                    {
                        batch_id = batchId,
                        batch_friendly_name = batch.NameOfBatch,
                        batch_activity_type_id = batch.TemplateActivityTypeId.Value,
                        activity_log_id = null,
                        followup_activity_id = null,
                        created_by_user_guid = prospectingUser.UserGuid,
                        created_datetime = createdDatetime,
                        target_contact_person_id = recipient.ContactPersonId,
                        target_cellphone_no = recipient.QualifiedCellNumber,
                        target_lightstone_property_id = recipient.TargetLightstonePropertyId,
                        status = status,
                        msg_body_or_link_id = recipient.SMSMessage
                    };
                    prospecting.sms_communications_logs.InsertOnSubmit(record);
                }
                prospecting.SubmitChanges();
            }
        }
       
        private static string CreateEmailSubject(string subjectText)
        {
            return subjectText;
        }

        private static string CreateEmailBody(string rawBody, ProspectingContactPerson contact)
        {
            var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;

            string personTitle = "";
            if (contact.Title.HasValue)
            {
                personTitle = ProspectingLookupData.ContactPersonTitle.First(cpt => cpt.Key == contact.Title.Value).Value;
            }
            string name = titlecaser.ToTitleCase(contact.Firstname.ToLower());
            string surname = titlecaser.ToTitleCase(contact.Surname.ToLower());
            string address = ProspectingCore.GetFormattedAddress(contact.TargetLightstonePropertyIdForComms.Value);

            rawBody = rawBody.Replace("*title*", personTitle)
                             .Replace("*name*", name)
                             .Replace("*surname*", surname)
                             .Replace("*address*", address);

            var link = "http://154.70.214.213/ProspectingTaskScheduler/UnsubscribeEmail.html?email=" + contact.TargetContactEmailAddress + "&contactid=" + contact.ContactPersonId;
            string optoutLink = "<p /><a href='" + link + "' target='_blank'>Unsubscribe</a>";

            return rawBody + optoutLink;
        }

        private static List<ProspectingContactPerson> FilterContactsForTemplateType(List<ProspectingContactPerson> contacts, int? templateActivityTypeId)
        {
            if (templateActivityTypeId == null || templateActivityTypeId == ProspectingLookupData.ActivityTypes.First(act => act.Value == "General").Key)
            {
                return contacts;
            }

            int birthdayActivityType = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "Birthday").Key;
            int fiveYearAnniversaryActvityType = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "5 Year Anniversary").Key;
            int sevenYearAnniversaryType = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "7 Year Anniversary").Key;

            if (templateActivityTypeId == birthdayActivityType)
            {
                return FilterContactsByTodaysBirthday(contacts);
            }

            if (templateActivityTypeId == fiveYearAnniversaryActvityType)
            {
                return FilterContactsByAnniversaryDate(contacts, 5);
            }

            if (templateActivityTypeId == sevenYearAnniversaryType)
            {
                return FilterContactsByAnniversaryDate(contacts, 7);
            }

            return contacts;
        }

        private static List<ProspectingContactPerson> FilterContactsByTodaysBirthday(List<ProspectingContactPerson> contacts)
        {
            List<ProspectingContactPerson> filteredContacts = new List<ProspectingContactPerson>();
            foreach (var contact in contacts)
            {
                string idNumber = contact.IdNumber;
                if (idNumber.Length == 13 && idNumber.Take(6).All(s => char.IsNumber(s)))
                {
                    // Try convert the first 6 digits to a date
                    DateTime result;
                    if (DateTime.TryParseExact(idNumber.Substring(0, 6), "yyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result)) 
                    {
                        if (result.Month == DateTime.Today.Month && result.Day == DateTime.Today.Day)
                        {
                            filteredContacts.Add(contact);
                        }
                    }
                }
            }
            return filteredContacts;
        }

        private static List<ProspectingContactPerson> FilterContactsByAnniversaryDate(List<ProspectingContactPerson> contacts, int years)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                List<ProspectingContactPerson> filteredContacts = new List<ProspectingContactPerson>();
                foreach (var contact in contacts)
                {
                    if (contact.TargetLightstonePropertyIdForComms != null)
                    {
                        int lightstonePropertyId = contact.TargetLightstonePropertyIdForComms.Value;
                        var propertingProperty = prospecting.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropertyId);
                        if (!string.IsNullOrEmpty(propertingProperty.lightstone_reg_date) && propertingProperty.lightstone_reg_date.Length == 8)
                        {
                            DateTime regDate;
                            if (DateTime.TryParseExact(propertingProperty.lightstone_reg_date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out regDate))
                            {
                                var testDate = regDate.AddYears(years).Date;
                                var todaysDate = DateTime.Today;
                                if (testDate == todaysDate)
                                {
                                    filteredContacts.Add(contact);
                                }
                            }                        
                        }
                    }
                }
                return filteredContacts;
            }
        }

        private static List<ProspectingContactPerson> GetEmailTargetContactPersons(EmailBatch batch)
        {
            List<ProspectingContactPerson> contacts = new List<ProspectingContactPerson>();
            using (var prospecting = new ProspectingDataContext())
            {
                if (batch.Recipients.Count > 0)
                {
                    return batch.Recipients; // make sure the front-end populates these with all required values
                }

                if (batch.TargetAllMySuburbs)
                {
                    var targetContacts = GetEmailTargetContactPersons(batch.UserSuburbIds.ToArray());
                    contacts.AddRange(targetContacts);
                }

                if (batch.TargetCurrentSuburb)
                {
                    var targetContacts = GetEmailTargetContactPersons(new [] {batch.CurrentSuburbId.Value});
                    contacts.AddRange(targetContacts);
                }

                contacts = contacts.Distinct(new ContactPersonEmailComparer()).ToList();
                contacts = FilterContactsForTemplateType(contacts, batch.TemplateActivityTypeId);

                return contacts;
            }
        }

        private static List<ProspectingContactPerson> GetEmailTargetContactPersons(int[] suburbIDs)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                var propertyOwnerContacts = from pp in prospecting.prospecting_properties
                                            join pr in prospecting.prospecting_person_property_relationships on pp.prospecting_property_id equals pr.prospecting_property_id
                                            join cp in prospecting.prospecting_contact_persons on pr.contact_person_id equals cp.contact_person_id
                                            join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
                                            where ProspectingLookupData.EmailTypeIds.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted
                                            && suburbIDs.Contains(pp.seeff_area_id.Value) && pp.prospected == true && pp.latest_reg_date == null && !cp.optout_emails && !cp.is_popi_restricted
                                            select new ProspectingContactPerson 
                                            { 
                                                ContactPersonId = cp.contact_person_id,
                                                Title = cp.person_title,
                                                Firstname = cp.firstname,
                                                Surname = cp.surname,
                                                IdNumber = cp.id_number,
                                                TargetLightstonePropertyIdForComms = pp.lightstone_property_id, 
                                                TargetContactEmailAddress = cd.contact_detail
                                            };

                var propertyCompanyContacts = from pp in prospecting.prospecting_properties
                                              join cpr in prospecting.prospecting_company_property_relationships on pp.prospecting_property_id equals cpr.prospecting_property_id
                                              join pcr in prospecting.prospecting_person_company_relationships on cpr.contact_company_id equals pcr.contact_company_id
                                              join cp in prospecting.prospecting_contact_persons on pcr.contact_person_id equals cp.contact_person_id
                                              join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
                                              where ProspectingLookupData.EmailTypeIds.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted
                                              && suburbIDs.Contains(pp.seeff_area_id.Value) && pp.prospected == true && pp.latest_reg_date == null && !cp.optout_emails && !cp.is_popi_restricted
                                              select new ProspectingContactPerson
                                              {
                                                  ContactPersonId = cp.contact_person_id,
                                                  Title = cp.person_title,
                                                  Firstname = cp.firstname,
                                                  Surname = cp.surname,
                                                  IdNumber = cp.id_number,
                                                  TargetLightstonePropertyIdForComms = pp.lightstone_property_id,
                                                  TargetContactEmailAddress = cd.contact_detail
                                              };

                return propertyOwnerContacts.Union(propertyCompanyContacts).ToList(); 
            }
        }

        public static CostOfBatch CalculateCostOfEmailBatch(EmailBatch batch)
        {
            var targetContacts = GetEmailTargetContactPersons(batch);
            return new CostOfBatch
            {
                UnitCost = 0.01M,
                NumberOfUnits = targetContacts.Count
            };
        }

        public static CostOfBatch CalculateCostOfSmsBatch(SmsBatch batch)
        {
            var recipients = GetRecipientsFromBatch(batch);
            int totalUnits = 0;
            foreach (var recipient in recipients)
            {
                var numberUnits = Math.Ceiling((decimal)recipient.SMSMessage.Length / 160.0M);
                totalUnits += (int)numberUnits;
            }
            return new CostOfBatch
            {
                UnitCost = 0.19M,
                NumberOfUnits = totalUnits
            };
        }

        private static CostOfBatch CalculateCostOfSmsBatch(List<SmsRecipient> recipients)
        {
             int totalUnits = 0;
            foreach (var recipient in recipients)
            {
                var numberUnits = Math.Ceiling((decimal)recipient.SMSMessage.Length / 160.0M);
                totalUnits += (int)numberUnits;
            }
            return new CostOfBatch
            {
                UnitCost = 0.19M,
                NumberOfUnits = totalUnits
            };
        }

        public static CommunicationBatchStatus SubmitSMSBatch(SmsBatch batch)
        {
            try
            {
                byte[] data = Convert.FromBase64String(batch.SmsBodyRaw);
                batch.SmsBodyRaw = Encoding.UTF8.GetString(data);

                data = Convert.FromBase64String(batch.NameOfBatch);
                batch.NameOfBatch = Encoding.UTF8.GetString(data);

                var smsRecipients = GetRecipientsFromBatch(batch);
                EnqueueBatch(smsRecipients, batch);
                var batchCost = CalculateCostOfSmsBatch(smsRecipients);
                DebitUserBalanceForSmsBatch(batchCost.TotalCost);

                return new CommunicationBatchStatus { SuccessfullySubmitted = true };
            }
            catch (Exception ex)
            {
                using (var prospectingDb = new ProspectingDataContext())
                {
                    var errorRec = new exception_log
                    {
                        friendly_error_msg = ex.Message,
                        exception_string = ex.ToString(),
                        user = RequestHandler.GetUserSessionObject().UserGuid,
                        date_time = DateTime.Now
                    };
                    prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                    prospectingDb.SubmitChanges();
                }
                return new CommunicationBatchStatus
                {
                    SuccessfullySubmitted = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private static void DebitUserBalanceForSmsBatch(decimal cost)
        {
            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {            
                var prospectingUser = RequestHandler.GetUserSessionObject();
                prospectingAuthService.DebitUserBalance(cost, prospectingUser.UserGuid);
            }
        }       
    }
}