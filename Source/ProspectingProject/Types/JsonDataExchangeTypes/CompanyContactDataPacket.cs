using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CompanyContactDataPacket
    {
        public ProspectingContactCompany ContactCompany { get; set; }
        public int? ProspectingPropertyId { get; set; }
    }
}