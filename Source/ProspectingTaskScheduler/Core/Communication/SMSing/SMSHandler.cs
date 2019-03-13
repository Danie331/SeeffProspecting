using Hangfire;
using Newtonsoft.Json;
using ProspectingTaskScheduler.Core.Housekeeping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.SMSing
{
    public class SMSHandler
    {
        [AutomaticRetry(Attempts = 0)]
        public static async Task SendSMS(IJobCancellationToken cancellationToken)
        {
            try
            {
                var batch = await GetBatch5();
                await SubmitBatchAsync(batch, cancellationToken);
            }
            catch (Exception ex)
            {
                StatusNotifier.SendEmail("danie.vdm@seeff.com", "Task Scheduler", "reports@seeff.com", null, "Exception in SMSHandler.SendSMS()", ex.ToString() + " --- Stack-trace --- " + ex.StackTrace);
            }
        }

        private static async Task<List<sms_communications_log>> GetBatch5()
        {
            using (var prospecting = new seeff_prospectingEntities())
            {
                List<sms_communications_log> batch = new List<sms_communications_log>();
                if (DateTime.Now.Hour > 19 || DateTime.Now.Hour < 8)
                {
                    return batch;
                }

                int pendingStatus = CommunicationHelpers.GetCommunicationStatusId("PENDING_SUBMIT_TO_API");

                batch = await prospecting.sms_communications_log.Where(sms => sms.status == pendingStatus)
                                                            .OrderBy(item => item.created_datetime)
                                                            .Take(5)
                                                            .ToListAsync();

                return batch;
            }
        }

        private static KeyValuePair<int, string> GetMessageStatus(MessageSendResult result)
        {
            int prospectingSmsStatus;
            switch (result.status)
            {
                case 1:
                case 2:
                    prospectingSmsStatus = CommunicationHelpers.GetCommunicationStatusId("SMS_SUBMITTED");
                    break;
                case 4:
                    prospectingSmsStatus = CommunicationHelpers.GetCommunicationStatusId("SMS_DELIVERED");
                    break;
                default:
                    prospectingSmsStatus = CommunicationHelpers.GetCommunicationStatusId("SMS_OTHER");
                    break;
            }

            KeyValuePair<int, string> statusDesc = new KeyValuePair<int, string>(prospectingSmsStatus, result.message);

            return statusDesc;
        }

        private static async Task SubmitBatchAsync(List<sms_communications_log> batch, IJobCancellationToken cancellationToken)
        {
            using (var prospecting = new seeff_prospectingEntities())
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://api.panaceamobile.com/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = null;
                    foreach (var item in batch)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            string encodedMsg = HttpUtility.UrlEncode(item.msg_body_or_link_id);
                            string encodedCallbackURL = HttpUtility.UrlEncode("http://154.70.214.213/ProspectingTaskScheduler/api/SMS/UpdateDeliveryStatus?recordID=" + item.sms_communications_log_id + "&status=%d");
                            string targetURI = string.Format("json?action=message_send&username=seeffnational&password=dPBbboXWMgqz5GqZ2f1C&to={0}&text={1}&from={2}&report_mask=31&report_url={3}",
                                                            item.target_cellphone_no,
                                                            encodedMsg,
                                                            "27724707471",
                                                            encodedCallbackURL);

                            var targetRecord = await prospecting.sms_communications_log.FirstAsync(rec => rec.sms_communications_log_id == item.sms_communications_log_id);

                            if (targetRecord.status != CommunicationHelpers.GetCommunicationStatusId("PENDING_SUBMIT_TO_API"))
                                continue;

                            response = await client.GetAsync(targetURI);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            MessageSendResult messageResult = JsonConvert.DeserializeObject<MessageSendResult>(responseContent);

                            var messageStatus = GetMessageStatus(messageResult);

                            targetRecord.api_delivery_status = messageStatus.Value;
                            targetRecord.updated_datetime = DateTime.Now;
                            targetRecord.status = messageStatus.Key;
                            targetRecord.api_tracking_id = messageResult.details;
                            targetRecord.msg_body_or_link_id = null;

                            await prospecting.SaveChangesAsync();
                        }
                        catch (OperationCanceledException)
                        {
                            // Suppress and return as the job will be retried during its next scheduled run.
                            return;
                        }
                        catch (Exception ex)
                        {
                            StatusNotifier.SendEmail("danie.vdm@seeff.com", "Task Scheduler", "reports@seeff.com", null, "Exception whilst submitting SMS record", ex.ToString());
                        }
                    }
                }
            }
        }

        private async Task LogRecordUpdateException(string context, Exception e)
        {
            using (var prospecting = new seeff_prospectingEntities())
            {
                exception_log errorRecord = new exception_log
                {
                    friendly_error_msg = context,
                    date_time = DateTime.Now,
                    exception_string = e.ToString(),
                    user = Guid.NewGuid()
                };
                prospecting.exception_log.Add(errorRecord);
                await prospecting.SaveChangesAsync();
            }
        }

        public static async Task<long> CreateActivityForRecord(sms_communications_log smsItem)
        {
            using (var prospecting = new seeff_prospectingEntities())
            {
                var activityRecord = new activity_log
                {
                    lightstone_property_id = smsItem.target_lightstone_property_id,
                    followup_date = null,
                    allocated_to = smsItem.created_by_user_guid,
                    activity_type_id = smsItem.batch_activity_type_id,
                    comment = BuildCommentForSMSActivity(smsItem),
                    created_by = smsItem.created_by_user_guid,
                    created_date = DateTime.Now,
                    contact_person_id = smsItem.target_contact_person_id,
                    // Add the rest later
                    parent_activity_id = null,
                    activity_followup_type_id = null
                };
                prospecting.activity_log.Add(activityRecord);
                try
                {
                    await prospecting.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    using (var newContext = new seeff_prospectingEntities())
                    {
                        string msg = "Error inserting activity record for SMS communication sent. (SMS comm record id: " + smsItem.sms_communications_log_id + ")";
                        exception_log logentry = new exception_log
                        {
                            friendly_error_msg = msg,
                            exception_string = e.ToString(),
                            date_time = DateTime.Now,
                            user = smsItem.created_by_user_guid
                        };
                        newContext.exception_log.Add(logentry);
                        await newContext.SaveChangesAsync();
                    }
                }
                return activityRecord.activity_log_id;
            }
        }

        private static string BuildCommentForSMSActivity(sms_communications_log smsItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*** SMS sent to contact person ***");
            sb.AppendLine(string.Format("An SMS was sent to {0} at {1}.", smsItem.target_cellphone_no, DateTime.Now));
            return sb.ToString();
        }
    }
}