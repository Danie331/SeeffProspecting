using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class ErrorResponse
    {
        public string field { get; set; }
        public string message { get; set; }
    }
}