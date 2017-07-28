using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class TrackingSettings
    {
        public ClickTracking click_tracking { get; set; }
        public OpenTracking open_tracking { get; set; }
    }
}