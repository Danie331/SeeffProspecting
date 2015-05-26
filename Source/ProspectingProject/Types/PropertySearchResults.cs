using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class PropertySearchResults
    {
        public PropertySearchResults() {
            MatchingPropertiesOutsideCurrentSuburb = new List<ProspectingProperty>();
            ProspectingProperties = new List<ProspectingProperty>();
        }

        public List<ProspectingProperty> MatchingPropertiesOutsideCurrentSuburb { get; set; }
        public List<ProspectingProperty> ProspectingProperties { get; set; }
    }
}