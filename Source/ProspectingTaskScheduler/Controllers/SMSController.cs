using ProspectingTaskScheduler.Core.Communication;
using ProspectingTaskScheduler.Core.Communication.SMSing;
using ProspectingTaskScheduler.Core.Housekeeping;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace ProspectingTaskScheduler.Controllers
{
    public class SMSController : ApiController
    {
        [HttpGet]
        public void ProcessReply()
        {
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                using (var prospecting = new ProspectingDataContext())
                {
                    // Find the latest matching record for this userid (by latest I mean the last record inserted)
                    string apiTrackingID = nvc["panacea_msg_uuid"];
                    var targetRecord = prospecting.sms_communications_logs.FirstOrDefault(rec => rec.api_tracking_id == apiTrackingID);
                    if (targetRecord != null)
                    {
                        string replyText = nvc["message"] ?? string.Empty;
                        targetRecord.reply = replyText;
                        targetRecord.status = CommunicationHelpers.GetCommunicationStatusId("SMS_REPLY");
                        targetRecord.updated_datetime = DateTime.Now;
                        // Create a follow up using the reply
                        long? followupActivityId = null;
                        if (targetRecord.activity_log != null)
                        {
                            var followupActivity = new activity_log
                            {
                                lightstone_property_id = targetRecord.target_lightstone_property_id,
                                followup_date = DateTime.Now,
                                allocated_to = targetRecord.created_by_user_guid,
                                activity_type_id = targetRecord.batch_activity_type_id,
                                comment = BuildCommentForFollowupReply(replyText, targetRecord),
                                created_by = new Guid(),
                                created_date = DateTime.Now,
                                contact_person_id = targetRecord.target_contact_person_id,
                                parent_activity_id = targetRecord.activity_log_id,
                                activity_followup_type_id = 2 // NB soft code this at a later stage                            
                            };
                            prospecting.activity_logs.InsertOnSubmit(followupActivity);
                            prospecting.SubmitChanges();
                            followupActivityId = followupActivity.activity_log_id;
                        }

                        targetRecord.followup_activity_id = followupActivityId;
                        prospecting.SubmitChanges();

                        // Capture email address against contact if present in reply:
                        Regex regex = new Regex(@"\b[a-zA-Z0-9.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z0-9.-]+\b");
                        var matches = regex.Matches(replyText);
                        if (matches != null && matches.Count > 0)
                        {
                            var matchEnumerable = matches.Cast<Match>();
                            var target = matchEnumerable.First();
                            string email = target.Value;

                            // Find all email addresses associated with this contact person and see if ours already exists
                            var existingContactDetails = prospecting.prospecting_contact_details.Where(cd => cd.contact_person_id == targetRecord.target_contact_person_id);
                            bool contactDetailExists = existingContactDetails.FirstOrDefault(ed => ed.contact_detail == email) != null;
                            if (!contactDetailExists)
                            {
                                prospecting_contact_detail newEmailAddress = new prospecting_contact_detail
                                {
                                    contact_detail_type = 4,
                                    contact_person_id = targetRecord.target_contact_person_id,
                                    contact_detail = email,
                                    is_primary_contact = false                                     
                                };
                                prospecting.prospecting_contact_details.InsertOnSubmit(newEmailAddress);
                                prospecting.SubmitChanges();
                            } 
                        }

                        if (replyText.ToUpper() == "STOP")
                        {
                            // Opt-out
                            var targetContactPersonRecord = prospecting.prospecting_contact_persons.FirstOrDefault(c => c.contact_person_id == targetRecord.target_contact_person_id);
                            // We need to find the record for the cell phone number that was used
                            if (targetContactPersonRecord != null)
                            {
                                targetContactPersonRecord.optout_sms = true;
                                prospecting.SubmitChanges();
                                string cellMatch = null;
                                foreach (var cd in targetContactPersonRecord.prospecting_contact_details)
                                {
                                    string testNumber = cd.prospecting_area_dialing_code.dialing_code_id + cd.contact_detail.Remove(0, 1);
                                    if (testNumber == targetRecord.target_cellphone_no)
                                    {
                                        cellMatch = cd.contact_detail;
                                        break;
                                    }
                                }
                                if (!string.IsNullOrEmpty(cellMatch))
                                {
                                    var allMatches = prospecting.prospecting_contact_details.Where(cd => cd.contact_detail == cellMatch);
                                    foreach (var item in allMatches)
                                    {
                                        item.prospecting_contact_person.optout_sms = true;
                                        prospecting.SubmitChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusNotifier.SendEmail("danie.vdm@seeff.com", "Task Scheduler", "reports@seeff.com", null, "Exception in SMSController.ProcessReply()", ex.ToString());
            }
        }

        private string BuildCommentForFollowupReply(string Fullsms, sms_communications_log targetRecord)
        {
            string comment = "*** Reply to SMS communication ***" + Environment.NewLine +
                "You have received the following reply from an SMS to contact number " + targetRecord.target_cellphone_no + Environment.NewLine + Environment.NewLine +
                Fullsms + Environment.NewLine + Environment.NewLine +
                "If their reply contains the word 'STOP' it means they have elected to opt-out and their database record will be updated accordingly.";

            return comment;
        }

        [HttpGet]
        public void UpdateDeliveryStatus()
        {
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var param1 = nvc["recordID"];
                var param2 = nvc["status"];
                using (var prospecting = new ProspectingDataContext())
                {
                    int recordID = Convert.ToInt32(param1);
                    int apistatus = Convert.ToInt32(param2);
                    var targetRecord = prospecting.sms_communications_logs.FirstOrDefault(item => item.sms_communications_log_id == recordID);
                    if (targetRecord != null)
                    {
                        switch(apistatus)
                        {
                            case 1:
                                targetRecord.status = CommunicationHelpers.GetCommunicationStatusId("SMS_DELIVERED");
                                if (targetRecord.activity_log_id == null)
                                {
                                    targetRecord.activity_log_id = SMSHandler.CreateActivityForRecord(targetRecord);
                                }
                                targetRecord.api_delivery_status = "Delivered";
                                break;
                            case 2: 
                                targetRecord.status = CommunicationHelpers.GetCommunicationStatusId("SMS_OTHER");
                                targetRecord.api_delivery_status = "Undelivered";
                                break;
                            case 4: 
                                targetRecord.status = CommunicationHelpers.GetCommunicationStatusId("SMS_OTHER");
                                targetRecord.api_delivery_status = "Queued at network";
                                break;
                            case 8:
                                targetRecord.status = CommunicationHelpers.GetCommunicationStatusId("SMS_OTHER");
                                targetRecord.api_delivery_status = "Sent to network";
                                break;
                            case 16:
                                targetRecord.status = CommunicationHelpers.GetCommunicationStatusId("SMS_OTHER");
                                targetRecord.api_delivery_status = "Failed at network";
                                break;
                            default:
                                targetRecord.status = CommunicationHelpers.GetCommunicationStatusId("SMS_OTHER");
                                targetRecord.api_delivery_status = "Unknown status: " + apistatus;
                                break;
                        }

                        targetRecord.updated_datetime = DateTime.Now;

                        prospecting.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusNotifier.SendEmail("danie.vdm@seeff.com", "Task Scheduler", "reports@seeff.com", null, "Exception updating delivery status", Request.RequestUri.Query + " - " + ex.ToString());
            }
        }
    }
}
