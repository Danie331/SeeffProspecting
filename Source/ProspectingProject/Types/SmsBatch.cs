using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class SmsBatch
    {
        public List<ProspectingContactPerson> Recipients { get; set; }
        public string SmsBodyRaw { get; set; }
        public bool TargetCurrentSuburb { get; set; }
        public bool TargetAllMySuburbs { get; set; }
        public int? CurrentSuburbId { get; set; }
        public string NameOfBatch { get; set; }
        public int? TemplateActivityTypeId { get; set; }
        public List<int> UserSuburbIds { get; set; }
    }
}