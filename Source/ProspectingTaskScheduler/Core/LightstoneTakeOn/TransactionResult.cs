using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    public class TransactionResult
    {
        public string unique_id { get; set; }
        public string division { get; set; }
        public int pErrorNo { get; set; }
        public string pErrorMessage { get; set; }
    }
}