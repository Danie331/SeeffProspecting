using ProspectingTaskScheduler.Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProspectingTaskScheduler.Controllers
{
    public class SMSController : ApiController
    {
        [HttpGet]
        public void ProcessReply(string senderid, string userid, string Fullsms, string timestamp)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                // Find the latest matching record for this userid (by latest I mean the last record inserted)
                var targetRecord = prospecting.sms_communications_logs.OrderByDescending(s => s.created_datetime).FirstOrDefault(s => s.target_cellphone_no == senderid);
                if (targetRecord != null && !string.IsNullOrWhiteSpace(Fullsms))
                {
                    targetRecord.reply = Fullsms;
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
                            comment = BuildCommentForFollowupReply(Fullsms, targetRecord),
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

                    if (Fullsms.ToUpper() == "STOP")
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

        private string BuildCommentForFollowupReply(string Fullsms, sms_communications_log targetRecord)
        {
            string comment = "*** Reply to SMS communication ***" + Environment.NewLine +
                "You have received the following reply from an SMS to contact number " + targetRecord.target_cellphone_no + Environment.NewLine + Environment.NewLine +
                Fullsms + Environment.NewLine + Environment.NewLine +
                "If their reply contains the word 'STOP' it means they have elected to opt-out and their database record will be updated accordingly.";

            return comment;
        }
    }
}
