using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class TrustEnquiryResultPacket : CompanyEnquiryResponsePacket
    {
        public List<TrustSearchResult> Results = new List<TrustSearchResult>();

        public CompanyEnquiryInputPacket CurrentInfo { get; set; }
    }
}