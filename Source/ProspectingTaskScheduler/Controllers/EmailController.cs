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
                // To verify this request we need to compare the ContactPersonId and ContactDetail in the request to a valid contact person in the database

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
    }
}
