using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using System.Web;

namespace ProspectingProject
{
    public class ProspectingStaticData
    {
        public const string DracorePhoneEnquiryRequest = "DRACORE_PHONE";
        public const string DracoreEmailEnquiryRequest = "DRACORE_EMAIL";
        public const string TracePSEnquiryRequest = "TRACEPS";

        public static List<KeyValuePair<int, string>> ContactDetailTypes {get; set;}
        public static List<KeyValuePair<int, string>> PersonPropertyRelationshipTypes { get; set; }
        public static List<KeyValuePair<int, string>> CompanyPropertyRelationshipTypes { get; set; }
        public static List<KeyValuePair<int, string>> ContactPersonTitle { get; set; }
        public static List<KeyValuePair<int, string>> PersonPersonRelationshipTypes { get; set; }
        public static List<KeyValuePair<int, string>> ContactEmailTypes { get; set; }
        public static List<KeyValuePair<int, string>> ContactPhoneTypes { get; set; }
        public static List<KeyValuePair<int, string>> IntlDialingCodes { get; set; }
        public static List<KeyValuePair<int, string>> PersonCompanyRelationshipTypes { get; set; }
        public static List<KeyValuePair<int, string>> ActivityTypes { get; set; }
        public static List<KeyValuePair<int, string>> ActivityFollowupTypes { get; set; }

        public static IEnumerable<int> PhoneTypeIds { get; private set; }
        public static IEnumerable<int> EmailTypeIds { get; private set; }

        public static Func<ProspectingDataContext, prospecting_property, bool, IQueryable<ProspectingContactPerson>> PropertyContactsRetriever { get; private set; }
        public static Func<ProspectingDataContext, ProspectingContactPerson, IQueryable<ProspectingContactDetail>> PropertyContactPhoneNumberRetriever { get; private set; }
        public static Func<ProspectingDataContext, ProspectingContactPerson, IQueryable<ProspectingContactDetail>> PropertyContactEmailRetriever { get; private set; }
        public static Func<ProspectingDataContext, prospecting_property, bool, IQueryable<ProspectingContactPerson>> PropertyCompanyContactsRetriever { get; private set; }

        public static Guid? UserSessionGuid { get; set; }     

