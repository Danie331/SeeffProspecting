using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ProspectingActivity
    {
        public bool IsForInsert { get; set; }
        public bool IsForUpdate { get; set; }

        public long ActivityLogId { get; set; }

        public DateTime? FollowUpDate { get; set; }

        public Guid? AllocatedTo { get; set; }

        public int ActivityTypeId { get; set; }

        public string ActivityType { get; set; }

        public string Comment { get; set; }

        public Guid CreatedByGuid { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ContactPersonId { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool? Deleted { get; set; }

        public long? ParentActivityId { get; set; }

        public int? SmsTemplateId { get; set; }

        public bool? SmsSent { get; set; }

        public int? EmailTemplateId { get; set; }

        public bool? EmailSent { get; set; }

        public bool? PhoneCall { get; set; }

        public bool? Visit { get; set; }

        public int LightstonePropertyId { get; set; }

        public string CreatedBy { get; set; }

        public string AllocatedToName { get; set; }

        public int? ActivityFollowupTypeId { get; set; }

        public string RelatedToContactPersonName { get; set; }

        public string ActivityFollowupTypeName { get; set; }

        public int? SeeffAreaId { get; set; }

        public string PropertyType { get; set; }

        public string PropertyAddress { get; set; }
    }
}