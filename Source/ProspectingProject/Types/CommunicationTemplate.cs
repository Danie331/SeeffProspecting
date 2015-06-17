using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class CommunicationTemplate
    {
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
        public string ActivityName { get; set; }
        public int? TemplateActivityTypeId { get; set; }
    }
}