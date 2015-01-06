using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class NewProspectingLocation
    {
        public List<LightstonePropertyMatch> PropertyMatches { get; set; }
        public bool IsSectionalScheme { get; set; }
        public string SectionalScheme { get; set; }

        public bool SSExists { get; set; }

        public string ErrorMessage { get; set; }
    }
}