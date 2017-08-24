using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class MultiContactListSelection
    {
        public List<ListSelected> SelectedLists { get; set; }
        public List<int> VisibleProperties { get; set; }
        public List<ProspectingContactPerson> TargetContactsList { get; set; }
    }
}