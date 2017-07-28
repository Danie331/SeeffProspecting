using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class Personalization
    {
        public List<MessageTo> to { get; set; }
        public string subject { get; set; }
        public List<MessageTo> bcc { get; set; }
        public CustomData custom_args { get; set; }
    }
}