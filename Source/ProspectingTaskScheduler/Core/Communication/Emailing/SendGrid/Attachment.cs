using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class Attachment
    {
        public string content { get; set; }
        public string type { get; set; }
        public string filename { get; set; }
        public string disposition { get; set; }
        public string content_id { get; set; }
    }
}