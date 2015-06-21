using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SmsRecipient
    {
        public string Fullname { get; set; }
        public string QualifiedCellNumber { get; set; }
        public string SMSMessage { get; set; }
        public int TargetLightstonePropertyId { get; set; }
        public int ContactPersonId { get; set; }
    }
}