using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactList
    {
        public int id { get; set; }
        public int? ListId { get; set; }
        public string ListName { get; set; }
        public KeyValuePair<int, string> ListType { get; set; }
        public string ListTypeDescription { get; internal set; }
        public int MemberCount { get; internal set; }
        public List<ProspectingContactPerson> Members { get; set; }
    }
}