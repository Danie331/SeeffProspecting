
namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillSendRequest
    {
        public string key { get; set; }
        public MandrillEmailMessage message { get; set; }
        public bool async { get; set; }
        public string ip_pool { get; set; }
        public string send_at { get; set; }
    }
}
