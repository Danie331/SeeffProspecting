using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SentSMSLogItem
    {
        public int id { get; set; }
        public string SentTo { get; set; }
        public DateTime DateSent { get; set; }
        public string FriendlyNameOfBatch { get; set; }
        public string SentBy { get; set; }
        public int TargetLightstonePropertyId { get; set; }
        public string DeliveryStatus { get; set; }
    }
}