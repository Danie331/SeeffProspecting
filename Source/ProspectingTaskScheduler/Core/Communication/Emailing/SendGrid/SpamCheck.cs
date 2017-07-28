using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class SpamCheck
    {
        public bool enable { get; set; }
        public int threshold { get; set; }
        public string post_to_url { get; set; }
    }
}