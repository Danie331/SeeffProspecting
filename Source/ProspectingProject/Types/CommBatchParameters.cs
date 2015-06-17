using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CommBatchParameters
    {
        // Inputs
        public string CommunicationType { get; set; }
        public int RecipientCount { get; set; }
        public int? CurrentSuburb { get; set; }
        public bool TargetAllUserSuburbs { get; set; }

        // Outputs
        public decimal UnitCost { get; set; }
        public int NumberOfUnits { get; set; }
        public decimal AvailableCredit { get; set; }
        public decimal TotalCost
        {
            get { return ((decimal)UnitCost / 100.0M) * NumberOfUnits;  }
        }
    }
}