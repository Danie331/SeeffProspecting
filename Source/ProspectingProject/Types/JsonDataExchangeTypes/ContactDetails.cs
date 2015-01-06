using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactDetails : BaseDataRequestPacket
    {
        public List<string> PhoneNumbers { get; set; }
        public List<string> EmailAddresses { get; set; }

        public string IdNumber { get; set; }
    }
}