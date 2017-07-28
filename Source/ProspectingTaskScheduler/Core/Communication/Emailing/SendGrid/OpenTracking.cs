using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class OpenTracking
    {
        public bool enable { get; set; }
        public string substitution_tag { get; set; }
    }
}