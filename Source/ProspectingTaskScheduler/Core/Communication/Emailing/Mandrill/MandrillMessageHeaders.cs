
using Newtonsoft.Json;
namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillMessageHeaders
    {
        [JsonProperty("Reply-To")]
        public string reply_to{get; set;}
    }
}
