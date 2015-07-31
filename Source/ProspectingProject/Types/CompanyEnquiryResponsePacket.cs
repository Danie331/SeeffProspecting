using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CompanyEnquiryResponsePacket
    {
        public string ErrorMsg { get; set; }
        public bool EnquirySuccessful { get; set; }
        public decimal? WalletBalance { get; set; }
        public List<ProspectingContactPerson> Contacts { get; set; }
    }
}