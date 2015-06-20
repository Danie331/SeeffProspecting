using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CostOfBatch
    {
        public decimal UnitCost { get; set; }
        public int NumberOfUnits { get; set; }
        public decimal TotalCost
        {
            get { return UnitCost * (decimal)NumberOfUnits; }
        }
    }
}