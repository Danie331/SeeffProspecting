using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class EventCallbackWebhook
    {
        public string TransactionIdentifier { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        public string url { get; set; }
        public string sg_event_id { get; set; }
    }
}