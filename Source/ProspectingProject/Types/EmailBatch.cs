using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class EmailBatch
    {
        public List<ProspectingContactPerson> Recipients { get; set; }
        public string EmailBodyHTMLRaw { get; set; }
        public string EmailSubjectRaw { get; set; }
        public bool? ContactsInCurrentSuburb { get; set; }
        public bool? ContactsInAllMySuburbs { get; set; }
        public int? CurrentSuburbId { get; set; }
        public string NameOfBatch { get; set; }
        public int? TemplateActivityTypeId { get; set; }
    }
}