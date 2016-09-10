using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ActivitiesFollowupsFilterResponse
    {
        public List<ActivitiesFollowupsPivotRow> OutputRows { get; set; }

        public List<int> FilteredProperties { get; set; }
    }
}