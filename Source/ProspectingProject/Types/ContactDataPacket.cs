using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactDataPacket : BaseDataRequestPacket
    {
        public ProspectingContactPerson ContactPerson { get; set; }
        public int? ProspectingPropertyId { get; set; }
        public int? ContactCompanyId { get; set; }
    }
}