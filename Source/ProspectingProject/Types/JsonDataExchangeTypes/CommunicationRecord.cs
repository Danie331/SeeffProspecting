using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CommunicationRecord
    {
        public string CommContext { get; set; }
        public string CommType { get; set; }
        public int TargetContactPersonId { get; set; }
        public string TargetContactDetail { get; set; }
        public int TargetLightstonePropId { get; set; }
        public string SentStatus { get; set; }
        public string SendingError { get; set; }
        public string MessageBase64 { get; set; }
        public string SubjectText { get; set; }
    }
}