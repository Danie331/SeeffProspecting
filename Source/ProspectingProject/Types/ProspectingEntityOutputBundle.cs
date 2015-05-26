using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ProspectingEntityOutputBundle
    {
        public ProspectingEntityOutputBundle()
        {
            SectionalSchemes = new Dictionary<string, List<ProspectingProperty>>();
            FhProperties = new List<ProspectingProperty>();
        }

        public string CreationErrorMsg { get; set; }
        public Dictionary<string, List<ProspectingProperty>> SectionalSchemes { get; set; }
        public List<ProspectingProperty> FhProperties { get; set; }

        // Serves 2 purposes: provides a random lat/long for us to center on (random amongst one of the selected prospects)
        // And in the case where only one unit was created, gives us an indication of where to direct the user once created
        public ProspectingProperty TargetProspect { get; set; }

        public int? SeeffAreaId { get; set; }
    }
}