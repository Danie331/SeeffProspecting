using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class FollowUpActivity
    {
        public long ActivityLogId { get; set; }
        public int LightstonePropertyId { get; set; }
        public DateTime? FollowupDate { get; set; }
        public int ActivityTypeId { get; set; }
        public string Comment { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByUsername { get; set; }
        public int? RelatedToContactPersonId { get; set; }
        public long? ParentActivityId { get; set; }
        public string PrimaryContactNo { get; set; }
        public string PrimaryEmailAddress { get; set; }
        public string PropertyAddress { get; set; }

        public string ActivityTypeName { get; set; }

        public int? ActivityFollowupTypeId { get; set; }
        public string FollowupActivityTypeName { get; set; }

        public ProspectingContactPerson RelatedToContactPerson { get; set; }
    }
}