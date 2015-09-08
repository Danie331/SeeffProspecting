using Newtonsoft.Json;
using ProspectingTaskScheduler.Core.Communication;
using ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProspectingTaskScheduler.Controllers
{
    public class EmailController : ApiController
    {
        [HttpGet]
        public void Optout(int contactPersonId, string contactDetail)
        {
            using (var prospectingContext = new ProspectingDataContext())
            {     
                var contactPersons = from cd in prospectingContext.prospecting_contact_details
                                     where cd.contact_detail == contactDetail
                                     select cd.prospecting_contact_person;
                var contactPersonIDs = contactPersons.Select(cp => cp.contact_person_id);

                if (contactPersonIDs.Any(c => c == contactPersonId))
                {
                    foreach (var contactPerson in contactPersons)
                    {
                        contactPerson.optout_emails = true;
                    }
                    prospectingContext.SubmitChanges();
                }
            }
        }

        [HttpPost]
        public void UpdateEmailDeliveryStatus()
        {
            Func<MandrillEventMessageResult, string> getFailureReason = mr => 
                {
                   string result = mr.State;
                    if (!string.IsNullOrEmpty(mr.BounceDesc)) {
                        result = result + " (" + mr.BounceDesc + ")";
                    }
                    if (!string.IsNullOrEmpty(mr.Diag)) {
                        result = result + ": " + mr.Diag;
                    }

                    return result;
                };

            int emailSuccessStatus = CommunicationHelpers.GetCommunicationStatusId("EMAIL_SENT");
            int emailFailedStatus = CommunicationHelpers.GetCommunicationStatusId("EMAIL_UNSENT");
            try
            {
                string raw = Request.Content.ReadAsStringAsync().Result;
                string decoded = System.Web.HttpUtility.UrlDecode(raw);

                decoded = decoded.Replace("mandrill_events=", "");
                MandrillEvent[] events = JsonConvert.DeserializeObject<MandrillEvent[]>(decoded);
                MandrillEvent target = events[0];

                if (target.MessageResult != null)
                {
                    using (var prospecting = new ProspectingDataContext())
                    {
                        var targetRecord = prospecting.email_communications_logs.FirstOrDefault(em => em.api_tracking_id == target.ApiTrackingId);
                        if (targetRecord != null)
                        {
                            string failureReason = string.Empty;
                            switch (target.Event)
                            {
                                case "send":
                                    if (target.MessageResult.State != "sent")
                                    {
                                        failureReason = getFailureReason(target.MessageResult);
                                        targetRecord.status = emailFailedStatus;
                                        targetRecord.error_msg = failureReason;
                                        targetRecord.updated_datetime = DateTime.Now;
                                        prospecting.SubmitChanges();
                                    }
                                    break;
                                case "deferral":
                                case "hard_bounce":
                                case "soft_bounce":
                                case "spam":
                                case "unsub":
                                case "reject":
                                    failureReason = getFailureReason(target.MessageResult);
                                    targetRecord.status = emailFailedStatus;
                                    targetRecord.error_msg = failureReason;
                                    targetRecord.updated_datetime = DateTime.Now;
                                    prospecting.SubmitChanges();
                                    break;
                                default: break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (var p = new ProspectingDataContext())
                {
                    exception_log e = new exception_log
                    {
                        date_time = DateTime.Now,
                        exception_string = ex.ToString(),
                        friendly_error_msg = "Error occurred in UpdateEmailDeliveryStatus() in Task Scheduler",
                        user = new Guid()
                    };
                    p.exception_logs.InsertOnSubmit(e);
                    p.SubmitChanges();
                }
            }            
        }
    }
}
