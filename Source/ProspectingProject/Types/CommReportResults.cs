using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CommReportResults
    {
       public List<SentEmailLogItem> EmailLogItems { get; set; }
       public List<SentSMSLogItem> SMSLogItems { get; set; }

        public int TotalResultsPerFilterCriteria { get; set; }
    }
}