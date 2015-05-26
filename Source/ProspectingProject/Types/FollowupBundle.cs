using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class FollowupBundle
    {
        public List<FollowUpActivity> Followups {get; set;}
        public int TotalCount { get; set; }
    }
}