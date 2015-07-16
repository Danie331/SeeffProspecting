using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class PropertyValuation
    {
        public int ValuationRecordId { get; set; }
        public int ProspectingPropertyId { get; set; }
        public decimal Value { get; set; }
        public DateTime ValuationDate { get; set; }
        public bool IsCurrentValue { get; set; }
        public bool CreateActivity { get; set; }
        public Guid CreatedByGuid { get; set; }
        public string CreatedByUsername { get; set; }
    }
}