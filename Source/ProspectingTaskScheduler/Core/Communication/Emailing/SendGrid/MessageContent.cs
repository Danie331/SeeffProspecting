using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class MessageContent
    {
        public string type { get; set; }
        public string value { get; set; }
    }
}