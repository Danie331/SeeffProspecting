using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class FlattenedPropertyRecord
    {
        public int prospecting_property_id { get; set; }
        public int lightstone_property_id { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public string property_address { get; set; }
        public string street_or_unit_no { get; set; }
        public int? seeff_area_id { get; set; }
        public string lightstone_id_or_ck_no { get; set; }
        public string lightstone_reg_date { get; set; }
        public int? erf_no { get; set; }
        public string comments { get; set; }
        public string ss_fh { get; set; }
        public string ss_name { get; set; }
        public string ss_number { get; set; }
        public string ss_id { get; set; }
        public string unit { get; set; }
        public string ss_door_number { get; set; }
        public decimal? last_purch_price { get; set; }
        public bool? prospected { get; set; }
        public string farm_name { get; set; }
        public int? portion_no { get; set; }
        public string lightstone_suburb { get; set; }
        public string ss_unique_identifier { get; set; }
        public string latest_reg_date { get; set; }
        public int? baths { get; set; }
        public string condition { get; set; }
        public int? beds { get; set; }
        public int? dwell_size { get; set; }
        public int? erf_size { get; set; }
        public int? garages { get; set; }
        public bool? pool { get; set; }
        public int? receptions { get; set; }
        public bool? staff_accomodation { get; set; }
        public int? studies { get; set; }
        public int? parking_bays { get; set; }
        public int? contact_person_id { get; set; }
        public int? relationship_to_property { get; set; }
        public int? relationship_to_company { get; set; }
        public int? contact_company_id { get; set; }
        public string firstname { get; set; }
        public string surname { get; set; }
        public string id_number { get; set; }
        public int? person_title { get; set; }
        public string person_gender { get; set; }
        public string comments_notes { get; set; }
        public bool? is_popi_restricted { get; set; }
        public bool? optout_emails { get; set; }
        public bool? optout_sms { get; set; }
        public bool? do_not_contact { get; set; }
        public int? email_contactability_status { get; set; }
        public string age_group { get; set; }
        public string bureau_adverse_indicator { get; set; }
        public string citizenship { get; set; }
        public string deceased_status { get; set; }
        public string directorship { get; set; }
        public string occupation { get; set; }
        public string employer { get; set; }
        public string physical_address { get; set; }
        public string home_ownership { get; set; }
        public string marital_status { get; set; }
        public string location { get; set; }
        public int? contact_detail_type { get; set; }
        public int? prospecting_contact_detail_id { get; set; }
        public string contact_detail { get; set; }
        public bool? is_primary_contact { get; set; }
        public int? intl_dialing_code_id { get; set; }
        public int? dialing_code_id { get; set; }
        public int? eleventh_digit { get; set; }
        public bool? deleted { get; set; }
        public bool? is_short_term_rental { get; set; }
        public bool? is_long_term_rental { get; set; }
        public bool? is_commercial { get; set; }
        public bool? is_agricultural { get; set; }
        public bool? is_investment { get; set; }

        public bool? has_cell { get; set; }
        public bool? has_primary_cell { get; set; }

        public bool? has_email { get; set; }
        public bool? has_primary_email { get; set; }

        public bool? has_landline { get; set; }
        public bool? has_primary_landline { get; set; }

        public int? property_listing_id { get; set; }
    }
}