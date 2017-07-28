using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class Response
    {
        public List<ErrorResponse> errors { get; set; }
    }
}