using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class NewProspectingEntity
    {
        public List<LightstonePropertyMatch> PropertyMatches { get; set; }
        public bool IsSectionalScheme { get; set; }
        public string SectionalScheme { get; set; }
        public bool Exists { get; set; }
        public string ErrorMessage { get; set; }

        public string SS_ID { get; set; }
        public string SS_UNIQUE_IDENTIFIER { get; set; }

        public int? SeeffAreaId { get; set; }

        // Farms
        public bool IsFarm { get; set; }
    }
}