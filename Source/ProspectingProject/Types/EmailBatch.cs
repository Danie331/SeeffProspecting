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
        public bool TargetCurrentSuburb { get; set; }
        public bool TargetAllMySuburbs { get; set; }
        public int? CurrentSuburbId { get; set; }
        public string NameOfBatch { get; set; }
        public int? TemplateActivityTypeId { get; set; }
        public List<int> UserSuburbIds { get; set; }
        public List<EmailAttachment> Attachments { get; set; }
        public bool IncludeUnsubscribeLink { get; set; } = true;
    }
}