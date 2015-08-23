using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CommReportFilters
    {
        public string MessageType { get; set; }
        public bool SentByMe { get; set; } // If not by me, then anyone in my business unit.
        public string SentStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string BatchName { get; set; }
        public string TargetEmailAddress { get; set; }
        public string TargetCellNo { get; set; }
        public int? TargetLightstonePropertyId { get; set; }
    }
}