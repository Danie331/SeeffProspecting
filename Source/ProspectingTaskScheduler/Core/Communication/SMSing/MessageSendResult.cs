using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.SMSing
{
    public class MessageSendResult
    {
        public int status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
    }
}