using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillSuccessfulResponse
    {
        public string email { get; set; }
        public string status { get; set; }
        public string reject_reason { get; set; }
        public string _id { get; set; }
    }
}