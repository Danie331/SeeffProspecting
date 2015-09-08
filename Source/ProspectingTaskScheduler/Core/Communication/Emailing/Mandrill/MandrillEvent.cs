using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillEvent
    {
        [JsonProperty("event")]
        public string Event {get; set;}

        [JsonProperty("_id")]
        public string ApiTrackingId { get; set; }

        [JsonProperty("msg")]
        public MandrillEventMessageResult MessageResult { get; set; }    
    }
}