        static ProspectingStaticData()
        {
            LoadContactDetailTypes();
            LoadPersonPropertyRelationshipTypes();
            LoadContactPersonTitles();
            LoadPersonPersonRelationshipTypes();
            LoadIntlDialingCodes();
            LoadCompanyPropertyRelationshipTypes();
            LoadPersonCompanyRelationshipTypes();
            LoadActivityTypes();
            LoadActivityFollowupTypes();

            PhoneTypeIds = ProspectingStaticData.ContactPhoneTypes.Select(k => k.Key);
            EmailTypeIds = ProspectingStaticData.ContactEmailTypes.Select(k => k.Key);

            PropertyContactsRetriever = CompiledQuery.Compile((ProspectingDataContext ctx, prospecting_property property, bool loadOwnedProperties) => (from ppr in ctx.prospecting_person_property_relationships
                                                                                                                                                        join pcp in ctx.prospecting_contact_persons on ppr.contact_person_id equals pcp.contact_person_id
                                                                                                                                                        where ppr.prospecting_property_id == property.prospecting_property_id
                                                                                                                                                        select new ProspectingContactPerson
                                                                                                                                                        {
                                                                                                                                                            ContactPersonId = ppr.contact_person_id,
                                                                                                                                                            PersonPropertyRelationships = ProspectingDomain.LoadPersonPropertyRelationships(ctx, ppr.contact_person_id),// ppr.relationship_to_property,
                                                                                                                                                            PersonCompanyRelationshipType = null,
                                                                                                                                                            Firstname = pcp.firstname,
                                                                                                                                                            Surname = pcp.surname,
                                                                                                                                                            IdNumber = pcp.id_number,
                                                                                                                                                            Title = pcp.person_title,
                                                                                                                                                            Gender = pcp.person_gender,
                                                                                                                                                            Comments = pcp.comments_notes,
                                                                                                                                                            IsPOPIrestricted = pcp.is_popi_restricted,
                                                                                                                                                            PropertiesOwned = loadOwnedProperties ? ProspectingDomain.LoadPropertiesOwnedByThisContact(pcp.id_number, ctx) : null,

                                                                                                                                                            // Dracore fields
                                                                                                                                                            AgeGroup = pcp.age_group,
                                                                                                                                                            BureauAdverseIndicator = pcp.bureau_adverse_indicator,
                                                                                                                                                            Citizenship = pcp.citizenship,
                                                                                                                                                            DeceasedStatus = pcp.deceased_status,
                                                                                                                                                            Directorship = pcp.directorship,
                                                                                                                                                            Occupation = pcp.occupation,
                                                                                                                                                            Employer = pcp.employer,
                                                                                                                                                            PhysicalAddress = pcp.physical_address,
                                                                                                                                                            HomeOwnership = pcp.home_ownership,
                                                                                                                                                            MaritalStatus = pcp.marital_status,
                                                                                                                                                            Location = pcp.location,
                                                                                                                                                        }));
            PropertyCompanyContactsRetriever = CompiledQuery.Compile((ProspectingDataContext ctx, prospecting_property property, bool loadOwnedProperties) => (from pcpr in ctx.prospecting_company_property_relationships
                                                                                                                                                               join ppcr in ctx.prospecting_person_company_relationships on pcpr.contact_company_id equals ppcr.contact_company_id
                                                                                                                                                               join pcp in ctx.prospecting_contact_persons on ppcr.contact_person_id equals pcp.contact_person_id
                                                                            where pcpr.prospecting_property_id == property.prospecting_property_id
                                                                            select new ProspectingContactPerson
                                            {
                                                ContactPersonId = pcp.contact_person_id,
                                                PersonPropertyRelationships = new List<KeyValuePair<int, int>>(),
                                                PersonCompanyRelationshipType = ppcr.relationship_to_company,
                                                ContactCompanyId = ppcr.contact_company_id,
                                                Firstname = pcp.firstname,
                                                Surname = pcp.surname,
                                                IdNumber = pcp.id_number,
                                                Title = pcp.person_title,
                                                Gender = pcp.person_gender,
                                                Comments = pcp.comments_notes,
                                                IsPOPIrestricted = pcp.is_popi_restricted,
                                                PropertiesOwned = loadOwnedProperties ? ProspectingDomain.LoadPropertiesOwnedByThisContact(pcp.id_number, ctx) : null,

                                                // Dracore fields
                                                AgeGroup = pcp.age_group,
                                                BureauAdverseIndicator = pcp.bureau_adverse_indicator,
                                                Citizenship = pcp.citizenship,
                                                DeceasedStatus = pcp.deceased_status,
                                                Directorship = pcp.directorship,
                                                Occupation = pcp.occupation,
                                                Employer = pcp.employer,
                                                PhysicalAddress = pcp.physical_address,
                                                HomeOwnership = pcp.home_ownership,
                                                MaritalStatus = pcp.marital_status,
                                                Location = pcp.location
                                            }));

            PropertyContactPhoneNumberRetriever = CompiledQuery.Compile((ProspectingDataContext ctx, ProspectingContactPerson cp) => (from det in ctx.prospecting_contact_details
                                                                                                         where det.contact_person_id == cp.ContactPersonId
                                                                                                         && PhoneTypeIds.Contains(det.contact_detail_type)
                                                                                                         && !det.deleted
                                                                                                         select new ProspectingContactDetail
                                                                                                         {
                                                                                                             ItemId = det.prospecting_contact_detail_id.ToString(),
                                                                                                             ContactItemType = "PHONE",
                                                                                                             ItemContent = det.contact_detail,
                                                                                                             IsPrimary = det.is_primary_contact,
                                                                                                             ItemType = det.contact_detail_type,
                                                                                                             IsValid = true,
                                                                                                             IntDialingCode = det.intl_dialing_code_id,
                                                                                                             EleventhDigit = det.eleventh_digit
                                                                                                         }));

            PropertyContactEmailRetriever = CompiledQuery.Compile((ProspectingDataContext ctx, ProspectingContactPerson cp) => (from det in ctx.prospecting_contact_details
                                                                         where det.contact_person_id == cp.ContactPersonId
                                                                         && EmailTypeIds.Contains(det.contact_detail_type)
                                                                         && !det.deleted
                                                                         select new ProspectingContactDetail
                                                                         {
                                                                             ItemId = det.prospecting_contact_detail_id.ToString(),
                                                                             ContactItemType = "EMAIL",
                                                                             ItemContent = det.contact_detail,
                                                                             IsPrimary = det.is_primary_contact,
                                                                             ItemType = det.contact_detail_type,
                                                                             IsValid = true,
                                                                             IntDialingCode = det.intl_dialing_code_id,
                                                                             EleventhDigit = det.eleventh_digit
                                                                         }));
        }

