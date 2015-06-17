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

                var userSuburbs = (List<UserSuburb>)HttpContext.Current.Session["user_suburbs"];
                var prospectingUser = RequestHandler.GetUserSessionObject();

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    DebitUserBalance(batch, prospectingUser.UserGuid, userSuburbs);
                    var emailRecipients = GetRecipientsFromBatch(batch, userSuburbs);
                    EnqueueBatch(emailRecipients, batch, prospectingUser);
                });

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

        private static void DebitUserBalance(EmailBatch batch, Guid userGuid, List<UserSuburb> userSuburbs)
        {
            CommBatchParameters cost = new CommBatchParameters
            {
                CommunicationType = "EMAIL",
                RecipientCount = batch.Recipients != null ? batch.Recipients.Count : 0,
                CurrentSuburb = batch.CurrentSuburbId,
                TargetAllUserSuburbs = batch.ContactsInAllMySuburbs.HasValue ? batch.ContactsInAllMySuburbs.Value : false
            };

            var result = CalculateCostOfBatch(cost, userSuburbs);
            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                prospectingAuthService.DebitUserBalance(result.TotalCost, userGuid);
            }
        }

        private static List<EmailRecipient> GetRecipientsFromBatch(EmailBatch batch, List<UserSuburb> userSuburbs)
        {
            var contacts = batch.Recipients;
            if (contacts != null && contacts.Count > 0)
            {
                return CreateMailRecipients(contacts, batch);
            }

            // TODO: If we specified to send email to all contacts in the current suburb/ALL suburbs :: remember to de-select multi-select mode (and the selection) and also warn the user about the rules that will be used by the system when selecting the target contact detail.
            return CreateMailRecipientsForTargetSuburbs(batch, userSuburbs);
        }

        /// <summary>
        /// Assumption here that all contacts have at least one email address
        /// </summary>
        private static List<EmailRecipient> CreateMailRecipients(List<ProspectingContactPerson> contacts, EmailBatch batch)
        {
            contacts = FilterContactsForTemplateType(contacts, batch.TemplateActivityTypeId);

            List<EmailRecipient> recipients = new List<EmailRecipient>();
            using (var prospectingContext = new ProspectingDataContext())
            {
                // Note: The ProspectingContactPerson's in the batch come from the front-end and ARE NOT populated with data.
                // The contact record from the front-end holds only the information needed to identify the target contact person and lightstone property.
                foreach (var contact in contacts)
                {
                    var contactRecord = prospectingContext.prospecting_contact_persons.First(c => c.contact_person_id == contact.ContactPersonId);
                    string targetEmailAddress = GetDefaultEmailAddress(prospectingContext, contact); // TAKE NOTE: contactRecord != contact
                    string emailBody = CreateEmailBody(batch.EmailBodyHTMLRaw, contactRecord, contact.TargetLightstonePropertyIdForComms.Value, targetEmailAddress); // TAKE NOTE: contactRecord != contact
                    string emailSubject = CreateEmailSubject(batch.EmailSubjectRaw);

                    var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                    EmailRecipient recipient = new EmailRecipient
                    {
                        ContactPersonId = contactRecord.contact_person_id,
                        EmailBody = emailBody,
                        EmailSubject = emailSubject,
                        ToEmailAddress = targetEmailAddress,
                        TargetLightstonePropertyId = contact.TargetLightstonePropertyIdForComms.Value, // Must be present in this context.
                        Fullname = titlecaser.ToTitleCase(contactRecord.firstname.ToLower()) + " " + titlecaser.ToTitleCase(contactRecord.surname.ToLower())
                    };
                    recipients.Add(recipient);
                }
            }

            return recipients.Distinct(new EmailAddressComparer()).ToList();
        }

        private static List<EmailRecipient> CreateMailRecipientsForTargetSuburbs(EmailBatch batch, List<UserSuburb> userSuburbs)
        {
            List<EmailRecipient> recipients = new List<EmailRecipient>();
            using (var prospectingContext = new ProspectingDataContext())
            {
                List<int> targetSuburbs = new List<int>();
                if (batch.ContactsInCurrentSuburb.HasValue && batch.ContactsInCurrentSuburb.Value /*== true*/)
                {
                    targetSuburbs.Add(batch.CurrentSuburbId.Value);
                }
                else if (batch.ContactsInAllMySuburbs.HasValue && batch.ContactsInAllMySuburbs.Value /* == true */)
                {                    
                    targetSuburbs.AddRange(userSuburbs.Select(us => us.SuburbId));
                }
                var propertiesInSuburbs = prospectingContext.prospecting_properties.Where(pp => targetSuburbs.Contains(pp.seeff_area_id.Value));
                List<ProspectingContactPerson> allContacts = new List<ProspectingContactPerson>();
                int totalProps = propertiesInSuburbs.Count();
                int counter = 1;
                foreach (var property in propertiesInSuburbs)
                {
                    var percComplete = ((decimal)counter / (decimal)totalProps) * 100.0M;
                    counter++;
                    if (property.prospected == null || !property.prospected.Value)
                    {
                        continue;
                    }
                    var directContacts = ProspectingLookupData.PropertyContactsRetriever(prospectingContext, property, false).ToList();
                    var companyContacts = ProspectingLookupData.PropertyCompanyContactsRetriever(prospectingContext, property, false).ToList();
                    var allContactsForProperty = directContacts.Union(companyContacts).Distinct().ToList();
                    foreach (var contact in allContactsForProperty)
                    {
                        // Check if he has a default email address.
                        var emailAddresses = ProspectingLookupData.PropertyContactEmailRetriever(prospectingContext, contact).ToList();
                        if (!emailAddresses.Any())
                        {
                            continue;
                        }
                        var defaultEmailAddress = emailAddresses.FirstOrDefault(em => em.IsPrimary.HasValue && em.IsPrimary == true);
                        if (defaultEmailAddress == null) 
                        {
                            continue;
                        }
                        if (contact.EmailOptout || contact.IsPOPIrestricted)
                        {
                            continue;
                        }

                        contact.TargetLightstonePropertyIdForComms = property.lightstone_property_id;
                        allContacts.Add(contact);
                    }
                }

                return CreateMailRecipients(allContacts, batch);
            }
        }

        private static void EnqueueBatch(List<EmailRecipient> emailRecipients, EmailBatch batch, UserDataResponsePacket prospectingUser)
        {
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
                    prospecting.email_communications_logs.InsertOnSubmit(record);
                }
                prospecting.SubmitChanges();
            }
        }
       
        private static string CreateEmailSubject(string subjectText)
        {
            return subjectText;
        }

        private static string GetDefaultEmailAddress(ProspectingDataContext ctx, ProspectingContactPerson contact)
        {
            var emailAddresses = ProspectingLookupData.PropertyContactEmailRetriever(ctx, contact).ToList();
            var primaryDefaultEmail = emailAddresses.FirstOrDefault(em => em.IsPrimary == true);
            if (primaryDefaultEmail != null)
            {
                return primaryDefaultEmail.ItemContent;
            }

            return emailAddresses.First().ItemContent;
        }

        private static string CreateEmailBody(string rawBody, prospecting_contact_person personRecordFromDB, int lightstonePropertyID, string emailAddress)
        {
            var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;

            string personTitle = "";
            if (personRecordFromDB.person_title.HasValue)
            {
                personTitle = ProspectingLookupData.ContactPersonTitle.First(cpt => cpt.Key == personRecordFromDB.person_title).Value;
            }
            string name = titlecaser.ToTitleCase(personRecordFromDB.firstname.ToLower());
            string surname = titlecaser.ToTitleCase(personRecordFromDB.surname.ToLower());
            string address = ProspectingCore.GetFormattedAddress(lightstonePropertyID);

            rawBody = rawBody.Replace("*title*", personTitle)
                             .Replace("*name*", name)
                             .Replace("*surname*", surname)
                             .Replace("*address*", address);

            var link = "http://154.70.214.213/ProspectingTaskScheduler/api/Email/Optout?contactPersonId=" + personRecordFromDB.contact_person_id + "&contactDetail=" + emailAddress;
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
                        if (!string.IsNullOrEmpty(propertingProperty.lightstone_reg_date))
                        {
                            var regDate = DateTime.ParseExact(propertingProperty.lightstone_reg_date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                            var testDate = regDate.AddYears(years).Date;
                            var todaysDate = DateTime.Today;
                            if (testDate == todaysDate)
                            {
                                filteredContacts.Add(contact);
                            }
                        }
                    }
                }
                return filteredContacts;
            }
        }

        public static CommBatchParameters CalculateCostOfBatch(CommBatchParameters inputParams, List<UserSuburb> userSuburbs)
        {
            if (inputParams.CommunicationType == "EMAIL")
            {
                var costResults = new CommBatchParameters { UnitCost = 1 };
                if (inputParams.RecipientCount > 0)
                {                    
                    costResults.NumberOfUnits = inputParams.RecipientCount;
                }
                else if (inputParams.TargetAllUserSuburbs)
                {
                    costResults.NumberOfUnits = GetTargetEmailCountForUserSuburbs(userSuburbs);
                }
                else if (inputParams.CurrentSuburb.HasValue && inputParams.CurrentSuburb > 0)
                {
                    costResults.NumberOfUnits = GetTargetEmailsSuburb(inputParams.CurrentSuburb.Value).Count;
                }

                return costResults;
            }
            return new CommBatchParameters();
        }

        private static List<string> GetTargetEmailsSuburb(int suburbId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var propertyOwnerContacts = from pp in prospecting.prospecting_properties
                            join pr in prospecting.prospecting_person_property_relationships on pp.prospecting_property_id equals pr.prospecting_property_id
                            join cp in prospecting.prospecting_contact_persons on pr.contact_person_id equals cp.contact_person_id
                            join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
                            where ProspectingLookupData.EmailTypeIds.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted
                            && pp.seeff_area_id == suburbId && pp.prospected == true && !cp.optout_emails && !cp.is_popi_restricted
                            select cd.contact_detail;

                var propertyCompanyContacts = from pp in prospecting.prospecting_properties
                            join cpr in prospecting.prospecting_company_property_relationships on pp.prospecting_property_id equals cpr.prospecting_property_id
                            join pcr in prospecting.prospecting_person_company_relationships on cpr.contact_company_id equals pcr.contact_company_id
                            join cp in prospecting.prospecting_contact_persons on pcr.contact_person_id equals cp.contact_person_id
                            join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
                            where ProspectingLookupData.EmailTypeIds.Contains(cd.contact_detail_type) && cd.is_primary_contact && !cd.deleted
                            && pp.seeff_area_id == suburbId && pp.prospected == true && !cp.optout_emails && !cp.is_popi_restricted
                            select cd.contact_detail;

                return propertyOwnerContacts.Union(propertyCompanyContacts).Distinct().ToList();
            }
        }

        private static int GetTargetEmailCountForUserSuburbs(List<UserSuburb> userSuburbs)        
        {
            List<string> results = new List<string>();
            foreach (var suburb in userSuburbs)
            {
                results.AddRange(GetTargetEmailsSuburb(suburb.SuburbId));
            }
            return results.Distinct().Count();
        }
    }
}