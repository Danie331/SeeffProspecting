using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Notifications
{
    public class ProspectingFollowup
    {
        public DateTime FollowupDate { get; set; }
        public string ActivityTypeName { get; set; }
        public string Comment { get; set; }
        public string CreatedByUsername { get; set; }
        public string RelatedToContactPerson { get; set; }
        //public string PrimaryContactNo { get; set; }
        //public string PrimaryEmailAddress { get; set; }
        //public string PrimaryContactNoFormatted { get; set; }
        //public string PrimaryEmailAddressFormatted { get; set; }
        public string PropertyAddress { get; set; }
        public string FollowupActivityTypeName { get; set; }       
    }
}