        private static void LoadActivityTypes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                ActivityTypes = (from item in prospecting.activity_types 
                                 where item.is_system_type.HasValue && !item.is_system_type.Value && item.active
                                 select new KeyValuePair<int, string>(item.activity_type_id, item.activity_name)).ToList();
            }
        }

        private static void LoadActivityFollowupTypes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                ActivityFollowupTypes = (from item in prospecting.activity_followup_types
                                         where item.active
                                         select new KeyValuePair<int, string>(item.activity_followup_type_id, item.activity_name)).ToList();
            }
        }

        private static void LoadPersonCompanyRelationshipTypes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                PersonCompanyRelationshipTypes = (from item in prospecting.prospecting_person_company_relationship_types
                                                  select new KeyValuePair<int, string>(item.person_company_relationship_type_id, item.relationship_desc)).ToList();
            }
        }

        private static void LoadCompanyPropertyRelationshipTypes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                CompanyPropertyRelationshipTypes = (from item in prospecting.prospecting_company_property_relationship_types
                                                   select new KeyValuePair<int, string>(item.company_property_relationship_type_id, item.relationship_desc)).ToList();
            }
        }

        private static void LoadIntlDialingCodes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                IntlDialingCodes = (from idc in prospecting.prospecting_area_dialing_codes
                                        select new KeyValuePair<int, string>(idc.prospecting_area_dialing_code_id, idc.code_desc)).ToList();
            }
        }

        private static void LoadPersonPersonRelationshipTypes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                PersonPersonRelationshipTypes = (from item in prospecting.prospecting_person_person_relationship_types
                                                     select new KeyValuePair<int, string>(item.person_person_relationship_type_id, item.relationship_desc)).ToList();
            }
        }

        private static void LoadContactPersonTitles()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                ContactPersonTitle = (from item in prospecting.prospecting_person_titles
                                select new KeyValuePair<int, string>(item.prospecting_person_title_id, item.person_title)).ToList();
            }
        }

        private static void LoadPersonPropertyRelationshipTypes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                PersonPropertyRelationshipTypes = (from item in prospecting.prospecting_person_property_relationship_types
                                select new KeyValuePair<int, string>(item.person_property_relationship_type_id, item.relationship_desc)).ToList();
            }
        }

        private static  void LoadContactDetailTypes()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                ContactDetailTypes = (from item in prospecting.prospecting_contact_detail_types
                                select new KeyValuePair<int, string>(item.contact_detail_type_id, item.type_desc)).ToList();
            }

            ContactEmailTypes = ContactDetailTypes.Where(i => i.Value.Contains("email")).ToList();
            ContactPhoneTypes = ContactDetailTypes.Where(i => !i.Value.Contains("email")).ToList();
        }

    }
}