using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillEventMessageResult
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("diag")]
        public string Diag { get; set; }

        [JsonProperty("bounce_description")]
        public string BounceDesc { get; set; }
    }
}