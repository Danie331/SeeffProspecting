using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ReferralResponseObject
    {
        public ReferralResponseObject()
        {
            InstanceValidationErrors = new List<string>();
        }

        // Web service/API input members
        public string department { get; set; }
        public int license_id_from { get; set; }
        public int license_id_to { get; set; }
        public int property_id { get; set; }
        public string property_desc { get; set; }
        public decimal property_lat { get; set; }
        public decimal property_long { get; set; }
        public string user_guid { get; set; }
        public string smart_pass_title { get; set; }
        public string smart_pass_name { get; set; }
        public string smart_pass_surname { get; set; }
        public string smart_pass_id_no { get; set; }
        public string smart_pass_company { get; set; }
        public string smart_pass_contact_type { get; set; }
        public string smart_pass_country_code { get; set; }
        public string smart_pass_contact_no { get; set; }
        public string smart_pass_email_address { get; set; }
        public string smart_pass_comment { get; set; }

        // Prospecting members
        public List<string> InstanceValidationErrors { get; set; }
        public int ContactPersonID { get; set; }

        // Referral response from web service members
        public int pSmart_pass_id { get; set; }
        public int pErrorNo { get; set; }
        public string pErrorMessage { get; set; }
    }
}