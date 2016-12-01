using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class NewMandateInputs
    {
        public int LightstonePropertyId { get; set; }
        public int MandateAgencyID { get; set; }
        public string MandateAgents { get; set; }
        public int MandateType { get; set; }
        public int MandateStatus { get; set; }
        public decimal? ListingPrice {get;set;}
        public DateTime? MandateFollowupDate { get; set; }
        public string MandateFollowupTypeText { get; set; }
        public Guid? FollowupAllocatedTo { get; set; }
        public string FollowupComment { get; set; }
    }
}