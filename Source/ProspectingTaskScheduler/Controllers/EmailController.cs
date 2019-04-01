using ProspectingTaskScheduler.Core.ClientSynchronisation;
using ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid;
using System;
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
            using (var prospectingContext = new seeff_prospectingEntities())
            {
                var contactPersons = from cd in prospectingContext.prospecting_contact_detail
                                     where cd.contact_detail == contactDetail
                                     select cd.prospecting_contact_person;
                var contactPersonIDs = contactPersons.Select(cp => cp.contact_person_id);

                if (contactPersonIDs.Any(c => c == contactPersonId))
                {
                    foreach (var contactPerson in contactPersons)
                    {
                        contactPerson.optout_emails = true;
                        contactPerson.email_contactability_status = 1;
                        prospectingContext.SaveChanges();
                        //ProspectingToCmsClientSynchroniser.AddClientSynchronisationRequest(contactPerson.contact_person_id, contactPerson.created_by);
                    }
                }
            }
        }

        [HttpGet]
        public HttpResponseMessage Optout(int contactPersonId)
        {
            using (var prospectingContext = new seeff_prospectingEntities())
            {
                var contactPerson = prospectingContext.prospecting_contact_person.FirstOrDefault(cp => cp.contact_person_id == contactPersonId);
                if (contactPerson != null)
                {
                    contactPerson.optout_emails = true;
                    contactPerson.email_contactability_status = 1;
                    prospectingContext.SaveChanges();
                    //ProspectingToCmsClientSynchroniser.AddClientSynchronisationRequest(contactPerson.contact_person_id, contactPerson.created_by);
                }
            }

            var response = Request.CreateResponse(HttpStatusCode.Found);
            response.Headers.Location = new Uri("https://www.seeff.com/Account/OptOut");
            return response;
        }

        [HttpGet]
        public HttpResponseMessage Optin(int contactPersonId)
        {
            using (var prospectingContext = new seeff_prospectingEntities())
            {
                var contactPerson = prospectingContext.prospecting_contact_person.FirstOrDefault(cp => cp.contact_person_id == contactPersonId);
                if (contactPerson != null)
                {
                    contactPerson.optout_emails = false;
                    contactPerson.email_contactability_status = 2;
                    prospectingContext.SaveChanges();
                    //ProspectingToCmsClientSynchroniser.AddClientSynchronisationRequest(contactPerson.contact_person_id, contactPerson.created_by);
                }
            }

            var response = Request.CreateResponse(HttpStatusCode.Found);
            response.Headers.Location = new Uri("https://www.seeff.com/Account/OptIn");
            return response;
        }

        [HttpPost]
        public HttpResponseMessage UpdateEmailDeliveryStatus()
        {
            int emailSuccessStatus = 6;
            int emailFailedStatus = 7;

            string[] successfulEvents = new[] { "delivered", "open", "click", "spamreport", "unsubscribe" };
            string[] unsuccessfulEvents = new[] { "processed", "dropped", "deferred", "bounce" };
            try
            {
                string raw = Request.Content.ReadAsStringAsync().Result;
                string decoded = System.Web.HttpUtility.UrlDecode(raw);
                using (var prospecting = new seeff_prospectingEntities())
                {
                    var eventsArray = Newtonsoft.Json.JsonConvert.DeserializeObject<EventCallbackWebhook[]>(decoded);
                    if (eventsArray != null && eventsArray.Count() > 0)
                    {
                        var eventGroups = eventsArray.GroupBy(gr => gr.sg_event_id);
                        foreach (var item in eventGroups)
                        {
                            var uniqueEvent = item.First();
                            var targetRecord = prospecting.email_communications_log.FirstOrDefault(em => em.api_tracking_id == uniqueEvent.TransactionIdentifier);
                            if (targetRecord != null)
                            {
                                // only update a 'success' event with another 'success' event
                                if (targetRecord.last_api_event_dump != null && successfulEvents.Any(e => e == targetRecord.last_api_event_dump))
                                {
                                    if (uniqueEvent.Event != null && successfulEvents.Any(e => e == uniqueEvent.Event))
                                    {
                                        targetRecord.status = emailSuccessStatus;
                                        targetRecord.updated_datetime = DateTime.Now;
                                        targetRecord.last_api_event_dump = uniqueEvent.Event;
                                        targetRecord.error_msg = null;
                                        prospecting.SaveChanges();
                                    }
                                }
                                else
                                {
                                    bool successStatus = uniqueEvent.Event != null && successfulEvents.Any(e => e == uniqueEvent.Event);
                                    targetRecord.status = successStatus ? emailSuccessStatus : emailFailedStatus;
                                    targetRecord.updated_datetime = DateTime.Now;
                                    targetRecord.last_api_event_dump = uniqueEvent.Event;
                                    targetRecord.error_msg = !successStatus ? uniqueEvent.Event : null;
                                    prospecting.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (var p = new seeff_prospectingEntities())
                {
                    exception_log e = new exception_log
                    {
                        date_time = DateTime.Now,
                        exception_string = ex.ToString(),
                        friendly_error_msg = "Error occurred in UpdateEmailDeliveryStatus() in Task Scheduler",
                        user = new Guid()
                    };
                    p.exception_log.Add(e);
                    p.SaveChanges();
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public void UnsubscribeProspectingUser(int reg_id)
        {
            using (var boss = new bossEntities())
            {
                var targetUser = boss.user_preference.FirstOrDefault(up => up.fk_user_registration_id == reg_id && up.fk_user_preference_type_id == 1);
                if (targetUser != null)
                {
                    boss.user_preference.Remove(targetUser);
                    boss.SaveChanges();
                }
            }
        }

    }
}
