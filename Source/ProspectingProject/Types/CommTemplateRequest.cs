using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CommTemplateRequest
    {
        public string TemplateContent { get; set; }
        public string TemplateName { get; set; }
        public string CommunicationType { get; set; } // EMAIL or SMS
        public bool IsSystemTemplate { get; set; }
        public int? ActivityTypeId { get; set; }
        public bool? IsFromUrl { get; set; }
        public string URL { get; set; }
    }
}