using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing
{
    public class EmailResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string ApiTrackingKey { get; set; }
    }
}