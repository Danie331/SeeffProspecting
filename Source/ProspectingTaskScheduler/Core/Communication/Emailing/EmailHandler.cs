using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ProspectingTaskScheduler.Core.Communication;
using ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill;
using System.Net.Http;

namespace ProspectingTaskScheduler.Core.Communication.Emailing
{
    public class EmailHandler
    {
        private static long CreateActivityForRecord(email_communications_log emailItem)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var activityRecord = new activity_log
                {
                    lightstone_property_id = emailItem.target_lightstone_property_id,
                    followup_date = null,
                    allocated_to = emailItem.created_by_user_guid,
                    activity_type_id = emailItem.batch_activity_type_id,
                    comment = BuildCommentForEmailActivity(emailItem),
                    created_by = emailItem.created_by_user_guid,
                    created_date = DateTime.Now,
                    contact_person_id = emailItem.target_contact_person_id,
                    // Add the rest later
                    parent_activity_id = null,
                    activity_followup_type_id = null
                };
                prospecting.activity_logs.InsertOnSubmit(activityRecord);
                try
                {
                    prospecting.SubmitChanges();
                }
                catch (Exception e)
                {
                    using (var newContext = new ProspectingDataContext())
                    {
                        string msg = "Error inserting activity record for email communication sent. (Email comm record id: " + emailItem.email_communications_log_id + ")";
                        exception_log logentry = new exception_log
                        {
                            friendly_error_msg = msg,
                            exception_string = e.ToString(),
                            date_time = DateTime.Now,
                            user = emailItem.created_by_user_guid
                        };
                        newContext.exception_logs.InsertOnSubmit(logentry);
                        newContext.SubmitChanges();
                    }
                }
                return activityRecord.activity_log_id;
            }
        }

        private static string BuildCommentForEmailActivity(email_communications_log emailItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*** Email sent to contact person ***");
            sb.AppendLine(string.Format("An email was sent to {0} at {1}.", emailItem.target_email_address, DateTime.Now));
            sb.AppendLine(string.Format("Subject of the email: \"{0}\"", emailItem.email_subject_or_link_id));
            return sb.ToString();
        }

            private static void UpdateRecord(email_communications_log emailItem, EmailResult sendingResult)
            {
                if (sendingResult.Success)
                {
                    int emailSuccessStatus = CommunicationHelpers.GetCommunicationStatusId("EMAIL_SENT");
                    long activityLogId = CreateActivityForRecord(emailItem);
                    emailItem.activity_log_id = activityLogId;
                    emailItem.status = emailSuccessStatus;
                    emailItem.updated_datetime = DateTime.Now;
                    emailItem.email_body_or_link_id = null;
                    emailItem.email_subject_or_link_id = null;
                    emailItem.attachment1_content = null;
                    emailItem.attachment1_name = null;
                    emailItem.attachment1_type = null;
                    emailItem.api_tracking_id = sendingResult.ApiTrackingKey;

                    return;
                }

                // Handle the unsent case
                int emailFailedStatus = CommunicationHelpers.GetCommunicationStatusId("EMAIL_UNSENT");
                emailItem.updated_datetime = DateTime.Now;
                emailItem.status = emailFailedStatus;
                emailItem.email_body_or_link_id = null;
                emailItem.email_subject_or_link_id = null;
                emailItem.attachment1_content = null;
                emailItem.attachment1_name = null;
                emailItem.attachment1_type = null;
                emailItem.error_msg = sendingResult.ErrorMessage;
                emailItem.api_tracking_id = sendingResult.ApiTrackingKey;
            }


            public static void SendEmails()
            {
                using (var prospecting = new ProspectingDataContext())
                {
                    int pendingStatus = CommunicationHelpers.GetCommunicationStatusId("PENDING_SUBMIT_TO_API");
                    var batch = prospecting.email_communications_logs.Where(em => em.status == pendingStatus).Take(10).ToList();
                    int awaitingStatus = CommunicationHelpers.GetCommunicationStatusId("AWAITING_RESPONSE_FROM_API");
                    foreach (var item in batch)
                    {
                        item.status = awaitingStatus;
                        try
                        {
                            prospecting.SubmitChanges();
                        }
                        catch(Exception e)
                        {
                            LogUpdateRecordError(e, "Unexpected error updating the status of email comm record to AWAITING_RESPONSE_FROM_API", item);
                        }
                    }
                    foreach (var item in batch)
                    { 
                        EmailResult result = SendEmail(item);
                        UpdateRecord(item, result);
                        try
                        {
                            prospecting.SubmitChanges();
                        }
                        catch (Exception e)
                        {
                            // If an error occurred trying to update the record in the DB, log it
                            LogUpdateRecordError(e, "Unexpected error when saving updates to email comm. record after send.", item);
                        }
                    }
                }
            }

            private static void LogUpdateRecordError(Exception e, string context, email_communications_log record)
            {
                using (var newContext = new ProspectingDataContext())
                {
                    string msg = context + "(Email communication record id: " + record.email_communications_log_id + ")";
                    exception_log logentry = new exception_log
                    {
                        friendly_error_msg = msg,
                        exception_string = e.ToString(),
                        date_time = DateTime.Now,
                        user = record.created_by_user_guid
                    };
                    newContext.exception_logs.InsertOnSubmit(logentry);
                    newContext.SubmitChanges();
                }
            }

        private static EmailResult SendEmail(email_communications_log pendingItem)
        {
            EmailResult result = new EmailResult { Success = true };
            try
            {
                var titlecaser = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                string contactFullname = titlecaser.ToTitleCase(pendingItem.prospecting_contact_person.firstname.ToLower()) + " " + titlecaser.ToTitleCase(pendingItem.prospecting_contact_person.surname.ToLower());

                MandrillMessageBuilder builder = new MandrillMessageBuilder(pendingItem.email_body_or_link_id,
                    pendingItem.email_subject_or_link_id,
                    pendingItem.created_by_user_email_address,
                    pendingItem.created_by_user_name,
                    pendingItem.target_email_address,
                    contactFullname,
                    pendingItem.attachment1_name,
                    pendingItem.attachment1_type,
                    pendingItem.attachment1_content,
                    pendingItem.user_business_unit_id);

                var req = builder.BuildMandrillSendRequest();
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://mandrillapp.com/");
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;
                try
                {
                    response = client.PostAsync("/api/1.0/messages/send.json", httpContent).Result;

                    var mandrillResponseArray = response.Content.ReadAsAsync<MandrillSuccessfulResponse[]>().Result;
                    MandrillSuccessfulResponse successResponse = mandrillResponseArray[0];
                    result.ApiTrackingKey = successResponse._id;
                    if (successResponse.status != "sent" && !string.IsNullOrEmpty(successResponse.reject_reason))
                    {
                        result.Success = false;
                        result.ErrorMessage = successResponse.status + ": " + successResponse.reject_reason;
                    }
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.ErrorMessage = "Error occurred during POST req to emailing API for message id: " + pendingItem.email_communications_log_id + " -- detailed exception: " + e.ToString();
                }
                // NB: we do not as yet check any statuses from the API response, only whether an exception occured during the POST.
                if (response != null)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        result.Success = false;
                        result.ErrorMessage = "The POST req while sending an email to the API failed - " + response.ReasonPhrase;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = "Error occurred while building the email message id: " + pendingItem.email_communications_log_id + " -- detailed exception: " + ex.ToString();
            }

            return result;
        }
    }
}