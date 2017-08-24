using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactListSelection
    {
        public int ContactPersonId { get; set; }
        public List<ListSelected> ListAllocation { get; set; }
        public int TargetPropertyId { get; set; }
    }
}