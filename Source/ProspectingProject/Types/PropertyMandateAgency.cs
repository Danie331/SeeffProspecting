using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class PropertyMandateAgency
    {
        public string Agency { get; set; }
        public string MandateType { get; set; }
        public string Status { get; set; }
        public string ListingPrice { get; set; }
        public string ExpiryDate { get; set; }
        public string FollowupDate { get; set; }
        public string FollowupType { get; set; }
        public string Agents { get; set; }
        public int PropertyMandateAgencyID { get; internal set; }

        public int id { get { return PropertyMandateAgencyID; } }
    }
}