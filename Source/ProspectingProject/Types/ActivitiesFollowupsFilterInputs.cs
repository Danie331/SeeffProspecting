using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ActivitiesFollowupsFilterInputs
    {
        public bool ShowingActivityTypes { get; set; }

        public List<int> ActivityTypes { get; set; }
        public List<int> ActivityFollowupTypes { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? AllocatedTo { get; set; }
        public DateTime? FollowupDateFrom { get; set; }
        public DateTime? FollowupDateTo { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public int CurrentSuburbID { get; set; }
    }
}