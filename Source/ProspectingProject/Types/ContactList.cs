using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactList
    {
        public string CreatedByUser { get; internal set; }
        public bool CurrentContactIsMember { get; set; }
        public int id { get; set; }
        public int? ListId { get; set; }
        public string ListName { get; set; }
        public KeyValuePair<int, string> ListType { get; set; }
        public string ListTypeDescription { get;  set; }
        public int MemberCount { get;  set; }
        public string UpdatedDate { get; internal set; }
    }
}