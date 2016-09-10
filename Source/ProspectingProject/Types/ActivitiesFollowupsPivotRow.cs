using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ActivitiesFollowupsPivotRow
    {
        [JsonProperty(PropertyName = "Activity Type")]
        public string ActivityType { get; internal set; }

        [JsonProperty(PropertyName = "Allocated To")]
        public string AllocatedTo { get; internal set; }

        [JsonProperty(PropertyName = "Created By")]
        public string CreatedBy { get; internal set; }

        [JsonProperty(PropertyName = "Followup Type")]
        public string FollowupType { get; set; }
    }
}