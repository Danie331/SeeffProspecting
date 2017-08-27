using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ListExportSelection
    {
        public int ListId { get; set; }
        public string OutputFormat { get; set; }
        public List<string> Columns { get; set; }
        public bool OmitRecordIfEmptyField { get; set; }
        public bool UsePrimaryContactDetailOnly { get; set; }
        public bool ExcludeRecordIfPopiChecked { get; set; }
        public bool ExcludeRecordIfEmailOptOut { get; set; }
        public bool ExcludeRecordIfSmsOptOut { get; set; }
        public bool ExcludeRecordIfDoNotContactChecked { get; set; }
        public bool ExcludeDuplicateContacts { get; set; }
        public bool ExcludeDuplicateEmailAddress { get; set; }
    }
}