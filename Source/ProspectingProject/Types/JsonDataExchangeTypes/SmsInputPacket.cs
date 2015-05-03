using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SmsInputPacket
    {
        public string Message { get; set; }
        public List<string> TargetRecipients { get; set; }
    }
}