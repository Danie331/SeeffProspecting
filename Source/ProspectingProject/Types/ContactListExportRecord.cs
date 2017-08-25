using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ContactListExportRecord
    {
        public int contact_person_id { get; set; }
        public string person_title { get; set; }
        public string firstname { get; set; }
        public string surname { get; set; }
        public string id_number { get; set; }
    }
}