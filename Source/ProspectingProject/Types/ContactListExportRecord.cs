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
        public bool is_popi_restricted { get; set; }
        public bool optout_emails { get; set; }
        public bool optout_sms { get; set; }
        public bool do_not_contact { get; set; }
        public string street_or_unit_no { get; set; }
        public string unit { get; set; }
        public string property_address { get; set; }
        public string ss_name { get; set; }
        public string ss_door_number { get; set; }
        public int? contact_detail_type { get; set; }
        public string contact_detail { get; set; }
        public int? eleventh_digit { get; set; }
        public string code_desc { get; set; }
        public bool? is_primary_contact { get; set; }
        public string ss_fh { get; set; }
        public int prospecting_property_id { get; set; }
    }
}