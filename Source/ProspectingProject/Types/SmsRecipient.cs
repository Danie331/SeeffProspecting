using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SmsRecipient
    {
        public string Message { get; set; }
        public int ContactpersonId { get; set; }
        public int ProspectingPropertyId { get; set; }
        public string TargetCellNo { get; set; }
    }
}