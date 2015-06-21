using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ProspectingContactDetail
    {
        /// <summary>
        ///  ItemId is recorded as a string because new records in the javascript are flagged with new guids.
        ///  The guid must be replaced after the item is saved with its corresponding DB id.
        /// </summary>
        public string ItemId { get; set; }
        public string ContactItemType { get; set; }
        public int? IntDialingCode { get; set; }
        public int? IntDialingCodePrefix { get; set; }

        public int? ItemType { get; set; }
        public string ItemContent { get; set; }
        public int? EleventhDigit { get; set; }

        public bool? IsPrimary { get; set; }

        public bool IsValid { get ; set; }
    }
}