using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    // Represents a ready-to-send email object
    public class EmailRecipient
    {
        public string Fullname { get; set; }
        public string ToEmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public int TargetLightstonePropertyId { get; set; }
        public int ContactPersonId { get; set; }
    }
}