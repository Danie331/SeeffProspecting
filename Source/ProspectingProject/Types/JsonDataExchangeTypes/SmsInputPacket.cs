using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SmsInputPacket
    {
        public List<SmsRecipient> TargetRecipients { get; set; }
    }
}