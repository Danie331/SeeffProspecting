using Newtonsoft.Json;
using OfficeOpenXml;
using ProspectingProject.Services;
using ProspectingProject.Services.SeeffSpatial;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
// first git push!
namespace ProspectingProject
{
    /// <summary>
    /// This class must contain all business logic specific to Prospecting. 
    /// It is the only class that should handle the .dbml data context objects directly.
    /// Do not inherit off this type.
    /// </summary>
    public sealed partial class ProspectingCore
    {
        public static List<ProspectingProperty> CreateProspectableProperties(int seeffAreaId)
        {
            List<ProspectingProperty> prospectables = new List<ProspectingProperty>();
            // At this point we only have lightstone listings; some of which may already have been prospected.
            using (var prospectingDB = new ProspectingDataContext())
            {
                var prospects = from p in prospectingDB.prospecting_properties
                                where p.seeff_area_id == seeffAreaId
                                select p;
                foreach (var prospectable in prospects)
                {
                    //ProspectingProperty prop = LoadProspectingProperty(prospectingDB, prospectable, false);
                    ProspectingProperty prop = LoadProspectingProperty(prospectable, false);
                    prospectables.Add(prop);
                }

            }

            return prospectables;
        }

        public static List<GeoLocation> LoadPolyCoords(string polyWKT)
        {
            //using (var prospectingDB = new ProspectingDataContext())
            //{
            //    return (from entry in prospectingDB.prospecting_kml_areas
            //            where entry.prospecting_area_id == suburbId && entry.area_type == resCommAgri.ToCharArray()[0]
            //            orderby entry.seq ascending
            //            select new GeoLocation
            //            {
            //                Lat = entry.latitude,
            //                Lng = entry.longitude
            //            }).ToList();
            //}
            List<GeoLocation> polygon = new List<GeoLocation>();
            polyWKT = polyWKT.Replace("POLYGON ((", "").Replace("))", "");
            var coordPairs = polyWKT.Split(new[] { ',' });
            foreach (var item in coordPairs)
            {
                string[] coordPair = item.Trim().Split(new[] { ' ' });
                string lat = coordPair[1];
                string lng = coordPair[0];
                GeoLocation loc = new GeoLocation
                {
                    Lat = Decimal.Parse(lat),
                    Lng = Decimal.Parse(lng)
                };
                polygon.Add(loc);
            }
            return polygon;
        }

        //public static string GetAreaName(int suburbId)
        //{
        //    using (var prospectingDB = new ProspectingDataContext())
        //    {
        //        return prospectingDB.prospecting_areas.First(n => n.prospecting_area_id == suburbId).area_name;
        //    }
        //}      

        private static ProspectingProperty CreateProspectingProperty(ProspectingDataContext prospectingContext, prospecting_property prospectingRecord, bool loadContactsOnly, bool loadContactsAndCompanies, bool loadOwnedProperties, bool loadActivities)
        {
            var latLng = new GeoLocation { Lat = prospectingRecord.latitude.Value, Lng = prospectingRecord.longitude.Value };
            ProspectingProperty prop = new ProspectingProperty
            {
                ProspectingPropertyId = prospectingRecord.prospecting_property_id,
                LightstonePropertyId = prospectingRecord.lightstone_property_id,
                LatLng = latLng,
                PropertyAddress = prospectingRecord.property_address,
                StreetOrUnitNo = prospectingRecord.street_or_unit_no,
                SeeffAreaId = prospectingRecord.seeff_area_id.HasValue ? prospectingRecord.seeff_area_id.Value : (int?)null,
                LightstoneIDOrCKNo = prospectingRecord.lightstone_id_or_ck_no,
                LightstoneRegDate = prospectingRecord.lightstone_reg_date,
                ErfNo = prospectingRecord.erf_no,
                Comments = prospectingRecord.comments,
                //SS_FH = prospectingRecord.ss_fh == "SS" ? "SS" : "FH", // default to FH for backward compat.
                SSName = prospectingRecord.ss_name,
                SSNumber = prospectingRecord.ss_number,
                SS_ID = prospectingRecord.ss_id,
                Unit = prospectingRecord.unit,
                SSDoorNo = prospectingRecord.ss_door_number,
                LastPurchPrice = prospectingRecord.last_purch_price,
                Prospected = Convert.ToBoolean(prospectingRecord.prospected),
                FarmName = prospectingRecord.farm_name,
                Portion = prospectingRecord.portion_no != null ? prospectingRecord.portion_no.ToString() : "n/a",
                LightstoneSuburb = prospectingRecord.lightstone_suburb,
                SS_UNIQUE_IDENTIFIER = prospectingRecord.ss_unique_identifier,
                LatestRegDateForUpdate = prospectingRecord.latest_reg_date,

                Baths = prospectingRecord.baths,
                Condition = prospectingRecord.condition,
                Beds = prospectingRecord.beds,
                DwellingSize = prospectingRecord.dwell_size,
                ErfSize = prospectingRecord.erf_size,
                Garages = prospectingRecord.garages,
                Pool = prospectingRecord.pool,
                Receptions = prospectingRecord.receptions,
                StaffAccomodation = prospectingRecord.staff_accomodation,
                Studies = prospectingRecord.studies,
                ParkingBays = prospectingRecord.parking_bays,

                IsShortTermRental = prospectingRecord.is_short_term_rental,
                IsLongTermRental = prospectingRecord.is_long_term_rental,
                IsCommercial = prospectingRecord.is_commercial,
                IsAgricultural = prospectingRecord.is_agricultural,
                IsInvestment = prospectingRecord.is_investment,

                HasContactWithCell = prospectingRecord.has_cell,
                HasContactWithPrimaryCell = prospectingRecord.has_primary_cell,

                HasContactWithEmail = prospectingRecord.has_email,
                HasContactWithPrimaryEmail = prospectingRecord.has_primary_email,

                HasContactWithLandline = prospectingRecord.has_landline,
                HasContactWithPrimaryLandline = prospectingRecord.has_primary_landline,

                PropertyListingId = prospectingRecord.property_listing_id
            };

            switch (prospectingRecord.ss_fh)
            {
                case "SS": prop.SS_FH = "SS"; break;
                case "FH": prop.SS_FH = "FH"; break;
                case "FS": prop.SS_FH = "FS"; break;
                case "FRM": prop.SS_FH = "FRM"; break;
                default: prop.SS_FH = "FH"; break;
            }

            //prop.HasMandate = PropertyHasActiveMandate(prospectingContext, prospectingRecord);

            if (loadContactsOnly)
            {
                prop.Contacts = LoadContacts(prospectingContext, prospectingRecord, loadOwnedProperties);
            }

            if (loadContactsAndCompanies)
            {
                prop.Contacts = LoadContacts(prospectingContext, prospectingRecord, loadOwnedProperties);
                prop.ContactCompanies = LoadContactCompanies(prospectingContext, prospectingRecord);
            }

            if (loadActivities)
            {
                prop.ActivityBundle = LoadProspectingActivities(prospectingContext, prop.LightstonePropertyId);
            }

            return prop;
        }

        //private static bool PropertyHasActiveMandate(ProspectingDataContext prospectingContext, prospecting_property prospectingRecord)
        //{
        //    bool result = prospectingRecord.property_mandates.SelectMany(i => i.property_mandate_agencies).Any(i => !i.deleted);
        //    return result;
        //}

        //public static MandateSet LoadCurrentMandateSet(int lightstonePropertyId)
        //{
        //    MandateSet results = new MandateSet();
        //    try
        //    {
        //        using (var prospecting = new ProspectingDataContext())
        //        using (var ls_base = new ls_baseEntities())
        //        {
        //            var propertyMandates = prospecting.property_mandates.Where(pm => pm.lightstone_property_id == lightstonePropertyId);
        //            var propertyMandatesIDs = propertyMandates.Select(d => d.property_mandate_id);
        //                var pmas = prospecting.property_mandate_agencies.Where(pm => propertyMandatesIDs.Contains(pm.property_mandate_id) && !pm.deleted);
        //                if (pmas.Any()) {
        //                    foreach (var item in pmas.OrderByDescending(c => c.created_date))
        //                    {
        //                        var followup = item.activity_log;
        //                        var agency = ls_base.agency.FirstOrDefault(a => a.agency_id == item.agency_id)?.agency_name;
        //                        PropertyMandateAgency pma = new PropertyMandateAgency
        //                        {
        //                            PropertyMandateAgencyID = item.property_mandate_agency_id,
        //                            Agency = agency,
        //                            MandateType = item.property_mandate_type.type_description,
        //                            Status = item.property_mandate_status.status_description,
        //                            ListingPrice = FormatSalePrice(item.listing_price),
        //                            ExpiryDate = item.mandate_expiry_date?.ToShortDateString(),
        //                            FollowupDate = followup != null ? followup.followup_date?.ToShortDateString() : "n/a",
        //                            FollowupType = followup != null ? followup.activity_followup_type.activity_name : "n/a",
        //                            Agents = agency == "Seeff" ? item.seeff_agents : item.agents
        //                        };
        //                        results.ListOfMandates.Add(pma);
        //                    }
        //                }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        results.ErrorMessage = ex.Message;
        //    }

        //    return results;
        //}

        private static ActivityBundle LoadProspectingActivities(ProspectingDataContext prospectingContext, int? lightstonePropertyId)
        {
            ActivityBundle activityBundle = new ActivityBundle();
            UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
            activityBundle.BusinessUnitUsers = user.BusinessUnitUsers;
            activityBundle.ActivityTypes = ProspectingLookupData.ActivityTypes;
            activityBundle.ActivityFollowupTypes = ProspectingLookupData.ActivityFollowupTypes;

            IQueryable<activity_log> activities = null;
            if (lightstonePropertyId.HasValue)
            {
                activities = prospectingContext.activity_logs.Where(a => a.lightstone_property_id == lightstonePropertyId);
            }
            else
            {
                activities = prospectingContext.activity_logs.Where(a => a.created_by == user.UserGuid).OrderByDescending(d => d.created_date).Take(50);
            }
            foreach (var activity in activities)
            {
                var createdByUser = user.BusinessUnitUsers.FirstOrDefault(b => b.UserGuid == activity.created_by);
                string createdBy = createdByUser != null && createdByUser.UserGuid != Guid.Empty ? createdByUser.UserName + " " + createdByUser.UserSurname : "System";
                var allocatedToUser = user.BusinessUnitUsers.FirstOrDefault(b => b.UserGuid == activity.allocated_to);
                string allocatedToName = allocatedToUser != null ? allocatedToUser.UserName + " " + allocatedToUser.UserSurname : "n/a";
                string relatedToContactPersonName = null;
                if (activity.contact_person_id != null)
                {
                    var relatedToContactPerson = prospectingContext.prospecting_contact_persons.First(pp => pp.contact_person_id == activity.contact_person_id);
                    relatedToContactPersonName = relatedToContactPerson.firstname + " " + relatedToContactPerson.surname;
                }
                var activityFollowupType = prospectingContext.activity_followup_types.FirstOrDefault(t => t.activity_followup_type_id == activity.activity_followup_type_id);
                string activityFollowupTypeName = "";
                if (activityFollowupType != null)
                {
                    activityFollowupTypeName = activityFollowupType.activity_name;
                }
                var propertyRecord = prospectingContext.prospecting_properties.First(pp => pp.lightstone_property_id == activity.lightstone_property_id);
                ProspectingActivity act = new ProspectingActivity
                {
                    ActivityLogId = activity.activity_log_id,
                    LightstonePropertyId = activity.lightstone_property_id,
                    FollowUpDate = activity.followup_date,
                    AllocatedTo = activity.allocated_to,
                    AllocatedToName = allocatedToName,
                    ActivityTypeId = activity.activity_type_id,
                    ActivityType = prospectingContext.activity_types.First(t => t.activity_type_id == activity.activity_type_id).activity_name,
                    Comment = activity.comment,
                    CreatedByGuid = activity.created_by,
                    CreatedBy = createdBy,
                    CreatedDate = activity.created_date,
                    ContactPersonId = activity.contact_person_id,
                    DeletedBy = activity.deleted_by,
                    DeletedDate = activity.delete_date,
                    Deleted = activity.deleted,
                    ParentActivityId = activity.parent_activity_id,
                    SmsTemplateId = activity.sms_template_id,
                    SmsSent = activity.sms_sent,
                    EmailTemplateId = activity.email_template_id,
                    EmailSent = activity.email_sent,
                    PhoneCall = activity.phone_call,
                    Visit = activity.visit,
                    RelatedToContactPersonName = relatedToContactPersonName,
                    RelatedToContactPersonId = activity.contact_person_id,
                    ActivityFollowupTypeId = activity.activity_followup_type_id,
                    ActivityFollowupTypeName = activityFollowupTypeName,
                    SeeffAreaId = propertyRecord.seeff_area_id,
                    PropertyType = propertyRecord.ss_fh,
                    PropertyAddress = GetFormattedAddress(propertyRecord.lightstone_property_id)
                };
                activityBundle.Activities.Add(act);
            }

            activityBundle.Activities = activityBundle.Activities.OrderByDescending(o => o.CreatedDate).ToList();

            return activityBundle;
        }

        private static ProspectingProperty LoadProspectingProperty(prospecting_property prospectingRecord, bool loadOwnedProperties)
        {
            var latLng = new GeoLocation { Lat = prospectingRecord.latitude.Value, Lng = prospectingRecord.longitude.Value };
            ProspectingProperty prop = new ProspectingProperty
            {
                ProspectingPropertyId = prospectingRecord.prospecting_property_id,
                LightstonePropertyId = prospectingRecord.lightstone_property_id,
                LatLng = latLng,
                PropertyAddress = prospectingRecord.property_address,
                StreetOrUnitNo = prospectingRecord.street_or_unit_no,
                SeeffAreaId = prospectingRecord.seeff_area_id.HasValue ? prospectingRecord.seeff_area_id.Value : (int?)null,
                LightstoneIDOrCKNo = prospectingRecord.lightstone_id_or_ck_no,
                LightstoneRegDate = prospectingRecord.lightstone_reg_date,
                ErfNo = prospectingRecord.erf_no,
                Comments = prospectingRecord.comments,
                SS_FH = !string.IsNullOrEmpty(prospectingRecord.ss_fh) ? prospectingRecord.ss_fh : "FH",
                SSName = prospectingRecord.ss_name,
                SSNumber = prospectingRecord.ss_number,
                SS_ID = prospectingRecord.ss_id,
                Unit = prospectingRecord.unit,
                SSDoorNo = prospectingRecord.ss_door_number,
                LastPurchPrice = prospectingRecord.last_purch_price,
                Prospected = Convert.ToBoolean(prospectingRecord.prospected),
                FarmName = prospectingRecord.farm_name,
                Portion = prospectingRecord.portion_no.HasValue ? prospectingRecord.portion_no.ToString() : null,
                LightstoneSuburb = prospectingRecord.lightstone_suburb,
                SS_UNIQUE_IDENTIFIER = prospectingRecord.ss_unique_identifier,
                LatestRegDateForUpdate = prospectingRecord.latest_reg_date,

                Baths = prospectingRecord.baths,
                Condition = prospectingRecord.condition,
                Beds = prospectingRecord.beds,
                DwellingSize = prospectingRecord.dwell_size,
                ErfSize = prospectingRecord.erf_size,
                Garages = prospectingRecord.garages,
                Pool = prospectingRecord.pool,
                Receptions = prospectingRecord.receptions,
                StaffAccomodation = prospectingRecord.staff_accomodation,
                Studies = prospectingRecord.studies,
                ParkingBays = prospectingRecord.parking_bays,

                IsShortTermRental = prospectingRecord.is_short_term_rental,
                IsLongTermRental = prospectingRecord.is_long_term_rental,
                IsCommercial = prospectingRecord.is_commercial,
                IsAgricultural = prospectingRecord.is_agricultural,
                IsInvestment = prospectingRecord.is_investment,

                HasContactWithCell = prospectingRecord.has_cell,
                HasContactWithPrimaryCell = prospectingRecord.has_primary_cell,

                HasContactWithEmail = prospectingRecord.has_email,
                HasContactWithPrimaryEmail = prospectingRecord.has_primary_email,

                HasContactWithLandline = prospectingRecord.has_landline,
                HasContactWithPrimaryLandline = prospectingRecord.has_primary_landline,

                PropertyListingId = prospectingRecord.property_listing_id
            };

            return prop;
        }


        private static List<ProspectingContactCompany> LoadContactCompanies(ProspectingDataContext prospectingContext, prospecting_property prospectingRecord)
        {
            var companies = (from pc in prospectingContext.prospecting_contact_companies
                             join rel in prospectingContext.prospecting_company_property_relationships on pc.contact_company_id equals rel.contact_company_id
                             where prospectingRecord.prospecting_property_id == rel.prospecting_property_id
                             select new ProspectingContactCompany
                             {
                                 CKNumber = pc.CK_number,
                                 CompanyName = pc.company_name,
                                 CompanyType = pc.company_type,
                                 ContactCompanyId = pc.contact_company_id,
                                 ContactEntityType = ContactEntityType.JuristicEntity,
                                 ProspectingProperties = null, // For now.
                                 CompanyContacts = LoadCompanyContacts(pc.CK_number)
                             }).ToList();
            return companies;
        }

        public static string LoadSuburbInfoForUser(string suburbsWithPermissions)
        {
            if (string.IsNullOrWhiteSpace(suburbsWithPermissions))
            {
                return string.Empty;
            }

            Regex regexSplitter = new Regex(@"\[(.*?)\]");
            var matches = regexSplitter.Matches(suburbsWithPermissions);
            if (matches.Count > 0)
            {
                string areasClause = "";

                foreach (Match match in matches)
                {
                    string prospectingAreas = match.Value;
                    prospectingAreas = prospectingAreas.Replace(" ", "").Replace("],[", "|").Replace("[", "").Replace("]", "");
                    prospectingAreas = prospectingAreas.Replace("[", "");
                    prospectingAreas = prospectingAreas.Replace("]", "");
                    prospectingAreas = prospectingAreas.Replace(",1", "");
                    prospectingAreas = prospectingAreas.Replace(",0", "");
                    if (areasClause == "") { areasClause = prospectingAreas; }
                    else
                    { areasClause += "," + prospectingAreas; };
                };

                var suburbsInfo = GetAreaData(areasClause);
                HttpContext.Current.Session["user_suburbs"] = suburbsInfo;
                return new JavaScriptSerializer().Serialize(suburbsInfo);
            }

            return string.Empty;
        }

        public static List<UserSuburb> GetAreaData(string areaIdList)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                prospecting.CommandTimeout = 60;
                var areaIds = areaIdList.Split(new[] { ',' }).Select(id => Convert.ToInt32(id));

                //var targets = from pp in prospecting.prospecting_properties
                //                  where areaIds.Contains(pp.seeff_area_id.Value)
                //                  group pp by pp.seeff_area_id into gr
                //                  select gr;

                var spatialReader = new SpatialServiceReader();
                var spatialList = spatialReader.SuburbsListOnly();
                var suburbsList = (from sub in spatialList
                                   select new ProspectingSuburb
                                   {
                                       LocationID = sub.SeeffAreaID,
                                       LocationName = sub.AreaName,
                                       IsDeleted = sub.IsDeleted
                                   }).Distinct().ToList();

                List<UserSuburb> userSuburbs = new List<UserSuburb>();
                foreach (var areaId in areaIds)
                {
                    var targetProps = prospecting.prospecting_properties.Where(sub => sub.seeff_area_id == areaId);

                    var suburbInSuburbsList = suburbsList.FirstOrDefault(sub => sub.LocationID == areaId);
                    if (suburbInSuburbsList != null && !suburbInSuburbsList.IsDeleted)
                    {
                        userSuburbs.Add(new UserSuburb
                        {
                            PropertiesRequireAttention = targetProps.Count(p => p.latest_reg_date != null),
                            SuburbId = areaId,
                            SuburbName = suburbInSuburbsList.LocationName,
                            TotalFullyProspected = targetProps.Count()
                        });
                    }
                }

                //foreach (var area in targets)
                //{
                //    var firstProperty = area.First();
                //    var suburbInSuburbsList = ProspectingLookupData.SuburbsListOnly.FirstOrDefault(sub => sub.LocationID == firstProperty.seeff_area_id);
                //    if (suburbInSuburbsList != null)
                //    {
                //        var userSuburb = new UserSuburb
                //        {
                //            SuburbId = firstProperty.seeff_area_id.Value,
                //            SuburbName = suburbInSuburbsList.LocationName,
                //            TotalFullyProspected = area.Count(),
                //            PropertiesRequireAttention = area.Where(p => p.latest_reg_date != null).Count()
                //        };
                //        userSuburbs.Add(userSuburb);
                //    }
                //}

                return userSuburbs.Distinct().OrderBy(a => a.SuburbName).ToList();

                //return (from n in prospecting.prospecting_areas
                //        join pp in prospecting.prospecting_properties on n.prospecting_area_id equals pp.seeff_area_id
                //        into sr
                //        from x in sr.DefaultIfEmpty()
                //        where areaIds.Contains(n.prospecting_area_id)
                //        select new UserSuburb
                //        {
                //            SuburbId = n.prospecting_area_id,
                //            SuburbName = n.area_name,
                //            TotalFullyProspected = sr.Select(p => p.prospecting_property_id).Count(),
                //            PropertiesRequireAttention = sr.Where(p => p.latest_reg_date != null).Count()
                //        }).Distinct().OrderBy(a => a.SuburbName).ToList();
            }
        }

        public static string SerialiseStaticProspectingData()
        {
            var a = new
            {
                ContactDetailTypes = ProspectingLookupData.ContactDetailTypes,
                PersonPropertyRelationshipTypes = ProspectingLookupData.PersonPropertyRelationshipTypes,
                ContactPersonTitle = ProspectingLookupData.ContactPersonTitle,
                PersonPersonRelationshipTypes = ProspectingLookupData.PersonPersonRelationshipTypes,
                ContactEmailTypes = ProspectingLookupData.ContactEmailTypes,
                ContactPhoneTypes = ProspectingLookupData.ContactPhoneTypes,
                IntlDialingCodes = ProspectingLookupData.IntlDialingCodes,
                PersonCompanyRelationshipTypes = ProspectingLookupData.PersonCompanyRelationshipTypes,
                SMSCost = ProspectingLookupData.SMSCost,
                SMSLength = ProspectingLookupData.SMSLength
            };

            return ProspectingCore.SerializeToJsonWithDefaults(a);
        }


        public static List<ProspectingContactPerson> LoadContacts(ProspectingDataContext prospecting, prospecting_property property, bool loadOwnedProperties)
        {
            List<ProspectingContactPerson> contactsAssociatedWithProperties = ProspectingLookupData.PropertyContactsRetriever(prospecting, property, loadOwnedProperties).ToList();
            List<ProspectingContactPerson> contactsAssociatedWithCompanies = ProspectingLookupData.PropertyCompanyContactsRetriever(prospecting, property, loadOwnedProperties).ToList();

            foreach (var cp in contactsAssociatedWithProperties)
            {
                cp.PhoneNumbers = ProspectingLookupData.PropertyContactPhoneNumberRetriever(prospecting, cp).ToList();
                cp.EmailAddresses = ProspectingLookupData.PropertyContactEmailRetriever(prospecting, cp).ToList();
            }


            foreach (var cp in contactsAssociatedWithCompanies)
            {
                cp.PhoneNumbers = ProspectingLookupData.PropertyContactPhoneNumberRetriever(prospecting, cp).ToList();
                cp.EmailAddresses = ProspectingLookupData.PropertyContactEmailRetriever(prospecting, cp).ToList();
            }

            // Combine 2 lists and return 
            contactsAssociatedWithProperties.AddRange(contactsAssociatedWithCompanies);
            return contactsAssociatedWithProperties;
        }

        private static ProspectingContactPerson LoadContactForFollowup(ProspectingDataContext prospecting, int? contactPersonId)
        {
            if (contactPersonId == null) return null;
            var pcp = prospecting.prospecting_contact_persons.FirstOrDefault(p => p.contact_person_id == contactPersonId);
            if (pcp != null)
            {
                var contactPerson = new ProspectingContactPerson
                {
                    ContactPersonId = pcp.contact_person_id,
                    PersonCompanyRelationshipType = null,
                    Firstname = pcp.firstname,
                    Surname = pcp.surname,
                    IdNumber = pcp.id_number,
                    Title = pcp.person_title,
                    Gender = pcp.person_gender,
                    Comments = pcp.comments_notes,
                    IsPOPIrestricted = pcp.is_popi_restricted,
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
                    EmailOptout = pcp.optout_emails,
                    SMSOptout = pcp.optout_sms,
                    DoNotContact = pcp.do_not_contact,
                    EmailContactabilityStatus = pcp.email_contactability_status
                };

                contactPerson.PhoneNumbers = ProspectingLookupData.PropertyContactPhoneNumberRetriever(prospecting, contactPerson).ToList();
                contactPerson.EmailAddresses = ProspectingLookupData.PropertyContactEmailRetriever(prospecting, contactPerson).ToList();

                return contactPerson;
            }

            return null;
        }

        public static List<KeyValuePair<int, int>> LoadPersonPropertyRelationships(ProspectingDataContext prospecting, int contactPersonId)
        {
            return (from rel in prospecting.prospecting_person_property_relationships
                    where rel.contact_person_id == contactPersonId
                    select new KeyValuePair<int, int>(rel.prospecting_property_id, rel.relationship_to_property)).ToList();
        }

        /// <summary>
        /// Creates a new prospect for an entirely new prospected location
        /// </summary>
        public static ProspectingProperty CreateNewProspectingRecord(NewProspectingEntity recordToCreate)
        {
            var standaloneUnit = recordToCreate.PropertyMatches[0];
            if (!recordToCreate.SeeffAreaId.HasValue)
            {
                if (!ProspectWithinOwnedSuburbs(standaloneUnit.LatLng))
                {
                    throw new Exception("Cannot create new prospect: The property falls outside of your available suburbs.");
                }
            }

            using (var baseData = new ls_baseEntities())
            using (var prospecting = new ProspectingDataContext())
            {
                int? areaId;
                if (recordToCreate.SeeffAreaId.HasValue)
                {
                    areaId = recordToCreate.SeeffAreaId.Value;
                }
                else
                {
                    var spatial = new SpatialServiceReader();
                    var spatialSuburb = spatial.GetSuburbFromID(standaloneUnit.LatLng.Lat, standaloneUnit.LatLng.Lng);
                    areaId = (spatialSuburb != null && spatialSuburb.SeeffAreaID.HasValue) ? spatialSuburb.SeeffAreaID.Value : (int?)null;
                    if (areaId == null)
                    {
                        throw new Exception("Cannot create new prospect: The system cannot allocate a Seeff ID to the area, please contact support for further information.");
                    }
                }

                var newPropRecord = new prospecting_property
                {
                    lightstone_property_id = standaloneUnit.LightstonePropId.Value,
                    latitude = standaloneUnit.LatLng.Lat,
                    longitude = standaloneUnit.LatLng.Lng,
                    property_address = string.Concat(standaloneUnit.StreetName + ", " + standaloneUnit.Suburb + ", " + standaloneUnit.City),
                    street_or_unit_no = standaloneUnit.StreetOrUnitNo,
                    seeff_area_id = areaId,
                    lightstone_reg_date = standaloneUnit.RegDate,
                    erf_no = int.Parse(standaloneUnit.ErfNo, CultureInfo.InvariantCulture),
                    ss_fh = standaloneUnit.SS_FH,
                    ss_id = standaloneUnit.SS_ID,
                    ss_unique_identifier = standaloneUnit.SS_UNIQUE_IDENTIFIER,
                    ss_name = !string.IsNullOrEmpty(standaloneUnit.SSName) ? standaloneUnit.SSName : null,
                    ss_number = !string.IsNullOrEmpty(standaloneUnit.SSNumber) ? standaloneUnit.SSNumber : null,
                    unit = !string.IsNullOrEmpty(standaloneUnit.Unit) ? standaloneUnit.Unit : null,
                    ss_door_number = null,
                    last_purch_price = !string.IsNullOrEmpty(standaloneUnit.PurchPrice) ? decimal.Parse(standaloneUnit.PurchPrice, CultureInfo.InvariantCulture) : (decimal?)null,

                    // Farms
                    farm_name = !string.IsNullOrEmpty(standaloneUnit.FarmName) ? standaloneUnit.FarmName : null,
                    portion_no = !string.IsNullOrEmpty(standaloneUnit.Portion) ? int.Parse(standaloneUnit.Portion) : (int?)null,
                    lightstone_suburb = !string.IsNullOrEmpty(standaloneUnit.LightstoneSuburb) ? standaloneUnit.LightstoneSuburb : null,

                    created_by = Guid.Parse((string)HttpContext.Current.Session["user_guid"]),
                    created_date = DateTime.Now
                };

                var recordInserted = false;
                prospecting.prospecting_properties.InsertOnSubmit(newPropRecord);
                try
                {
                    prospecting.SubmitChanges(); // Create the property first before adding contacts
                    recordInserted = true;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot insert duplicate key in object"))
                    {
                        throw new DuplicatePropertyRecordException { ErrorMsg = "Property already exists in the system.", SeeffAreaId = areaId.Value };
                    }

                    throw;
                }

                if (recordInserted)
                {
                    var marketShareRecord = baseData.base_data.FirstOrDefault(bd => bd.property_id == newPropRecord.lightstone_property_id);
                    if (marketShareRecord != null)
                    {
                        marketShareRecord.property_address = newPropRecord.property_address.TrimStart(new[] { ' ', ',', ' ' });
                        marketShareRecord.street_or_unit_no = newPropRecord.street_or_unit_no;
                        try
                        {
                            baseData.SaveChanges();
                        }
                        catch { }
                    }
                }

                foreach (var owner in standaloneUnit.Owners)
                {
                    if (owner.ContactEntityType == ContactEntityType.NaturalPerson)
                    {
                        var newContact = new ContactDataPacket { ContactPerson = (ProspectingContactPerson)owner, ProspectingPropertyId = newPropRecord.prospecting_property_id, ContactCompanyId = null };
                        SaveContactPerson(newContact);
                    }
                    if (owner.ContactEntityType == ContactEntityType.JuristicEntity)
                    {
                        var newContact = new CompanyContactDataPacket { ContactCompany = (ProspectingContactCompany)owner, ProspectingPropertyId = newPropRecord.prospecting_property_id };
                        SaveContactCompany(newContact);
                    }
                }

                var property = CreateProspectingProperty(prospecting, newPropRecord, false, true, true, false);
                recordToCreate.SeeffAreaId = property.SeeffAreaId = areaId;
                return property;
            }
        }

        private static ProspectingContactCompany SaveContactCompany(CompanyContactDataPacket contactCompanyDataPacket)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var incomingContactCompany = contactCompanyDataPacket.ContactCompany;
                string ckNoOfNewCompany = contactCompanyDataPacket.ContactCompany.CKNumber;
                // Search for a contact with this id number
                var companyWithExistingCKNumber = (from c in prospecting.prospecting_contact_companies
                                                   where c.CK_number == ckNoOfNewCompany
                                                   select c).FirstOrDefault();
                if (companyWithExistingCKNumber != null)
                {
                    // A company with this CK number already exists, we must ensure that they are linked to the property
                    var existingRelationshipToProperty = (from ppr in prospecting.prospecting_company_property_relationships
                                                          where ppr.contact_company_id == companyWithExistingCKNumber.contact_company_id
                                                          && ppr.prospecting_property_id == contactCompanyDataPacket.ProspectingPropertyId
                                                          select ppr).FirstOrDefault();

                    // TODO: This code will need to change to accomodate multiple records for the same person who changed their relationship to the property over time.
                    if (existingRelationshipToProperty == null)
                    {
                        // New person-property relationship
                        var companyPropertyRelationship = new prospecting_company_property_relationship
                        {
                            contact_company_id = companyWithExistingCKNumber.contact_company_id,
                            prospecting_property_id = contactCompanyDataPacket.ProspectingPropertyId.Value,
                            relationship_to_property = ProspectingLookupData.CompanyPropertyRelationshipTypes.First(s => s.Value == "Owner").Key,
                            created_date = DateTime.Now
                        };
                        prospecting.prospecting_company_property_relationships.InsertOnSubmit(companyPropertyRelationship);
                        prospecting.SubmitChanges();
                    }
                }
                else
                {
                    // This is a brand new company contact
                    // Create and insert a new contact
                    var newContactCompany = new prospecting_contact_company
                    {
                        CK_number = contactCompanyDataPacket.ContactCompany.CKNumber,
                        company_type = contactCompanyDataPacket.ContactCompany.CompanyType,
                        company_name = contactCompanyDataPacket.ContactCompany.CompanyName,
                        created_by = Guid.Parse((string)HttpContext.Current.Session["user_guid"]),
                        created_date = DateTime.Now
                    };
                    prospecting.prospecting_contact_companies.InsertOnSubmit(newContactCompany);
                    prospecting.SubmitChanges();
                    incomingContactCompany.ContactCompanyId = newContactCompany.contact_company_id;

                    // New person-property relationship
                    var propertyCompanyRelationship = new prospecting_company_property_relationship
                    {
                        contact_company_id = newContactCompany.contact_company_id,
                        prospecting_property_id = contactCompanyDataPacket.ProspectingPropertyId.Value,
                        relationship_to_property = ProspectingLookupData.CompanyPropertyRelationshipTypes.First(s => s.Value == "Owner").Key,
                        created_date = DateTime.Now
                    };
                    prospecting.prospecting_company_property_relationships.InsertOnSubmit(propertyCompanyRelationship);
                    prospecting.SubmitChanges();
                }

                prospecting.SubmitChanges();
                return incomingContactCompany;
            }
        }

        private static bool ProspectWithinOwnedSuburbs(GeoLocation geoLocation)
        {
            var availableSuburbs = (List<UserSuburb>)HttpContext.Current.Session["user_suburbs"];
            var spatial = new SpatialServiceReader();
            var spatialSuburb = spatial.GetSuburbFromID(geoLocation.Lat, geoLocation.Lng);
            var suburbID = (spatialSuburb != null && spatialSuburb.SeeffAreaID.HasValue) ? spatialSuburb.SeeffAreaID.Value : (int?)null;

            return suburbID == null ? false : availableSuburbs.Any(sub => sub.SuburbId == suburbID);
        }


        public static List<GeoLocation> PolyCoordsFromString(string inputSet)
        {
            string[] coordSets = inputSet.Split(new[] { ',' });
            return (from c in coordSets
                    let latLongSet = c.Split(new[] { ' ' })
                    select new GeoLocation
                    {
                        Lat = Decimal.Parse(latLongSet[0]),
                        Lng = Decimal.Parse(latLongSet[1])
                    }).ToList();
        }

        public static JsonSerializerSettings CreateDefaultJsonSettings()
        {
            return new JsonSerializerSettings { ContractResolver = new JsonNetPropertyNameResolverForSerialization() };
        }

        public static bool ConvertToLatLng(string latInput, string lngInput, out Decimal latOutput, out Decimal lngOutput)
        {
            latOutput = 0;
            lngOutput = 0;
            try
            {
                latOutput = Convert.ToDecimal(latInput, CultureInfo.InvariantCulture);
                lngOutput = Convert.ToDecimal(lngInput, CultureInfo.InvariantCulture);

                return true;
            }
            catch { }

            return false;
        }

        public static string SerializeToJsonWithDefaults(object obj)
        {
            return JsonConvert.SerializeObject(obj, CreateDefaultJsonSettings());
        }

        public static T Deserialise<T>(string json)
        {
            return (T)JsonConvert.DeserializeObject<T>(json);
        }

        public static void UpdateProspectingRecord(ProspectingInputData dataPacket)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var propectingPropertyId = dataPacket.ProspectingPropertyId;
                var streetOrUnitNo = dataPacket.StreetOrUnitNo;
                var propAddress = dataPacket.PropertyAddress;
                var unitDoorNo = dataPacket.SSDoorNo;

                var existingRecord = prospecting.prospecting_properties.First(pp => pp.prospecting_property_id == propectingPropertyId);
                if (dataPacket.SS_FH == "SS")
                {
                    existingRecord.ss_door_number = unitDoorNo;
                    if (dataPacket.TitleCaseSS == true)
                    {
                        existingRecord.ss_name = new CultureInfo("en-US", false).TextInfo.ToTitleCase(existingRecord.ss_name.ToLower()).Replace("Ss ", "SS ");
                    }
                }
                else
                {
                    existingRecord.property_address = propAddress;
                    existingRecord.street_or_unit_no = streetOrUnitNo;
                }

                existingRecord.erf_size = dataPacket.ErfSize;
                existingRecord.dwell_size = dataPacket.DwellingSize;
                existingRecord.condition = dataPacket.Condition;
                existingRecord.beds = dataPacket.Beds;
                existingRecord.baths = dataPacket.Baths;
                existingRecord.receptions = dataPacket.Receptions;
                existingRecord.studies = dataPacket.Studies;
                existingRecord.garages = dataPacket.Garages;
                existingRecord.parking_bays = dataPacket.ParkingBays;
                existingRecord.pool = dataPacket.Pool;
                existingRecord.staff_accomodation = dataPacket.StaffAccomodation;

                existingRecord.is_short_term_rental = dataPacket.IsShortTermRental;
                existingRecord.is_long_term_rental = dataPacket.IsLongTermRental;
                existingRecord.is_commercial = dataPacket.IsCommercial;
                existingRecord.is_agricultural = dataPacket.IsAgricultural;
                existingRecord.is_investment = dataPacket.IsInvestment;
                existingRecord.property_listing_id = dataPacket.PropertyListingId;

                prospecting.SubmitChanges();
            }
        }

        /// <summary>
        /// *** Prospecting user authorisation web methods ***
        /// </summary>

        public static UserDataResponsePacket LoadUser(Guid userGuid, Guid sessionKey, bool impersonate)
        {
            using (var authService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                var userAuthPacket = authService.AuthenticateAndGetUserInfo(userGuid, sessionKey, impersonate);
                if (!userAuthPacket.Authenticated)
                {
                    throw new UserSessionExpiredException(); // This is not a true "expired session" but we can treat it as such.
                }

                var businessUnitUsers = new List<UserDataResponsePacket>(
                        from bu in userAuthPacket.BusinessUnitUsers
                        select new UserDataResponsePacket
                        {
                            UserGuid = Guid.Parse(bu.Guid),
                            UserName = bu.UserName,
                            UserSurname = bu.UserSurname,
                            RegistrationId = bu.RegistrationId
                        });

                var followupBundle = LoadFollowups(userGuid, businessUnitUsers, new List<long>());
                var userPacket = new UserDataResponsePacket
                {
                    UserGuid = userGuid,
                    AvailableSuburbs = LoadSuburbInfoForUser(userAuthPacket.SuburbsList),
                    StaticProspectingData = SerialiseStaticProspectingData(),
                    AvailableCredit = userAuthPacket.AvailableCredit,
                    Authenticated = userAuthPacket.Authenticated,
                    UserName = userAuthPacket.UserName,
                    UserSurname = userAuthPacket.UserSurname,
                    IsProspectingManager = userAuthPacket.IsProspectingManager,
                    EmailAddress = userAuthPacket.EmailAddress,
                    ProspectingManager = new UserDataResponsePacket
                    {
                        IsProspectingManager = true,
                        EmailAddress = userAuthPacket.ManagerDetails.First().EmailAddress,
                        UserName = userAuthPacket.ManagerDetails.First().UserName
                    },
                    BusinessUnitUsers = businessUnitUsers,
                    FollowupActivities = followupBundle.Followups,
                    TotalFollowups = followupBundle.TotalCount,
                    HasCommAccess = userAuthPacket.CommunicationEnabled.Value,
                    CanCreateListing = userAuthPacket.CanCreateListing,
                    BusinessUnitID = userAuthPacket.BusinessUnitID,
                    TrustLookupsEnabled = userAuthPacket.TrustLookupsEnabled,
                    ActivityTypes = ProspectingLookupData.ActivityTypes,
                    ActivityFollowupTypes = ProspectingLookupData.ActivityFollowupTypes,
                    BranchID = userAuthPacket.BranchID,
                    RegistrationId = userAuthPacket.RegistrationId,
                    ExportPermission = userAuthPacket.ExportPermission,
                    PermissionLevelForLists = userAuthPacket.PermissionLevelLists
                };

                return userPacket;
            }
        }

        public static string RetrieveUserSignature()
        {
            using (var authService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                var user = RequestHandler.GetUserSessionObject();
                return authService.RetrieveUserSignature(user.UserGuid);
            }
        }

        public static FollowupBundle LoadFollowups(Guid userGuid, List<UserDataResponsePacket> businessUnitUsers, List<long> loadedFollowupActivities)
        {
            FollowupBundle fb = new FollowupBundle();
            List<FollowUpActivity> followups = new List<FollowUpActivity>();
            using (var prospecting = new ProspectingDataContext())
            {
                IEnumerable<activity_log> activities = from act in prospecting.activity_logs
                                                       where act.allocated_to == userGuid && act.followup_date != null && act.followup_date <= DateTime.Now
                                                       orderby act.followup_date
                                                       select act;

                activities = activities.OrderByDescending(d => d.followup_date);

                List<long?> parentIds = prospecting.activity_logs.Where(a => a.parent_activity_id != null).Select(a => a.parent_activity_id).ToList();
                activities = activities.Where(a => !parentIds.Contains(a.activity_log_id));

                // Only load follow ups that do not have children
                //Where(a => !prospecting.activity_logs.Any(t => t.parent_activity_id == a.activity_log_id));
                fb.TotalCount = activities.Count();

                // Exclude followups that have already been loaded
                activities = activities.Where(a => !loadedFollowupActivities.Contains(a.activity_log_id));

                // Only ever load 10 max items at a time
                activities = activities.Take(10);
                foreach (var act in activities)
                {
                    var createdByUser = businessUnitUsers.FirstOrDefault(b => b.UserGuid == act.created_by);
                    string createdBy = createdByUser != null && createdByUser.UserGuid != Guid.Empty ? createdByUser.UserName + " " + createdByUser.UserSurname : "System";
                    var propertyRecord = prospecting.prospecting_properties.First(pp => pp.lightstone_property_id == act.lightstone_property_id);
                    string propAddress = GetFormattedAddress(act.lightstone_property_id);

                    var activityType = prospecting.activity_types.FirstOrDefault(t => t.activity_type_id == act.activity_type_id);
                    string activityTypeName = "";
                    if (activityType != null)
                    {
                        activityTypeName = activityType.activity_name;
                    }
                    var activityFollowupType = prospecting.activity_followup_types.FirstOrDefault(t => t.activity_followup_type_id == act.activity_followup_type_id);
                    string activityFollowupTypeName = "";
                    if (activityFollowupType != null)
                    {
                        activityFollowupTypeName = activityFollowupType.activity_name;
                    }
                    int? seeffAreaId = prospecting.prospecting_properties.First(pp => pp.lightstone_property_id == act.lightstone_property_id).seeff_area_id;
                    var followup = new FollowUpActivity
                    {
                        ActivityLogId = act.activity_log_id,
                        ActivityTypeId = act.activity_type_id,
                        ActivityTypeName = activityTypeName,
                        FollowupActivityTypeName = activityFollowupTypeName,
                        Comment = act.comment,
                        CreatedBy = act.created_by,
                        CreatedByUsername = createdBy,
                        FollowupDate = act.followup_date,
                        LightstonePropertyId = act.lightstone_property_id,
                        ParentActivityId = act.parent_activity_id,
                        RelatedToContactPersonId = act.contact_person_id,
                        RelatedToContactPerson = LoadContactForFollowup(prospecting, act.contact_person_id),
                        ActivityFollowupTypeId = act.activity_followup_type_id,
                        PropertyAddress = propAddress,
                        SeeffAreaId = seeffAreaId,
                        PropertyType = propertyRecord.ss_fh
                    };
                    followups.Add(followup);
                }
            }

            fb.Followups = followups.OrderByDescending(s => s.FollowupDate).ThenByDescending(s => s.ActivityLogId).ToList();
            return fb;
        }

        // Must send back the contact id as well as all the phone + tel no's: update their guid's to null
        // If a contact ID number does not exist, create the contact and link them to the new property
        // If a contact ID number already exists, simply link them to the new property
        public static ProspectingContactPerson SaveContactPerson(ContactDataPacket dataPacket)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var incomingContact = dataPacket.ContactPerson;
                var contactRecord = (from c in prospecting.prospecting_contact_persons
                               where c.contact_person_id == dataPacket.ContactPerson.ContactPersonId
                               select c).FirstOrDefault();
                int uniqueContactsByIDForNewID = 1;
                if (contactRecord != null)
                {
                    // Find all existing contacts with the ID number of the source contact
                    uniqueContactsByIDForNewID = prospecting.prospecting_contact_persons.Count(cp => cp.id_number == incomingContact.IdNumber);
                }
                if (contactRecord != null && (uniqueContactsByIDForNewID == 1 || uniqueContactsByIDForNewID == 0))
                {
                    // Update contact person
                    contactRecord.person_title = incomingContact.Title.HasValue ? (int?)incomingContact.Title.Value : null;
                    contactRecord.firstname = incomingContact.Firstname;
                    contactRecord.surname = incomingContact.Surname;
                    contactRecord.id_number = incomingContact.IdNumber;
                    contactRecord.person_gender = incomingContact.Gender;
                    contactRecord.updated_date = DateTime.Now;
                    contactRecord.comments_notes = incomingContact.Comments;
                    contactRecord.is_popi_restricted = incomingContact.IsPOPIrestricted;

                    contactRecord.deceased_status = incomingContact.DeceasedStatus;
                    contactRecord.age_group = incomingContact.AgeGroup;
                    contactRecord.location = incomingContact.Location;
                    contactRecord.marital_status = incomingContact.MaritalStatus;
                    contactRecord.home_ownership = incomingContact.HomeOwnership;
                    contactRecord.directorship = incomingContact.Directorship;
                    contactRecord.physical_address = incomingContact.PhysicalAddress;
                    contactRecord.employer = incomingContact.Employer;
                    contactRecord.occupation = incomingContact.Occupation;
                    contactRecord.bureau_adverse_indicator = incomingContact.BureauAdverseIndicator;
                    contactRecord.citizenship = incomingContact.Citizenship;

                    contactRecord.optout_emails = incomingContact.EmailOptout;
                    contactRecord.optout_sms = incomingContact.SMSOptout;
                    contactRecord.do_not_contact = incomingContact.DoNotContact;
                    if (incomingContact.EmailOptout)
                    {
                        contactRecord.email_contactability_status = 1;
                    }
                    else {
                        contactRecord.email_contactability_status = incomingContact.EmailContactabilityStatus;
                    }

                    prospecting.SubmitChanges();

                    if (dataPacket.ContactCompanyId.HasValue)
                    {
                        // This contact has a relationship with a company as opposed to a property
                        var personCompanyRelation = (from r in prospecting.prospecting_person_company_relationships
                                                     join q in prospecting.prospecting_company_property_relationships on r.contact_company_id equals q.contact_company_id
                                                     where r.contact_person_id == contactRecord.contact_person_id && q.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                     select r).FirstOrDefault();
                        if (personCompanyRelation != null)
                        {
                            personCompanyRelation.relationship_to_company = incomingContact.PersonCompanyRelationshipType.Value;
                            personCompanyRelation.contact_company_id = dataPacket.ContactCompanyId.Value;
                            personCompanyRelation.updated_date = DateTime.Now;
                        }
                        else
                        {
                            personCompanyRelation = new prospecting_person_company_relationship
                            {
                                contact_person_id = contactRecord.contact_person_id,
                                contact_company_id = dataPacket.ContactCompanyId,
                                relationship_to_company = incomingContact.PersonCompanyRelationshipType,
                                created_date = DateTime.Now
                            };
                            prospecting.prospecting_person_company_relationships.InsertOnSubmit(personCompanyRelation);
                        }

                        prospecting.SubmitChanges();

                        // *** delete any existing relationship person-property relationship 
                        var personPropertyRelation = (from r in prospecting.prospecting_person_property_relationships
                                                      where r.contact_person_id == contactRecord.contact_person_id &&
                                                      r.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                      select r).FirstOrDefault();
                        if (personPropertyRelation != null)
                        {
                            prospecting.prospecting_person_property_relationships.DeleteOnSubmit(personPropertyRelation);
                            prospecting.SubmitChanges();
                        }
                    }
                    else
                    {
                        // This contact has a direct relationship to a property. Update the person-property relationship table
                        var personPropertyRelation = (from r in prospecting.prospecting_person_property_relationships
                                                      where r.contact_person_id == contactRecord.contact_person_id &&
                                                      r.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                      select r).FirstOrDefault();
                        if (incomingContact.PersonPropertyRelationships == null || incomingContact.PersonPropertyRelationships.Count == 0)
                        {
                            // TODO: this line makes an incorrect assumption: that the existing contact linked to the new property is an "owner". Fix this.
                            incomingContact.PersonPropertyRelationships = new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(dataPacket.ProspectingPropertyId.Value, ProspectingLookupData.PersonPropertyRelationshipTypes.First().Key) };
                        }
                        // If a relationship already exists against this property just update it. TODO
                        if (personPropertyRelation != null)
                        {
                            personPropertyRelation.relationship_to_property = GetPersonRelationshipToProperty(dataPacket.ProspectingPropertyId, incomingContact.PersonPropertyRelationships);
                            personPropertyRelation.updated_date = DateTime.Now;
                        }
                        else
                        {
                            // Add a relationship
                            // New person-property relationship
                            personPropertyRelation = new prospecting_person_property_relationship
                            {
                                contact_person_id = contactRecord.contact_person_id,
                                prospecting_property_id = dataPacket.ProspectingPropertyId.Value,
                                relationship_to_property = GetPersonRelationshipToProperty(dataPacket.ProspectingPropertyId, incomingContact.PersonPropertyRelationships),
                                created_date = DateTime.Now
                            };
                            prospecting.prospecting_person_property_relationships.InsertOnSubmit(personPropertyRelation);
                            prospecting.SubmitChanges();
                        }

                        // Delete an existing company relationship if exists
                        var personCompanyRelation = (from r in prospecting.prospecting_person_company_relationships
                                                     join q in prospecting.prospecting_company_property_relationships on r.contact_company_id equals q.contact_company_id
                                                     where r.contact_person_id == contactRecord.contact_person_id && q.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                     select r).FirstOrDefault();
                        if (personCompanyRelation != null)
                        {
                            prospecting.prospecting_person_company_relationships.DeleteOnSubmit(personCompanyRelation);
                            prospecting.SubmitChanges();
                        }
                    }

                    // If not null, then we must assume there is valid data in the list. An empty list indicates
                    // that we should delete records. Same logic applies to email addresses.
                    var existingContactItems = from phoneOrEmail in prospecting.prospecting_contact_details
                                               where phoneOrEmail.contact_person_id == incomingContact.ContactPersonId
                                               && !phoneOrEmail.deleted
                                               select phoneOrEmail;
                    // Update contact info
                    if (incomingContact.PhoneNumbers != null)
                    {
                        UpdateContactDetails(incomingContact.ContactPersonId.Value,
                            ProspectingLookupData.ContactPhoneTypes,
                            incomingContact.PhoneNumbers,
                            existingContactItems,
                            incomingContact.IsPOPIrestricted,
                            prospecting);
                    }
                    if (incomingContact.EmailAddresses != null)
                    {
                        UpdateContactDetails(incomingContact.ContactPersonId.Value,
                            ProspectingLookupData.ContactEmailTypes,
                            incomingContact.EmailAddresses,
                            existingContactItems,
                            incomingContact.IsPOPIrestricted,
                            prospecting);
                    }

                    AddClientSynchronisationRequest(contactRecord.contact_person_id);
                }
                else
                {
                    string idNumberOfNewContact = dataPacket.ContactPerson.IdNumber;
                    // Search for a contact with this id number
                    var contactsWithExistingIDNumbers = (from c in prospecting.prospecting_contact_persons
                                                       where c.id_number == idNumberOfNewContact
                                                       select c).ToList();
                    if (contactsWithExistingIDNumbers.Any())
                    {
                        foreach (var contact in contactsWithExistingIDNumbers)
                        {
                            if (contact.firstname == "(unknown firstname)" &&
                     !string.IsNullOrWhiteSpace(dataPacket.ContactPerson.Firstname) &&
                     dataPacket.ContactPerson.Firstname != "(unknown firstname)")
                            {
                                contact.firstname = dataPacket.ContactPerson.Firstname;
                                prospecting.SubmitChanges();
                            }

                            if (contact.surname == "(unknown surname)" &&
                                !string.IsNullOrWhiteSpace(dataPacket.ContactPerson.Surname) &&
                                dataPacket.ContactPerson.Surname != "(unknown surname)")
                            {
                                contact.surname = dataPacket.ContactPerson.Surname;
                                prospecting.SubmitChanges();
                            }

                            prospecting.SubmitChanges();
                            AddClientSynchronisationRequest(contact.contact_person_id);

                            if (dataPacket.ContactCompanyId.HasValue)
                            {
                                // This contact has a relationship with a company as opposed to a property
                                var personCompanyRelation = (from r in prospecting.prospecting_person_company_relationships
                                                             join q in prospecting.prospecting_company_property_relationships on r.contact_company_id equals q.contact_company_id
                                                             where r.contact_person_id == contact.contact_person_id && q.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                             select r).FirstOrDefault();
                                if (personCompanyRelation != null)
                                {
                                    personCompanyRelation.relationship_to_company = incomingContact.PersonCompanyRelationshipType.Value;
                                    personCompanyRelation.contact_company_id = dataPacket.ContactCompanyId.Value;
                                    personCompanyRelation.updated_date = DateTime.Now;
                                    prospecting.SubmitChanges();
                                }
                                else
                                {
                                    personCompanyRelation = new prospecting_person_company_relationship
                                    {
                                        contact_person_id = contact.contact_person_id,
                                        contact_company_id = dataPacket.ContactCompanyId,
                                        relationship_to_company = incomingContact.PersonCompanyRelationshipType,
                                        created_date = DateTime.Now
                                    };
                                    prospecting.prospecting_person_company_relationships.InsertOnSubmit(personCompanyRelation);
                                    prospecting.SubmitChanges();
                                }
                            }
                            else
                            {
                                // A contact with this id number already exists, we must ensure that they are linked to the property
                                var existingRelationshipToProperty = from ppr in prospecting.prospecting_person_property_relationships
                                                                     where ppr.contact_person_id == contact.contact_person_id
                                                                     && ppr.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                                     select ppr;

                                // TODO: This code will need to change to accomodate multiple records for the same person who changed their relationship to the property over time.
                                if (existingRelationshipToProperty.Count() == 0)
                                {
                                    // Do an additional check to determine whether a contact with this ID number doesn't already exist
                                    existingRelationshipToProperty = from ppr in prospecting.prospecting_person_property_relationships
                                                                     where ppr.prospecting_property_id == dataPacket.ProspectingPropertyId && ppr.prospecting_contact_person.id_number == contact.id_number
                                                                     select ppr;
                                    if (existingRelationshipToProperty.Count() == 0)
                                    {

                                        incomingContact.PropertiesOwned = LoadPropertiesOwnedByThisContact(idNumberOfNewContact, prospecting);

                                        // New person-property relationship
                                        var personPropertyRelation = new prospecting_person_property_relationship
                                        {
                                            contact_person_id = contact.contact_person_id,
                                            prospecting_property_id = dataPacket.ProspectingPropertyId.Value,
                                            relationship_to_property = GetPersonRelationshipToProperty(dataPacket.ProspectingPropertyId, incomingContact.PersonPropertyRelationships),
                                            created_date = DateTime.Now
                                        };
                                        prospecting.prospecting_person_property_relationships.InsertOnSubmit(personPropertyRelation);
                                        prospecting.SubmitChanges();
                                    }
                                }
                            }

                            // If not null, then we must assume there is valid data in the list. An empty list indicates
                            // that we should delete records. Same logic applies to email addresses.
                            var existingContactItems = from phoneOrEmail in prospecting.prospecting_contact_details
                                                       where phoneOrEmail.contact_person_id == contact.contact_person_id
                                                       && !phoneOrEmail.deleted
                                                       select phoneOrEmail;
                            // Update contact info
                            if (incomingContact.PhoneNumbers != null)
                            {
                                UpdateContactDetails(contact.contact_person_id,
                                    ProspectingLookupData.ContactPhoneTypes,
                                    incomingContact.PhoneNumbers,
                                    existingContactItems,
                                    incomingContact.IsPOPIrestricted,
                                    prospecting);
                            }
                            if (incomingContact.EmailAddresses != null)
                            {
                                UpdateContactDetails(contact.contact_person_id,
                                    ProspectingLookupData.ContactEmailTypes,
                                    incomingContact.EmailAddresses,
                                    existingContactItems,
                                    incomingContact.IsPOPIrestricted,
                                    prospecting);
                            }

                        }                 
                    }
                    else
                    {
                        // This is a brand new contact
                        // Create and insert a new contact
                        var newContact = new prospecting_contact_person
                        {
                            person_title = incomingContact.Title.HasValue ? (int?)incomingContact.Title.Value : null,
                            firstname = incomingContact.Firstname,
                            surname = incomingContact.Surname,
                            id_number = incomingContact.IdNumber,
                            person_gender = incomingContact.Gender,
                            job_title = "",
                            comments_notes = incomingContact.Comments,
                            created_by = Guid.Parse((string)HttpContext.Current.Session["user_guid"]),
                            created_date = DateTime.Now,
                            deceased_status = incomingContact.DeceasedStatus,
                            age_group = incomingContact.AgeGroup,
                            location = incomingContact.Location,
                            marital_status = incomingContact.MaritalStatus,
                            home_ownership = incomingContact.HomeOwnership,
                            directorship = incomingContact.Directorship,
                            physical_address = incomingContact.PhysicalAddress,
                            employer = incomingContact.Employer,
                            occupation = incomingContact.Occupation,
                            bureau_adverse_indicator = incomingContact.BureauAdverseIndicator,
                            citizenship = incomingContact.Citizenship,
                            is_popi_restricted = incomingContact.IsPOPIrestricted,
                            optout_emails = incomingContact.EmailOptout,
                            optout_sms = incomingContact.SMSOptout,
                            do_not_contact = incomingContact.DoNotContact,
                            email_contactability_status = incomingContact.EmailOptout ? 1 : 4
                        };
                        prospecting.prospecting_contact_persons.InsertOnSubmit(newContact);
                        prospecting.SubmitChanges();
                        AddClientSynchronisationRequest(newContact.contact_person_id);
                        incomingContact.ContactPersonId = newContact.contact_person_id;

                        if (dataPacket.ContactCompanyId.HasValue)
                        {
                            var personCompanyRelation = new prospecting_person_company_relationship
                            {
                                contact_person_id = incomingContact.ContactPersonId.Value,
                                contact_company_id = dataPacket.ContactCompanyId,
                                relationship_to_company = incomingContact.PersonCompanyRelationshipType,
                                created_date = DateTime.Now
                            };
                            prospecting.prospecting_person_company_relationships.InsertOnSubmit(personCompanyRelation);
                            prospecting.SubmitChanges();
                        }
                        else
                        {
                            // New person-property relationship
                            var personPropertyRelation = new prospecting_person_property_relationship
                            {
                                contact_person_id = newContact.contact_person_id,
                                prospecting_property_id = dataPacket.ProspectingPropertyId.Value,
                                relationship_to_property = GetPersonRelationshipToProperty(dataPacket.ProspectingPropertyId, incomingContact.PersonPropertyRelationships),
                                created_date = DateTime.Now
                            };
                            prospecting.prospecting_person_property_relationships.InsertOnSubmit(personPropertyRelation);
                            prospecting.SubmitChanges();
                        }
                    }
                }

                prospecting.SubmitChanges();

                //Set the property as prospected
                MarkAsProspected((int)dataPacket.ProspectingPropertyId);

                return incomingContact;
            }
        }

        private static int GetPersonRelationshipToProperty(int? prospectingPropId, List<KeyValuePair<int, int>> relationships)
        {
            int rel = (from n in relationships
                       where n.Key == prospectingPropId
                       select n.Value).FirstOrDefault();

            if (rel == 0)
            {
                rel = ProspectingLookupData.PersonPropertyRelationshipTypes.First(s => s.Value == "Owner").Key;
            }

            return rel;
        }


        private static void AddClientSynchronisationRequest(int contactPersonID)
        {
            try
            {
                var loggedInUser = RequestHandler.GetUserSessionObject();
                using (var prospecting = new ProspectingDataContext())
                {
                    var clientSyncRecord = new client_sync_log
                    {
                        contact_person_id = contactPersonID,
                        user_guid = loggedInUser.UserGuid,
                        date_time = DateTime.Now
                    };
                    prospecting.client_sync_logs.InsertOnSubmit(clientSyncRecord);
                    prospecting.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                // Log
            }
        }

        // TODO: investigate deleting records from contact details table and keep tabs on record counts coming in as new records are added/removed..


        public static void UpdateContactDetails(int contactPersonId, List<KeyValuePair<int, string>> affectedContactTypes, IEnumerable<ProspectingContactDetail> incomingDetails, IEnumerable<prospecting_contact_detail> existingContactItems, bool isPOPIaction, ProspectingDataContext prospecting)
        {
            // Note on how this method must be handled:
            // Its important that if incomingDetails is null, we mustn't do anything, just return. 
            // POSSIBLE SCENARIOS:
            // 1) The array is null: don't touch existing contact details
            // 2) The array is empty: remove all contact items for this contact 
            // 3) Array has items: add new items, update existing items, AND any existing items in the DB not in the array must be deleted

            // SCENARIO (1)
            if (incomingDetails == null) return;

            List<int> affectedContactTypeIds = affectedContactTypes.Select(g => g.Key).ToList();

            if (isPOPIaction)
            {
                var allItemsToDelete = from c in prospecting.prospecting_contact_details
                                       where c.contact_person_id == contactPersonId
                                       && affectedContactTypeIds.Contains(c.contact_detail_type)
                                       select c;

                prospecting.prospecting_contact_details.DeleteAllOnSubmit(allItemsToDelete);
                return;
            }

            // SCENARIO (2)
            if (incomingDetails.Count() == 0)
            {
                var itemsToFlagDeleted = from c in prospecting.prospecting_contact_details
                                         where c.contact_person_id == contactPersonId
                                         && affectedContactTypeIds.Contains(c.contact_detail_type)
                                         && !c.deleted
                                         select c;

                foreach (var item in itemsToFlagDeleted)
                {
                    item.deleted = true;
                    item.deleted_by = Guid.Parse(HttpContext.Current.Session["user_guid"].ToString());
                    item.deleted_date = DateTime.Now;
                }
                ManageDeletedContactDetailsForSession(itemsToFlagDeleted.Count());
                return;
            }

            List<int> newContactIDs = new List<int>();
            // SCENARIO (3)
            foreach (var item in incomingDetails)
            {
                // If the item has a guid as id, it means that this is a new item ready for insert
                Guid newGuid;
                if (!string.IsNullOrEmpty(item.ItemId) && Guid.TryParse(item.ItemId, out newGuid))
                {
                    if (item.IsValid)
                    {
                        prospecting_contact_detail detail = new prospecting_contact_detail
                        {
                            contact_detail_type = item.ItemType.Value,
                            contact_person_id = contactPersonId,
                            contact_detail = item.ItemContent,
                            is_primary_contact = item.IsPrimary.HasValue ? item.IsPrimary.Value : false,
                            intl_dialing_code_id = item.IntDialingCode,
                            eleventh_digit = item.EleventhDigit
                        };
                        prospecting.prospecting_contact_details.InsertOnSubmit(detail);
                        prospecting.SubmitChanges();
                        item.ItemId = detail.prospecting_contact_detail_id.ToString();

                        newContactIDs.Add(int.Parse(item.ItemId));
                    }
                }
                else
                {
                    // Must already exist, so update it
                    var existingContactItem = existingContactItems.FirstOrDefault(c => c.prospecting_contact_detail_id.ToString() == item.ItemId);
                    if (existingContactItem != null)
                    {
                        if (item.IsValid)
                        {
                            existingContactItem.contact_detail_type = item.ItemType.Value;
                            existingContactItem.contact_detail = item.ItemContent;
                            existingContactItem.is_primary_contact = item.IsPrimary.HasValue ? item.IsPrimary.Value : false;
                            existingContactItem.intl_dialing_code_id = item.IntDialingCode;
                            existingContactItem.eleventh_digit = item.EleventhDigit;
                        }

                        newContactIDs.Add(existingContactItem.prospecting_contact_detail_id);
                    }
                }
            }

            // Finally remove other contact details in the DB that were not affected:
            var removedItemsToBeFlaggedForDelete = from c in prospecting.prospecting_contact_details
                                                   where c.contact_person_id == contactPersonId
                                                   && affectedContactTypeIds.Contains(c.contact_detail_type) // contact item is either a phone or email
                                                   && !newContactIDs.Contains(c.prospecting_contact_detail_id) // not affected by new insert/update
                                                   && !c.deleted
                                                   select c;
            foreach (var item in removedItemsToBeFlaggedForDelete)
            {
                item.deleted = true;
                item.deleted_by = Guid.Parse(HttpContext.Current.Session["user_guid"].ToString());
                item.deleted_date = DateTime.Now;
            }
            ManageDeletedContactDetailsForSession(removedItemsToBeFlaggedForDelete.Count());
        }

        private static void ManageDeletedContactDetailsForSession(int numDeletedDetails)
        {
            int deletedContactDetails = Convert.ToInt32(HttpContext.Current.Session["deleted_item_count"]);
            deletedContactDetails += numDeletedDetails;
            HttpContext.Current.Session["deleted_item_count"] = deletedContactDetails;
        }

        public static List<NewProspectingEntity> FindMatchingProperties(SearchInputPacket searchInputValues)
        {
            List<LightstonePropertyMatch> matches = new List<LightstonePropertyMatch>();
            // Step 1:  Search lightstone for matches
            using (lightstoneSeeffService.Seeff service = new lightstoneSeeffService.Seeff())
            {
                DataSet result = null;
                try
                {
                    int propertyID = 0;
                    int.TryParse(searchInputValues.PropertyID, out propertyID);
                    result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", searchInputValues.DeedTown, searchInputValues.ErfNo, searchInputValues.Portion
                        , searchInputValues.SSName, searchInputValues.Unit, searchInputValues.Suburb, searchInputValues.StreetName, searchInputValues.StreetOrUnitNo, ""
                        , "", "", "", "", searchInputValues.OwnerName, searchInputValues.OwnerIdNumber, searchInputValues.EstateName, "", propertyID, 500, "", "", 0, 0);

                    LogLightstoneCall("FindMatchingProperties()", Newtonsoft.Json.JsonConvert.SerializeObject(searchInputValues));

                    if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                    {
                        List<DataRow> dataRowCollection = new List<DataRow>();
                        dataRowCollection.AddRange(result.Tables[1].Rows.Cast<DataRow>());
                        // When only the owner ID number or name is given, we must search again using all the property Id's in the result set
                        if (OnlyOwnerNameOrIDProvided(searchInputValues))
                        {
                            dataRowCollection.Clear();
                            // Find all the property Id's in the results, and search again using those
                            List<int> resultsPropIds = new List<int>();
                            foreach (DataRow row in result.Tables[1].Rows)
                            {
                                int propId = Convert.ToInt32(row["PROP_ID"]);
                                resultsPropIds.Add(propId);
                            }
                            resultsPropIds = resultsPropIds.Distinct().ToList();
                            foreach (int propId in resultsPropIds)
                            {
                                result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", "", "", "", "", "", "", "", "", ""
                                                                        , "", "", "", "", "", "", "", "", propId, 1000, "", "", 0, 0);
                                dataRowCollection.AddRange(result.Tables[1].Rows.Cast<DataRow>());

                                LogLightstoneCall("foreach (int propId in resultsPropIds)", "resultsPropIds=" + resultsPropIds.Count);
                            }
                        }
                        using (var prospecting = new ProspectingDataContext())
                        {
                            List<int> existingLightstoneProps = prospecting.prospecting_properties.Select(p => p.lightstone_property_id).ToList();
                            foreach (DataRow row in dataRowCollection)
                            {
                                AddLightstonePropertyRow(row, matches, existingLightstoneProps);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                catch
                { /* Do nothing here, an empty set will be returned to front-end */ }
            }

            List<NewProspectingEntity> prospectingEntities = GenerateOutputForProspectingEntity(matches, null);
            return prospectingEntities;
        }

        private static bool OnlyOwnerNameOrIDProvided(SearchInputPacket searchInputValues)
        {
            if (!string.IsNullOrEmpty(searchInputValues.DeedTown)) return false;
            if (!string.IsNullOrEmpty(searchInputValues.EstateName)) return false;
            if (!string.IsNullOrEmpty(searchInputValues.SSName)) return false;
            if (!string.IsNullOrEmpty(searchInputValues.ErfNo)) return false;
            if (!string.IsNullOrEmpty(searchInputValues.Suburb)) return false;
            if (!string.IsNullOrEmpty(searchInputValues.StreetName)) return false;
            if (!string.IsNullOrEmpty(searchInputValues.StreetOrUnitNo)) return false;
            if (!string.IsNullOrEmpty(searchInputValues.PropertyID)) return false;

            if (!string.IsNullOrEmpty(searchInputValues.OwnerName) || !string.IsNullOrEmpty(searchInputValues.OwnerIdNumber))
                return true;

            return false;
        }

        public static ProspectingContactPerson SearchForExistingContactWithDetails(ContactDetails contactDetails)
        {
            contactDetails.EmailAddresses = contactDetails.EmailAddresses.Select(s => s.ToLower()).ToList();
            contactDetails.PhoneNumbers = contactDetails.PhoneNumbers.Select(s => s.ToLower()).ToList();
            using (var prospecting = new ProspectingDataContext())
            {
                prospecting_contact_person existingContactWithDetail = null;
                if (contactDetails.EmailAddresses.Any() || contactDetails.PhoneNumbers.Any())
                {
                    // Search for existing contact details
                    existingContactWithDetail = (from cp in prospecting.prospecting_contact_persons
                                                 join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
                                                 where (contactDetails.PhoneNumbers.Contains(cd.contact_detail.ToLower()) ||
                                                    contactDetails.EmailAddresses.Contains(cd.contact_detail.ToLower()))
                                                    && cp.id_number != contactDetails.IdNumber
                                                    && !cd.deleted
                                                 select cp).FirstOrDefault();
                }
                else
                {
                    // Search by ID number exclusively
                    if (!string.IsNullOrEmpty(contactDetails.IdNumber))
                    {
                        existingContactWithDetail = (from cp in prospecting.prospecting_contact_persons
                                                     where cp.id_number == contactDetails.IdNumber
                                                     select cp).FirstOrDefault();
                    }
                }

                if (existingContactWithDetail != null)
                {
                    var phoneTypeIds = ProspectingLookupData.ContactPhoneTypes.Select(k => k.Key);
                    var emailTypeIds = ProspectingLookupData.ContactEmailTypes.Select(k => k.Key);
                    ProspectingContactPerson person = new ProspectingContactPerson
                    {
                        Firstname = existingContactWithDetail.firstname,
                        Surname = existingContactWithDetail.surname,
                        IdNumber = existingContactWithDetail.id_number,
                        PropertiesOwned = LoadPropertiesOwnedByThisContact(existingContactWithDetail.id_number, prospecting),
                        ContactPersonId = existingContactWithDetail.contact_person_id,
                        PersonPropertyRelationships = new List<KeyValuePair<int, int>>(),//ProspectingStaticData.PersonPropertyRelationshipTypes.First(s => s.Value == "Owner").Key,
                        PersonCompanyRelationshipType = null,
                        Gender = existingContactWithDetail.person_gender,
                        Title = existingContactWithDetail.person_title,
                        IsPOPIrestricted = existingContactWithDetail.is_popi_restricted,

                        DeceasedStatus = existingContactWithDetail.deceased_status,
                        AgeGroup = existingContactWithDetail.age_group,
                        Location = existingContactWithDetail.location,
                        MaritalStatus = existingContactWithDetail.marital_status,
                        HomeOwnership = existingContactWithDetail.home_ownership,
                        Directorship = existingContactWithDetail.directorship,
                        PhysicalAddress = existingContactWithDetail.physical_address,
                        Employer = existingContactWithDetail.employer,
                        Occupation = existingContactWithDetail.occupation,
                        BureauAdverseIndicator = existingContactWithDetail.bureau_adverse_indicator,
                        Citizenship = existingContactWithDetail.citizenship,

                        EmailOptout = existingContactWithDetail.optout_emails,
                        SMSOptout = existingContactWithDetail.optout_sms,
                        DoNotContact = existingContactWithDetail.do_not_contact,
                        EmailContactabilityStatus = existingContactWithDetail.email_contactability_status,

                        PhoneNumbers = (from det in prospecting.prospecting_contact_details
                                        where det.contact_person_id == existingContactWithDetail.contact_person_id
                                        && phoneTypeIds.Contains(det.contact_detail_type)
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
                                            IntDialingCodePrefix = det.prospecting_area_dialing_code.dialing_code_id,
                                            EleventhDigit = det.eleventh_digit
                                        }).ToList(),

                        EmailAddresses = (from det in prospecting.prospecting_contact_details
                                          where det.contact_person_id == existingContactWithDetail.contact_person_id
                                          && emailTypeIds.Contains(det.contact_detail_type)
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
                                          }).ToList()
                    };

                    return person;
                }
                return null;
            }
        }

        public static List<ProspectingProperty> LoadPropertiesOwnedByThisContact(string idNumber, ProspectingDataContext dataContext)
        {
            List<ProspectingProperty> prospectables = new List<ProspectingProperty>();
            var prospectingDB = dataContext != null ? dataContext : new ProspectingDataContext();
            var contact = (from c in prospectingDB.prospecting_contact_persons
                           where c.id_number == idNumber
                           select c).FirstOrDefault();

            if (contact != null)
            {
                var propertiesOwned = from pp in prospectingDB.prospecting_properties
                                      join ppr in prospectingDB.prospecting_person_property_relationships on pp.prospecting_property_id equals ppr.prospecting_property_id
                                      where ppr.contact_person_id == contact.contact_person_id
                                      select pp;
                foreach (var prospectable in propertiesOwned)
                {
                    if (!prospectables.Any(p => p.LightstonePropertyId == prospectable.lightstone_property_id))
                    {
                        ProspectingProperty prop = CreateProspectingProperty(prospectingDB, prospectable, false, false, false, false);
                        prospectables.Add(prop);
                    }
                }
            }

            return prospectables;
        }

        public static ProspectingSuburb LoadProspectingSuburb(SuburbDataRequestPacket suburbDataRequest)
        {
            var spatialReader = new SpatialServiceReader();
            var spatialSuburb = spatialReader.LoadSuburb(suburbDataRequest.SuburbId);

            ProspectingSuburb suburb = new ProspectingSuburb();
            suburb.LocationID = suburbDataRequest.SuburbId;

            suburb.PolyCoords = ProspectingCore.LoadPolyCoords(spatialSuburb.PolyWKT);
            suburb.ProspectingProperties = ProspectingCore.CreateProspectableProperties(suburbDataRequest.SuburbId);
            suburb.LocationName = spatialSuburb.AreaName; //ProspectingCore.GetAreaName(suburbDataRequest.SuburbId);
            suburb.UnderMaintenance = spatialSuburb.UnderMaintenance;

            return suburb;
        }

        public static PersonEnquiryResponsePacket PerformLookup(ProspectingInputData dataPacket)
        {
            EnquiryServiceManager enquiryService = new EnquiryServiceManager(dataPacket);
            if (enquiryService.IsPersonLookup())
            {
                return enquiryService.PerformPersonEnquiry();
            }
            else
            {
                throw new Exception("Unsupported lookup type");
            }
        }

        public static List<NewProspectingEntity> GetMatchingAddresses(ProspectingInputData dataPacket)
        {
            List<LightstonePropertyMatch> matches = new List<LightstonePropertyMatch>();
            using (lightstoneSeeffService.Seeff service = new lightstoneSeeffService.Seeff())
            {
                DataSet result = null;
                double lat = Convert.ToDouble(dataPacket.LatLng.Lat);
                double lng = Convert.ToDouble(dataPacket.LatLng.Lng);
                try
                {
                    result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", "", "", "", "", "", ""
                        , "", "", "", "", "", "", "", "", "", "", "", 0, 1000, "", "", lng, lat);

                    LogLightstoneCall("GetMatchingAddresses()", Newtonsoft.Json.JsonConvert.SerializeObject(dataPacket));
                }
                catch { return new List<NewProspectingEntity>(); }
                if (result.Tables[0] != null && result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    using (var prospecting = new ProspectingDataContext())
                    {
                        List<int> existingLightstoneProps = prospecting.prospecting_properties.Select(p => p.lightstone_property_id).ToList();
                        foreach (DataRow row in result.Tables[1].Rows)
                        {
                            AddLightstonePropertyRow(row, matches, existingLightstoneProps);
                        }
                    }
                }
            }

            var matchesForSuburb = GetMatchesForCurrentSuburbOnly(matches, dataPacket.SeeffAreaId.Value);
            var responsePacket = GenerateOutputForProspectingEntity(matchesForSuburb, dataPacket.SeeffAreaId.Value);
            return responsePacket;
        }

        private static GeoLocation TryConvertToGeoLocation(object lat, object lng)
        {
            try
            {
                GeoLocation latLng = new GeoLocation();
                latLng.Lat = Convert.ToDecimal(lat);
                latLng.Lng = Convert.ToDecimal(lng);
                return latLng;
            }
            catch { return null; }
        }

        private static void AddLightstonePropertyRow(DataRow row, List<LightstonePropertyMatch> matches, List<int> existingLightstoneProperties)
        {
            int lightstonePropId = Convert.ToInt32(row["PROP_ID"]);
            // Try find an item in the list with same prop id in order to add another Owner to prop.
            var existingMatch = matches.FirstOrDefault(m => m.LightstonePropId == lightstonePropId);
            if (existingMatch != null)
            {
                // This means that there is another owner on this property so add it to the owners list for this property
                var propOwner = GetOwnerFromDataRow(row);
                if (propOwner != null)
                {
                    var existingOwner = DetermineIfOwnerExists(existingMatch.Owners, propOwner); //existingMatch.Owners.FirstOrDefault(o => o.IdNumber == propOwner.IdNumber);
                    if (existingOwner == null)
                    {
                        existingMatch.Owners.Add(propOwner);
                    }
                }
            }
            else
            {
                string streetOrUnitNo = row["STREET_NUMBER"].ToString();
                if (string.IsNullOrWhiteSpace(streetOrUnitNo))
                {
                    streetOrUnitNo = "n/a";
                }

                bool propertyPropIdExists = existingLightstoneProperties.Any(p => p == lightstonePropId);

                // The result must have a valid lat/long in order for it to be processed - otherwise ignore
                GeoLocation loc = TryConvertToGeoLocation(row["Y"], row["X"]);
                if (loc == null)
                    return;

                Func<object, string> formatSSName = nameField => nameField.ToString().Replace("&", " AND ");

                // This is a new property that must be added
                LightstonePropertyMatch match = new LightstonePropertyMatch
                {
                    // Property details
                    City = row["MUNICNAME"] != null ? row["MUNICNAME"].ToString() : string.Empty,
                    Suburb = row["DEEDTOWN"] != null ? row["DEEDTOWN"].ToString() : string.Empty,
                    StreetName = (row["STREET_NAME"] != null ? row["STREET_NAME"].ToString() : string.Empty) + " " + (row["STREET_TYPE"] != null ? row["STREET_TYPE"].ToString() : string.Empty),
                    StreetOrUnitNo = streetOrUnitNo,
                    LightstonePropId = lightstonePropId,
                    RegDate = row["REG_DATE"] != null ? row["REG_DATE"].ToString() : string.Empty,
                    LatLng = loc,
                    SSName = row["SECTIONAL_TITLE"] != null ? formatSSName(row["SECTIONAL_TITLE"]) : string.Empty,
                    SS_FH = row["property_type"] != null ? row["property_type"].ToString() : string.Empty,
                    PurchPrice = row["PURCHASE_PRICE"] != null ? row["PURCHASE_PRICE"].ToString() : string.Empty,
                    Unit = row["UNIT"] != null ? row["UNIT"].ToString() : string.Empty,
                    SSNumber = row["SS_NUMBER"] != null ? row["SS_NUMBER"].ToString() : string.Empty,
                    SS_UnitNoFrom = row["SS_UnitNoFrom"] != null ? row["SS_UnitNoFrom"].ToString() : string.Empty,
                    SS_UnitTo = row["SS_UnitTo"] != null ? row["SS_UnitTo"].ToString() : string.Empty,
                    LightstoneIdExists = propertyPropIdExists,
                    ErfNo = row["ERF"] != null ? row["ERF"].ToString() : string.Empty,
                    SS_ID = row["SS_ID"] != null ? row["SS_ID"].ToString() : string.Empty,
                    FarmName = row["FARMNAME"] != null ? row["FARMNAME"].ToString() : string.Empty,
                    Portion = row["PORTION"] != null ? row["PORTION"].ToString() : string.Empty,
                    LightstoneSuburb = row["SUBURB"] != null ? row["SUBURB"].ToString() : string.Empty
                };

                var owner = GetOwnerFromDataRow(row);
                var owners = owner != null ? new List<IProspectingContactEntity>(new[] { owner }) : new List<IProspectingContactEntity>();
                match.Owners = owners;
                match.SS_UNIQUE_IDENTIFIER = CreateSSUniqueId(match);
                matches.Add(match);
            }
        }

        private static string CreateSSUniqueId(LightstonePropertyMatch match)
        {
            if (match.SS_FH == "SS" || !string.IsNullOrEmpty(match.SSName) || !string.IsNullOrEmpty(match.SS_ID))
            {
                string ssName = match.SSName.Replace(" ", "");
                string lat = match.LatLng.Lat.ToString();
                if (lat.Length < 12)
                {
                    lat = lat.PadRight(12, '0');
                }
                lat = lat.Replace(".", "");

                string lng = match.LatLng.Lng.ToString();
                if (lng.Length < 11)
                {
                    lng = lng.PadRight(11, '0');
                }
                lng = lng.Replace(".", "");

                string uniqueId = ssName + lat + lng;
                return uniqueId;
            }
            return null;
        }

        private static IProspectingContactEntity DetermineIfOwnerExists(List<IProspectingContactEntity> owners, IProspectingContactEntity propOwner)
        {
            return owners.FirstOrDefault(o => o.IsSameEntity(propOwner));
        }

        private static NewProspectingEntity CreateProspectingEntityForSS(List<LightstonePropertyMatch> unitsForSS, string ssName, int? seeffAreaId)
        {
            var singleUnit = unitsForSS.First(u => !string.IsNullOrEmpty(u.StreetName) && !string.IsNullOrEmpty(u.StreetOrUnitNo));
            return new NewProspectingEntity
            {
                IsSectionalScheme = true,
                PropertyMatches = unitsForSS.OrderBy(m => m.Unit).ToList(),
                Exists = DetermineIfSSAlreadyExists(singleUnit.SS_UNIQUE_IDENTIFIER),
                SectionalScheme = ssName,
                //SS_ID = singleUnit.SS_ID,
                SS_UNIQUE_IDENTIFIER = singleUnit.SS_UNIQUE_IDENTIFIER,
                SeeffAreaId = seeffAreaId
            };
        }


        private static List<NewProspectingEntity> GenerateOutputForProspectingEntity(List<LightstonePropertyMatch> matchesForSuburb, int? seeffAreaId)
        {
            List<NewProspectingEntity> prospectingEntities = new List<NewProspectingEntity>();
            if (matchesForSuburb.Count == 0)
            {
                return new List<NewProspectingEntity>();
            }

            // Sectional schemes
            var ssUnits = matchesForSuburb.Where(ss => !string.IsNullOrEmpty(ss.SS_ID));
            List<LightstonePropertyMatch> ssMatches = new List<LightstonePropertyMatch>(ssUnits);
            foreach (var ssUnitCollection in ssUnits.GroupBy(ss => ss.SS_ID))
            {
                var groupsByLatLng = ssUnitCollection.GroupBy(ss => ss.LatLng);
                if (groupsByLatLng.Count() > 1)
                {
                    // Find the exceptional record
                    var outcastGroup = groupsByLatLng.FirstOrDefault(gr => gr.Count() == 1);
                    if (outcastGroup != null)
                    {
                        // Find a 'normal' unit and apply its lat/lng to the outcast
                        var normalUnit = groupsByLatLng.FirstOrDefault(gr => gr.Count() > 1).First();
                        var outcast = outcastGroup.First();
                        outcast.LatLng = new GeoLocation { Lat = normalUnit.LatLng.Lat, Lng = normalUnit.LatLng.Lng };
                        outcast.SS_UNIQUE_IDENTIFIER = CreateSSUniqueId(outcast);
                    }
                }

                string erfNo = ssUnitCollection.First().ErfNo;
                string ss_id = ssUnitCollection.Key;
                var unitsToInclude = matchesForSuburb.Where(m =>
                {
                    if (m.SS_FH == "FH" && m.ErfNo == erfNo)
                        return true;
                    if (m.SS_FH == "FRM" && m.ErfNo == erfNo && m.LatLng.Equals(ssUnitCollection.First().LatLng))
                        return true;
                    return false;
                }).ToList();
                string ssName = ssUnitCollection.First(u => !String.IsNullOrEmpty(u.SSName)).SSName;
                string ssUniqueId = ssUnitCollection.First(u => !String.IsNullOrEmpty(u.SS_UNIQUE_IDENTIFIER)).SS_UNIQUE_IDENTIFIER;
                unitsToInclude.ForEach(u => { u.SS_FH = "FS"; u.SS_UNIQUE_IDENTIFIER = ssUniqueId; u.SS_ID = ss_id; u.SSName = ssName; });
                var ssEntity = CreateProspectingEntityForSS(ssUnitCollection.Union(unitsToInclude).ToList(), ssName, seeffAreaId);

                prospectingEntities.Add(ssEntity);
                ssMatches.AddRange(unitsToInclude);
            }

            matchesForSuburb = matchesForSuburb.OrderBy(m => m.SS_FH).ThenBy(m => m.ErfNo).ThenBy(m => m.Portion).ToList();

            // Handle all other matches (normal FH etc)
            foreach (var match in matchesForSuburb.Except(ssMatches))
            {
                if (match.SS_FH == "FH") // Explicity check for FH here. For any other type eg FRM, add code here to make provision for this.
                {
                    prospectingEntities.Add(new NewProspectingEntity
                    {
                        IsSectionalScheme = false,
                        PropertyMatches = new[] { match }.ToList(),
                        Exists = match.LightstoneIdExists,
                        SeeffAreaId = seeffAreaId
                    });
                }

                // Farms
                if (match.SS_FH == "FRM")
                {
                    prospectingEntities.Add(new NewProspectingEntity
                    {
                        IsSectionalScheme = false,
                        IsFarm = true,
                        PropertyMatches = new[] { match }.ToList(),
                        Exists = match.LightstoneIdExists,
                        SeeffAreaId = seeffAreaId
                    });
                }
            }

            return prospectingEntities;
        }

        private static bool DetermineIfSSAlreadyExists(string ssUniqueId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                bool exists = prospecting.prospecting_properties.Any(pp => pp.ss_unique_identifier == ssUniqueId);
                return exists;
            }
        }

        private static ContactEntityType DetermineEntityTypeFromDataRow(DataRow dr)
        {
            string personTypeId = dr["PERSON_TYPE_ID"].ToString();
            switch (personTypeId)
            {
                case "PP": return ContactEntityType.NaturalPerson;
                default: return ContactEntityType.JuristicEntity;
            }
        }

        private static ProspectingContactPerson CreatePersonContactFromDataRow(DataRow dr)
        {
            string idNumber = MakeIDUnique(dr["BUYER_IDCK"].ToString(), dr["SURNAME"].ToString());
            return new ProspectingContactPerson
            {
                Firstname = dr["FIRSTNAME"].ToString(),
                Surname = dr["SURNAME"].ToString(),
                IdNumber = idNumber,
                ContactPersonId = -1,
                Gender = DetermineOwnerGender(idNumber),
                PersonPropertyRelationships = new List<KeyValuePair<int, int>>(),//ProspectingStaticData.PersonPropertyRelationshipTypes.First(t => t.Value == "Owner").Key,
                PersonCompanyRelationshipType = null,
                //PropertiesOwned = LoadPropertiesOwnedByThisContact(idNumber, null),
                ContactEntityType = ContactEntityType.NaturalPerson
            };
        }

        private static ProspectingContactCompany CreateCompanyContactFromDataRow(DataRow dr)
        {
            var ckNo = dr["BUYER_IDCK"].ToString();
            if (string.IsNullOrEmpty(ckNo))
            {
                ckNo = "UNKNOWN_CK_" + dr["BUYER_NAME"].ToString().Replace(" ", "_");
            }
            var pcc = new ProspectingContactCompany
            {
                CKNumber = ckNo,
                CompanyName = dr["BUYER_NAME"].ToString(),
                CompanyType = dr["PERSON_TYPE_ID"].ToString(),
                //CompanyContacts = LoadCompanyContacts(ckNo),
                //ProspectingProperties = LoadPropertiesOwnedByCompany(ckNo),
                ContactEntityType = ContactEntityType.JuristicEntity
            };

            return pcc;
        }

        private static List<ProspectingProperty> LoadPropertiesOwnedByCompany(string ckNo)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var properties = (from pp in prospecting.prospecting_properties
                                  join cpr in prospecting.prospecting_company_property_relationships on pp.prospecting_property_id equals cpr.prospecting_property_id
                                  join c in prospecting.prospecting_contact_companies on cpr.contact_company_id equals c.contact_company_id
                                  where c.CK_number == ckNo
                                  select new ProspectingProperty
                                  {
                                      ProspectingPropertyId = pp.prospecting_property_id,
                                      LightstonePropertyId = pp.lightstone_property_id,
                                      //Contacts = LoadContacts(prospectingContext, prospectingRecord, loadOwnedProperties),
                                      //HasContactsWithDetails = DetermineIfAnyContactsHaveDetails(prospectingRecord.prospecting_property_id),
                                      //HasTracePSEnquiry = HasTracePSEnquiry(prospectingRecord.prospecting_property_id),
                                      LatLng = new GeoLocation { Lat = pp.latitude.Value, Lng = pp.longitude.Value },
                                      PropertyAddress = pp.property_address,
                                      StreetOrUnitNo = pp.street_or_unit_no,
                                      SeeffAreaId = pp.seeff_area_id.HasValue ? pp.seeff_area_id.Value : (int?)null,
                                      LightstoneIDOrCKNo = pp.lightstone_id_or_ck_no,
                                      LightstoneRegDate = pp.lightstone_reg_date,
                                      ErfNo = pp.erf_no,
                                      Comments = pp.comments,
                                      SS_FH = pp.ss_fh == "SS" ? "SS" : "FH", // default to FH for backward compat.
                                      SSName = pp.ss_name,
                                      SSNumber = pp.ss_number,
                                      SS_ID = pp.ss_id,
                                      Unit = pp.unit,
                                      SSDoorNo = pp.ss_door_number,
                                      LastPurchPrice = pp.last_purch_price,
                                      SS_UNIQUE_IDENTIFIER = pp.ss_unique_identifier,
                                      LatestRegDateForUpdate = pp.latest_reg_date,

                                      Baths = pp.baths,
                                      Condition = pp.condition,
                                      Beds = pp.beds,
                                      DwellingSize = pp.dwell_size,
                                      ErfSize = pp.erf_size,
                                      Garages = pp.garages,
                                      Pool = pp.pool,
                                      Receptions = pp.receptions,
                                      StaffAccomodation = pp.staff_accomodation,
                                      Studies = pp.studies,
                                      ParkingBays = pp.parking_bays,

                                      IsShortTermRental = pp.is_short_term_rental,
                                      IsLongTermRental = pp.is_long_term_rental,
                                      IsCommercial = pp.is_commercial,
                                      IsAgricultural = pp.is_agricultural,
                                      IsInvestment = pp.is_investment,

                                      HasContactWithCell = pp.has_cell,
                                      HasContactWithPrimaryCell = pp.has_primary_cell,

                                      HasContactWithEmail = pp.has_email,
                                      HasContactWithPrimaryEmail = pp.has_primary_email,

                                      HasContactWithLandline = pp.has_landline,
                                      HasContactWithPrimaryLandline = pp.has_primary_landline,

                                      PropertyListingId = pp.property_listing_id
                                  }).ToList();
                return properties;
            }
        }

        private static List<ProspectingContactPerson> LoadCompanyContacts(string ckNo)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var phoneTypeIds = ProspectingLookupData.ContactPhoneTypes.Select(k => k.Key);
                var emailTypeIds = ProspectingLookupData.ContactEmailTypes.Select(k => k.Key);
                var companyContacts = (from c in prospecting.prospecting_contact_companies
                                       join cpr in prospecting.prospecting_person_company_relationships on c.contact_company_id equals cpr.contact_company_id
                                       join cc in prospecting.prospecting_contact_persons on cpr.contact_person_id equals cc.contact_person_id
                                       where c.CK_number == ckNo
                                       select new ProspectingContactPerson
                                       {
                                           ContactPersonId = cc.contact_person_id,
                                           PersonCompanyRelationshipType = cpr.person_company_relationship_id,
                                           ContactCompanyId = cpr.contact_company_id,
                                           Firstname = cc.firstname,
                                           Surname = cc.surname,
                                           IdNumber = cc.id_number,
                                           Title = cc.person_title,
                                           Gender = cc.person_gender,
                                           Comments = cc.comments_notes,
                                           IsPOPIrestricted = cc.is_popi_restricted,

                                           EmailOptout = cc.optout_emails,
                                           SMSOptout = cc.optout_sms,
                                           DoNotContact = cc.do_not_contact,
                                           EmailContactabilityStatus = cc.email_contactability_status,

                                           // In due course may need to add the Dracore fields here

                                           PhoneNumbers = (from det in prospecting.prospecting_contact_details
                                                           where det.contact_person_id == cpr.contact_person_id
                                                           && phoneTypeIds.Contains(det.contact_detail_type)
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
                                                               IntDialingCodePrefix = det.prospecting_area_dialing_code.dialing_code_id,
                                                               EleventhDigit = det.eleventh_digit
                                                           }),
                                           EmailAddresses = (from det in prospecting.prospecting_contact_details
                                                             where det.contact_person_id == cpr.contact_person_id
                                                             && emailTypeIds.Contains(det.contact_detail_type)
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
                                                             })
                                       }).ToList();
                return companyContacts;
            }
        }

        private static IProspectingContactEntity GetOwnerFromDataRow(DataRow dr)
        {
            if (string.IsNullOrEmpty(dr["BUYER_IDCK"].ToString()) && string.IsNullOrEmpty(dr["BUYER_NAME"].ToString()))
            {
                return null;
            }

            ContactEntityType entityType = DetermineEntityTypeFromDataRow(dr);
            switch (entityType)
            {
                case ContactEntityType.NaturalPerson: return CreatePersonContactFromDataRow(dr);
                default: return CreateCompanyContactFromDataRow(dr);
            }
        }

        private static string MakeIDUnique(string idNumber, string surname)
        {
            idNumber = idNumber.Trim();

            if (idNumber.Contains("/") || idNumber.Contains("\\"))
            {
                return idNumber; // This is not a person's ID number
            }
            int nr;
            // Just birthday digits available
            if (idNumber.Length == 6 && int.TryParse(idNumber, out nr))
            {
                string result = GeneratePseudoIdentifier();
                return result;
            }

            if (idNumber.Length > 13)
            {
                idNumber = new string(idNumber.Take(13).ToArray());
            }
            return idNumber;
        }

        private static List<LightstonePropertyMatch> GetMatchesForCurrentSuburbOnly(IEnumerable<LightstonePropertyMatch> distinctMatches, int suburbId)
        {
            var spatial = new SpatialServiceReader();
            // Use the current province (from Google)
            using (var prospecting = new ProspectingDataContext())
            {
                List<LightstonePropertyMatch> results = new List<LightstonePropertyMatch>();
                foreach (var match in distinctMatches)
                {
                    var spatialSuburb = spatial.GetSuburbFromID(match.LatLng.Lat, match.LatLng.Lng);
                    var suburbID = (spatialSuburb != null && spatialSuburb.SeeffAreaID.HasValue) ? spatialSuburb.SeeffAreaID.Value : (int?)null;
                    if (suburbID != null && suburbID == suburbId)
                    {
                        results.Add(match);
                    }
                }
                return results;
            }
        }

        public static bool IsValidIDNumber(string input)
        {
            var regex = new Regex("^[0-9]+$");
            long val;
            bool isLong = long.TryParse(input, out val);
            return input.Length == 13 && regex.IsMatch(input) && isLong;
        }

        public static string GetContactNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;
            // For now we check that there are a minimum of 10 digits contained in the number - must allow for: +27724707471, 0724707471, 072 470 7471 and 072-470-7471
            int numDigits = 0;
            if (input.StartsWith("27") && input.Length == 11)
            {
                input = "0" + input.Remove(0, 2);
            }
            numDigits = input.ToCharArray().Count(d => char.IsDigit(d));
            return numDigits >= 10 ? input : null;
        }

        private static string GetEmailAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;
            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return input;
            }
            catch { return null; }
        }

        public static string DetermineOwnerGender(string idNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(idNumber) && idNumber.Length > 7)
                {
                    string G = idNumber.Substring(6, 1);
                    switch (G)
                    {
                        case "0":
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                            return "F";
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                            return "M";
                    }
                }
            }
            catch { }

            return "";
        }

        public static List<ProspectingProperty> CreateSectionalTitle(NewProspectingEntity sectionalTitle)
        {
            // First determine whether the sectional scheme falls within the user's available suburbs by examining the first record.
            // NB.: handle the null case!!
            var latLng = sectionalTitle.PropertyMatches[0].LatLng;
            if (!sectionalTitle.SeeffAreaId.HasValue)
            {
                if (!ProspectWithinOwnedSuburbs(latLng))
                {
                    throw new Exception("Cannot create sectional title: The building falls outside your available suburbs.");
                }
            }

            using (var baseData = new ls_baseEntities())
            using (var prospecting = new ProspectingDataContext())
            {
                prospecting.CommandTimeout = 4 * 60;

                int? areaId = null;
                //if (sectionalTitle.SeeffAreaId.HasValue)
                //{
                //    areaId = sectionalTitle.SeeffAreaId.Value;
                //}
                //else
                //{
                var spatial = new SpatialServiceReader();
                var spatialSuburb = spatial.GetSuburbFromID(latLng.Lat, latLng.Lng);
                areaId = (spatialSuburb != null && spatialSuburb.SeeffAreaID.HasValue) ? spatialSuburb.SeeffAreaID.Value : (int?)null;
                if (areaId == null)
                {
                    throw new Exception("Cannot create new prospect: The system cannot allocate a Seeff ID to the area, please contact support for further information.");
                }
                //}

                // Ensure that we are creating ALL units for this sectional title, not just one
                using (lightstoneSeeffService.Seeff service = new lightstoneSeeffService.Seeff())
                {
                    string erf = sectionalTitle.PropertyMatches.First(s => !string.IsNullOrWhiteSpace(s.ErfNo)).ErfNo;
                    string ssName = sectionalTitle.SectionalScheme;
                    DataSet result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", "", erf, "", ssName, "", ""
                       , "", "", "", "", "", "", "", "", "", "", "", 0, 1000, "", "", 0.0, 0.0);

                    LogLightstoneCall("CreateSectionalTitle()", Newtonsoft.Json.JsonConvert.SerializeObject(sectionalTitle));

                    if (sectionalTitle.PropertyMatches.Count < result.Tables[1].Rows.Count)
                    {
                        List<LightstonePropertyMatch> matches = new List<LightstonePropertyMatch>();
                        List<int> existingLightstoneProps = prospecting.prospecting_properties.Select(p => p.lightstone_property_id).ToList();
                        // This means we must create the sectional title from scratch
                        foreach (DataRow row in result.Tables[1].Rows)
                        {
                            AddLightstonePropertyRow(row, matches, existingLightstoneProps);
                        }
                        var completeSS = GenerateOutputForProspectingEntity(matches, areaId)[0];
                        sectionalTitle = completeSS;
                    }
                }

                List<ProspectingProperty> units = new List<ProspectingProperty>();
                foreach (var unit in sectionalTitle.PropertyMatches)
                {
                    var newPropRecord = new prospecting_property
                    {
                        lightstone_property_id = unit.LightstonePropId.Value,// dataPacket.LightstoneId,
                        latitude = unit.LatLng.Lat,// dataPacket.LatLng.Lat,
                        longitude = unit.LatLng.Lng,
                        property_address = string.Concat(unit.StreetName + ", " + unit.Suburb + ", " + unit.City),// dataPacket.PropertyAddress,
                        street_or_unit_no = unit.StreetOrUnitNo,// dataPacket.StreetOrUnitNo,
                        seeff_area_id = areaId,
                        lightstone_reg_date = unit.RegDate,// dataPacket.LightstoneRegDate,
                        erf_no = int.Parse(unit.ErfNo, CultureInfo.InvariantCulture),// dataPacket.ErfNo,
                        ss_fh = unit.SS_FH,// dataPacket.SS_FH,
                        ss_name = !string.IsNullOrEmpty(unit.SSName) ? unit.SSName : null,
                        ss_number = !string.IsNullOrEmpty(unit.SSNumber) ? unit.SSNumber : null,
                        unit = !string.IsNullOrEmpty(unit.Unit) ? unit.Unit : null,
                        ss_id = !string.IsNullOrEmpty(unit.SS_ID) ? unit.SS_ID : null,
                        ss_unique_identifier = !string.IsNullOrEmpty(unit.SS_UNIQUE_IDENTIFIER) ? unit.SS_UNIQUE_IDENTIFIER : null,
                        last_purch_price = !string.IsNullOrEmpty(unit.PurchPrice) ? decimal.Parse(unit.PurchPrice, CultureInfo.InvariantCulture) : (decimal?)null,

                        // Farms
                        farm_name = !string.IsNullOrEmpty(unit.FarmName) ? unit.FarmName : null,
                        portion_no = !string.IsNullOrEmpty(unit.Portion) ? int.Parse(unit.Portion) : (int?)null,
                        lightstone_suburb = !string.IsNullOrEmpty(unit.LightstoneSuburb) ? unit.LightstoneSuburb : null,

                        created_by = Guid.Parse((string)HttpContext.Current.Session["user_guid"]),
                        created_date = DateTime.Now
                    };

                    var recordInserted = false;
                    prospecting.prospecting_properties.InsertOnSubmit(newPropRecord);
                    try
                    {
                        prospecting.SubmitChanges(); // Create the property first before adding contacts
                        recordInserted = true;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Cannot insert duplicate key in object"))
                        {
                            throw new DuplicatePropertyRecordException { ErrorMsg = "Property already exists in the system.", SeeffAreaId = areaId.Value };
                        }

                        throw;
                    }

                    if (recordInserted)
                    {
                        //do for fh, an for updating.
                        var marketShareRecord = baseData.base_data.FirstOrDefault(bd => bd.property_id == newPropRecord.lightstone_property_id);
                        if (marketShareRecord != null)
                        {
                            marketShareRecord.property_address = string.Concat(newPropRecord.ss_name, ", ", newPropRecord.property_address.TrimStart(new[] { ' ', ',', ' ' }));
                            marketShareRecord.street_or_unit_no = newPropRecord.street_or_unit_no;
                            try
                            {
                                baseData.SaveChanges();
                            }
                            catch {}
                        }
                    }

                    foreach (var owner in unit.Owners)
                    {
                        if (owner.ContactEntityType == ContactEntityType.NaturalPerson)
                        {
                            var newContact = new ContactDataPacket { ContactPerson = (ProspectingContactPerson)owner, ProspectingPropertyId = newPropRecord.prospecting_property_id, ContactCompanyId = null };
                            SaveContactPerson(newContact);
                        }
                        if (owner.ContactEntityType == ContactEntityType.JuristicEntity)
                        {
                            var newContact = new CompanyContactDataPacket { ContactCompany = (ProspectingContactCompany)owner, ProspectingPropertyId = newPropRecord.prospecting_property_id };
                            SaveContactCompany(newContact);
                        }
                    }

                    var property = CreateProspectingProperty(prospecting, newPropRecord, false, false, false, false);
                    sectionalTitle.SeeffAreaId = property.SeeffAreaId = areaId;
                    units.Add(property);
                }

                return units;
            }
        }

        public static ProspectingProperty GetProspectingProperty(ProspectingPropertyId dataPacket)
        {
            UnlockCurrentProspectingRecord();

            using (var prospectingDB = new ProspectingDataContext())
            {
                int lightstoneId = dataPacket.LightstonePropertyId;
                var propRecord = prospectingDB.prospecting_properties.First(pp => pp.lightstone_property_id == lightstoneId);

                ProspectingProperty property = CreateProspectingProperty(prospectingDB, propRecord, false, true, true, dataPacket.LoadActivities);

                var currentUser = RequestHandler.GetUserSessionObject();
                if (propRecord.locked_by_guid != null && propRecord.locked_by_guid != currentUser.UserGuid)
                {
                    property.IsLockedByOtherUser = true;
                    var userWithLock = currentUser.BusinessUnitUsers.First(bu => bu.UserGuid == propRecord.locked_by_guid);
                    property.LockedUsername = userWithLock.UserName + " " + userWithLock.UserSurname;
                    property.LockedDateTime = propRecord.locked_datetime;
                }
                else
                {
                    // Lock the record as in-use by this user
                    propRecord.locked_by_guid = currentUser.UserGuid;
                    propRecord.locked_datetime = DateTime.Now;
                    try
                    {
                        prospectingDB.SubmitChanges();
                    }
                    catch (ChangeConflictException) { }
                }

                return property;
            }
        }

        public static void UnlockCurrentProspectingRecord()
        {
            var currentUser = RequestHandler.GetUserSessionObject();
            using (var prospectingDB = new ProspectingDataContext())
            {
                var propertiesLockedByUser = prospectingDB.prospecting_properties.Where(pp => pp.locked_by_guid == currentUser.UserGuid);
                foreach (var pp in propertiesLockedByUser)
                {
                    pp.locked_by_guid = null;
                    pp.locked_datetime = null;
                    try
                    {
                        prospectingDB.SubmitChanges();
                    }
                    catch (ChangeConflictException) { }
                }
            }
        }

        public static void MarkAsProspected(int propertyId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                int contactableContactCount = prospecting.prospected_propety_contact_count(propertyId).Value;
                var targetProp = prospecting.prospecting_properties.First(pp => pp.prospecting_property_id == propertyId);
                targetProp.prospected = contactableContactCount > 0;
                prospecting.SubmitChanges();
            }
        }

        public static ProspectingEntityOutputBundle CreateNewProspectingEntities(ProspectingEntityInputBundle prospectingEntityBundle, List<NewProspectingEntity> searchResults)
        {
            ProspectingEntityOutputBundle outputBundle = new ProspectingEntityOutputBundle();
            var firstResult = searchResults.FirstOrDefault();
            outputBundle.SeeffAreaId = firstResult != null ? firstResult.SeeffAreaId : null;
            try
            {
                // Create sectional titles
                // Because we already stored a cache of search results in the session before displaying it on the front-end, we now match what the user selected to what we have in memory.
                var sectionaTitlesToCreate = searchResults.Where(s => s.IsSectionalScheme && prospectingEntityBundle.SectionalSchemes.Any(f => f.SS_UNIQUE_IDENTIFIER == s.SS_UNIQUE_IDENTIFIER)).ToList();
                foreach (var ss in sectionaTitlesToCreate)
                {
                    var ssUnits = CreateSectionalTitle(ss);
                    string ssUniqueId = ssUnits.First(u => !String.IsNullOrEmpty(u.SS_UNIQUE_IDENTIFIER)).SS_UNIQUE_IDENTIFIER;
                    outputBundle.SectionalSchemes.Add(ssUniqueId, ssUnits);
                    outputBundle.TargetProspect = ssUnits.First(); // We don't need to set this in the loop each time, but saves me writing additional code checks.
                    outputBundle.SeeffAreaId = ssUnits.First().SeeffAreaId;
                }

                // Create free-holds
                var targetUnits = searchResults.Where(s => !s.IsSectionalScheme);
                var targetUnitsToCreate = targetUnits.Where(fh => prospectingEntityBundle.FHProperties.Any(h => h.PropertyMatches[0].LightstonePropId == fh.PropertyMatches[0].LightstonePropId));
                foreach (var unit in targetUnitsToCreate)
                {
                    var unitResult = CreateNewProspectingRecord(unit);
                    outputBundle.FhProperties.Add(unitResult);
                    outputBundle.TargetProspect = unitResult;
                    outputBundle.SeeffAreaId = unit.SeeffAreaId;
                }
            }
            catch (Exception e)
            {
                outputBundle.CreationErrorMsg = e.Message;
                var duplicateEx = e as DuplicatePropertyRecordException;
                if (duplicateEx != null)
                {
                    outputBundle.CreationErrorMsg = duplicateEx.ErrorMsg;
                    outputBundle.SeeffAreaId = duplicateEx.SeeffAreaId;
                }
                return outputBundle;
            }

            return outputBundle;
        }

        internal static void MarkAsProspected(int lightstonePropertyId, bool prospected)
        {
            using (var prospectingDB = new ProspectingDataContext())
            {
                var prop = prospectingDB.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropertyId);
                prop.prospected = prospected;
                prospectingDB.SubmitChanges();
            }
        }

        internal static void SendWarningNotificationToManager(UserDataResponsePacket user)
        {
            UserDataResponsePacket manager = user.ProspectingManager;
            string emailTemplate = string.Format(@"Hi {0}{1}{1}This is a system-generated email to notify you that a Prospecting user, {2},
                                                 has removed contact information from one or more prospects in the system, and you are receiving this email
                                                 as they have reached their threshold for the session.{1}
                                                 This activity might be completely normal, however we would like to ensure that every user is using the system responsibly for its intended purpose.{1}{1}
                                                 Kindly follow up with the user to ensure compliance with your Prospecting requirements.{1}{1}
                                                 Kind regards,{1}
                                                 Prospecting Support", manager.UserName, "<p />", user.UserName + " " + user.UserSurname);

            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                SendEmail(manager.EmailAddress, manager.UserName, "reports@seeff.com", null, "reports@seeff.com", "Prospecting system notification", emailTemplate);
            }
            else
            {
                SendEmail("danie.vdm@seeff.com", manager.UserName, "reports@seeff.com", "", "reports@seeff.com", "TEST EMAIL >>>>" + "Prospecting system notification", "TEST EMAIL >>>>" + emailTemplate);
            }
        }

        internal static void SendEmail(string toAddress, string displayName, string fromAddress, string ccAddress, string bccAddress, string subject, string body, bool htmlBody = true)
        {
            MailAddress from = new MailAddress(fromAddress, fromAddress, System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress(toAddress, displayName);
            MailMessage message = new MailMessage(from, to);
            if (!string.IsNullOrEmpty(ccAddress))
            {
                message.CC.Add(ccAddress);
            }
            if (!string.IsNullOrEmpty(bccAddress))
            {
                message.Bcc.Add(bccAddress);
            }
            message.Bcc.Add("danie.vdm@seeff.com");

            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;

            message.IsBodyHtml = htmlBody;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Body = body;

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("reports@seeff.com", "cvbiv76c6c");
                    smtpClient.Host = "smtp2.macrolan.co.za";
                    smtpClient.Port = 587;
                    smtpClient.Timeout = 20000;
                    smtpClient.Send(message);
                }
            }
            catch
            {
                // Add logging code later.
            }
        }

        public static long UpdateInsertActivity(ProspectingActivity act)
        {
            var currentUser = RequestHandler.GetUserSessionObject();
            // If Allocated To is null, default it to the logged in user
            if (!act.AllocatedTo.HasValue)
            {
                act.AllocatedTo = currentUser.UserGuid;
            }
            using (var prospecting = new ProspectingDataContext())
            {
                activity_log activityRecord = null;
                if (act.IsForInsert)
                {
                    activityRecord = new activity_log
                    {
                        lightstone_property_id = act.LightstonePropertyId,
                        followup_date = act.FollowUpDate,
                        allocated_to = act.AllocatedTo,
                        activity_type_id = act.ActivityTypeId,
                        comment = act.Comment,
                        created_by = currentUser.UserGuid,
                        created_date = DateTime.Now,
                        contact_person_id = act.ContactPersonId,
                        // Add the rest later
                        parent_activity_id = act.ParentActivityId,
                        activity_followup_type_id = act.ActivityFollowupTypeId
                    };
                    prospecting.activity_logs.InsertOnSubmit(activityRecord);
                }
                else if (act.IsForUpdate)
                {
                    activityRecord = prospecting.activity_logs.First(ac => ac.activity_log_id == act.ActivityLogId);
                    activityRecord.followup_date = act.FollowUpDate;
                    activityRecord.allocated_to = act.AllocatedTo;
                    activityRecord.activity_type_id = act.ActivityTypeId;
                    activityRecord.comment = act.Comment;
                    activityRecord.contact_person_id = act.ContactPersonId;
                    // Add the rest later
                    activityRecord.parent_activity_id = act.ParentActivityId;
                    activityRecord.activity_followup_type_id = act.ActivityFollowupTypeId;
                }
                prospecting.SubmitChanges();
                return activityRecord.activity_log_id;
            }
        }

        public static ActivityBundle GetActivityLookupData()
        {
            ActivityBundle activityBundle = new ActivityBundle();

            UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
            activityBundle.BusinessUnitUsers = user.BusinessUnitUsers;
            activityBundle.ActivityTypes = ProspectingLookupData.ActivityTypes;
            activityBundle.ActivityFollowupTypes = ProspectingLookupData.ActivityFollowupTypes;

            return activityBundle;
        }

        public static void MakeDefaultContactDetail(string itemId)
        {
            int contactDetailId = Convert.ToInt32(itemId);
            using (var prospecting = new ProspectingDataContext())
            {
                var contactDetail = prospecting.prospecting_contact_details.FirstOrDefault(c => c.prospecting_contact_detail_id == contactDetailId);
                if (contactDetail != null)
                {
                    if (contactDetail.contact_detail_type == 3) // cell
                    {
                        // Remove the primary flag for all other cell no's for this contact
                        var allCellNosForContact = prospecting.prospecting_contact_details.Where(c => c.contact_person_id == contactDetail.contact_person_id && c.contact_detail_type == 3);
                        foreach (var detail in allCellNosForContact)
                        {
                            detail.is_primary_contact = false;
                        }
                    }
                    contactDetail.is_primary_contact = true;
                    prospecting.SubmitChanges();
                    AddClientSynchronisationRequest(contactDetail.contact_person_id);
                }
            }
        }

        //private static void UpdateContactDetailStatisticsToPropertyRecord(int prospectingPropertyId)
        //{
        //    using (var prospecting = new ProspectingDataContext())
        //    {
        //        var prospectingProperty = prospecting.prospecting_properties.FirstOrDefault(pp => pp.prospecting_property_id == prospectingPropertyId);
        //        if (prospectingProperty != null)
        //        {
        //            var contactDetailsFromCompanyRelationships = from cpr in prospecting.prospecting_company_property_relationships
        //                                                         join pcr in prospecting.prospecting_person_company_relationships on cpr.contact_company_id equals pcr.contact_company_id
        //                                                         join cp in prospecting.prospecting_contact_persons on pcr.contact_person_id equals cp.contact_person_id
        //                                                         join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
        //                                                         where cpr.prospecting_property == prospectingProperty && !cd.deleted
        //                                                         select cd;
        //            var contactDetailsFromDirectRelationships = from ppr in prospecting.prospecting_person_property_relationships 
        //                                                        join cp in prospecting.prospecting_contact_persons on ppr.contact_person_id equals cp.contact_person_id
        //                                                        join cd in prospecting.prospecting_contact_details on cp.contact_person_id equals cd.contact_person_id
        //                                                        where ppr.prospecting_property == prospectingProperty && !cd.deleted
        //                                                        select cd;

        //            // Emails
        //            int countEmails1 = contactDetailsFromCompanyRelationships.Where(cd => cd.contact_detail_type == 4 || cd.contact_detail_type == 5).Count();
        //            int countEmails2 = contactDetailsFromDirectRelationships.Where(cd => cd.contact_detail_type == 4 || cd.contact_detail_type == 5).Count();

        //            prospectingProperty.has_email = (countEmails1 + countEmails2) > 0;
        //            // Emails (primary)
        //            countEmails1 = contactDetailsFromCompanyRelationships.Where(cd => (cd.contact_detail_type == 4 || cd.contact_detail_type == 5) && cd.is_primary_contact).Count();
        //            countEmails2 = contactDetailsFromDirectRelationships.Where(cd => (cd.contact_detail_type == 4 || cd.contact_detail_type == 5) && cd.is_primary_contact).Count();

        //            prospectingProperty.has_primary_email = (countEmails1 + countEmails2) > 0;
        //            // Cell 
        //            int countCells1 = contactDetailsFromCompanyRelationships.Where(cd => cd.contact_detail_type == 3).Count();
        //            int countCells2 = contactDetailsFromDirectRelationships.Where(cd => cd.contact_detail_type == 3).Count();

        //            prospectingProperty.has_cell = (countCells1 + countCells2) > 0;
        //            // Cell (primary)
        //            countCells1 = contactDetailsFromCompanyRelationships.Where(cd => cd.contact_detail_type == 3 && cd.is_primary_contact).Count();
        //            countCells2 = contactDetailsFromDirectRelationships.Where(cd => cd.contact_detail_type == 3 && cd.is_primary_contact).Count();

        //            prospectingProperty.has_primary_cell = (countCells1 + countCells2) > 0;
        //            // Landline
        //            int countLandlines1 = contactDetailsFromCompanyRelationships.Where(cd => cd.contact_detail_type == 1 || cd.contact_detail_type == 2).Count();
        //            int countLandline2 = contactDetailsFromDirectRelationships.Where(cd => cd.contact_detail_type == 1 || cd.contact_detail_type == 2).Count();

        //            prospectingProperty.has_landline = (countLandlines1 + countLandline2) > 0;
        //            // Landline (primary)
        //            countLandlines1 = contactDetailsFromCompanyRelationships.Where(cd => (cd.contact_detail_type == 1 || cd.contact_detail_type == 2) && cd.is_primary_contact).Count();
        //            countLandline2 = contactDetailsFromDirectRelationships.Where(cd => (cd.contact_detail_type == 1 || cd.contact_detail_type == 2) && cd.is_primary_contact).Count();

        //            prospectingProperty.has_primary_landline = (countLandlines1 + countLandline2) > 0;

        //            prospecting.SubmitChanges();
        //        }
        //    }
        //}

        public static ActivityBundle LoadUserActivities()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                return LoadProspectingActivities(prospecting, null);
            }
        }

        // NB. might need to remove occurrences of find_area_id that use the prospecting_area_layer tbl.
        public static UserSuburb FindAreaId(GeoLocation location)
        {
            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://spatial.seeff.com//"); // This is the web application's root server address
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            //HttpContent content = new ObjectContent<GeoLocation>(location, jsonFormatter);
            //var resp = client.PostAsync("api/SeeffSpatialLookup/GetAreaId", content).Result;
            //int? areaId = resp.Content.ReadAsAsync<int?>().Result;

            //UserSuburb suburb = null;
            //if (areaId.HasValue)
            //{
            //    suburb = new UserSuburb
            //    {
            //        SuburbId = areaId.Value
            //    };

            //    using (var prospecting = new ProspectingDataContext())
            //    {
            //        var areaTarget = prospecting.prospecting_areas.FirstOrDefault(a => a.prospecting_area_id == areaId.Value);
            //        suburb.SuburbName = areaTarget != null ? areaTarget.area_name : "";
            //    }
            //}

            var spatial = new SpatialServiceReader();
            var spatialSuburb = spatial.GetSuburbFromID(location.Lat, location.Lng);
            var suburbID = (spatialSuburb != null && spatialSuburb.SeeffAreaID.HasValue) ? spatialSuburb.SeeffAreaID.Value : (int?)null;

            if (suburbID != null)
            {
                return new UserSuburb
                {
                    SuburbId = suburbID.Value,
                    SuburbName = spatialSuburb.AreaName
                };
            }

            return null;
        }

        public static List<ProspectingProperty> LoadProperties(int[] inputProperties)
        {
            UnlockCurrentProspectingRecord();
            List<ProspectingProperty> results = new List<ProspectingProperty>();

            using (var prospecting = new ProspectingDataContext())
            {
                string params_ = (from n in inputProperties select n.ToString()).Aggregate((s1, s2) => s1 + "," + s2);
                var queryResults = prospecting.ExecuteQuery<FlattenedPropertyRecord>(@"SELECT        pp.prospecting_property_id, pp.lightstone_property_id, pp.latitude, pp.longitude, pp.property_address, pp.street_or_unit_no, pp.seeff_area_id, pp.lightstone_id_or_ck_no, pp.lightstone_reg_date, pp.erf_no, 
                         pp.comments, pp.ss_name, pp.ss_number, pp.ss_id, pp.unit, pp.ss_door_number, pp.last_purch_price, pp.prospected, pp.farm_name, pp.portion_no, pp.lightstone_suburb, pp.ss_fh, pp.ss_unique_identifier, 
                         pp.latest_reg_date, pp.baths, pp.condition, pp.beds, pp.dwell_size, pp.erf_size, pp.garages, pp.pool, pp.receptions, pp.staff_accomodation, pp.studies, pp.parking_bays,
                         pp.has_cell, pp.has_primary_cell, pp.has_email, pp.has_primary_email, pp.has_landline, pp.has_primary_landline, pp.property_listing_id, pcp.contact_person_id, 
                         ppr.relationship_to_property, NULL AS 'relationship_to_company', NULL AS 'contact_company_id', pcp.firstname, pcp.surname, pcp.id_number, pcp.person_title, pcp.person_gender, pcp.comments_notes, 
                         pcp.is_popi_restricted, pcp.optout_emails, pcp.optout_sms, pcp.do_not_contact, pcp.email_contactability_status, pcp.age_group, pcp.bureau_adverse_indicator, pcp.citizenship, pcp.deceased_status, pcp.directorship, pcp.occupation, pcp.employer, 
                         pcp.physical_address, pcp.home_ownership, pcp.marital_status, pcp.location, pcd.contact_detail_type, pcd.prospecting_contact_detail_id, pcd.contact_detail, pcd.is_primary_contact, pcd.intl_dialing_code_id, 
                         padc.dialing_code_id, pcd.eleventh_digit, pcd.deleted
FROM            prospecting_property AS pp LEFT OUTER JOIN
                         prospecting_person_property_relationship AS ppr ON pp.prospecting_property_id = ppr.prospecting_property_id LEFT OUTER JOIN
                         prospecting_contact_person AS pcp ON pcp.contact_person_id = ppr.contact_person_id LEFT OUTER JOIN
                         prospecting_contact_detail AS pcd ON pcd.contact_person_id = pcp.contact_person_id LEFT OUTER JOIN
                         prospecting_area_dialing_code AS padc ON padc.prospecting_area_dialing_code_id = pcd.intl_dialing_code_id
WHERE        (pp.lightstone_property_id IN (" + params_ + @"))
UNION ALL
SELECT        pp.prospecting_property_id, pp.lightstone_property_id, pp.latitude, pp.longitude, pp.property_address, pp.street_or_unit_no, pp.seeff_area_id, pp.lightstone_id_or_ck_no, pp.lightstone_reg_date, pp.erf_no, 
                         pp.comments, pp.ss_name, pp.ss_number, pp.ss_id, pp.unit, pp.ss_door_number, pp.last_purch_price, pp.prospected, pp.farm_name, pp.portion_no, pp.lightstone_suburb, pp.ss_fh, pp.ss_unique_identifier, 
                         pp.latest_reg_date, pp.baths, pp.condition, pp.beds, pp.dwell_size, pp.erf_size, pp.garages, pp.pool, pp.receptions, pp.staff_accomodation, pp.studies, pp.parking_bays,
                         pp.has_cell, pp.has_primary_cell, pp.has_email, pp.has_primary_email, pp.has_landline, pp.has_primary_landline, pp.property_listing_id, pcp.contact_person_id, NULL 
                         AS 'relationship_to_property', ppcr.relationship_to_company, ppcr.contact_company_id, pcp.firstname, pcp.surname, pcp.id_number, pcp.person_title, pcp.person_gender, pcp.comments_notes, 
                         pcp.is_popi_restricted, pcp.optout_emails, pcp.optout_sms, pcp.do_not_contact, pcp.email_contactability_status, pcp.age_group, pcp.bureau_adverse_indicator, pcp.citizenship, pcp.deceased_status, pcp.directorship, pcp.occupation, pcp.employer, 
                         pcp.physical_address, pcp.home_ownership, pcp.marital_status, pcp.location, pcd.contact_detail_type, pcd.prospecting_contact_detail_id, pcd.contact_detail, pcd.is_primary_contact, pcd.intl_dialing_code_id, 
                         padc.dialing_code_id, pcd.eleventh_digit, pcd.deleted
FROM            prospecting_property AS pp LEFT OUTER JOIN
                         prospecting_company_property_relationship AS cpr ON pp.prospecting_property_id = cpr.prospecting_property_id LEFT OUTER JOIN
                         prospecting_person_company_relationship AS ppcr ON ppcr.contact_company_id = cpr.contact_company_id LEFT OUTER JOIN
                         prospecting_contact_person AS pcp ON pcp.contact_person_id = ppcr.contact_person_id LEFT OUTER JOIN
                         prospecting_contact_detail AS pcd ON pcd.contact_person_id = pcp.contact_person_id LEFT OUTER JOIN
                         prospecting_area_dialing_code AS padc ON padc.prospecting_area_dialing_code_id = pcd.intl_dialing_code_id
WHERE        (pp.lightstone_property_id IN (" + params_ + @"))", new object[] { });

                // Performance: tolist() or not?
                var propertyGroupings = queryResults.GroupBy(fpr => fpr.lightstone_property_id);
                // .Select(g => new { LightstonePropID = g.Key, Properties = g.Select(t => t) });
                foreach (var sameLightstoneIDGroup in propertyGroupings)
                {
                    var prospectingRecord = sameLightstoneIDGroup.First();
                    var latLng = new GeoLocation { Lat = prospectingRecord.latitude.Value, Lng = prospectingRecord.longitude.Value };
                    ProspectingProperty prop = new ProspectingProperty
                    {
                        ProspectingPropertyId = prospectingRecord.prospecting_property_id,
                        LightstonePropertyId = prospectingRecord.lightstone_property_id,
                        LatLng = latLng,
                        PropertyAddress = prospectingRecord.property_address,
                        StreetOrUnitNo = prospectingRecord.street_or_unit_no,
                        SeeffAreaId = prospectingRecord.seeff_area_id.HasValue ? prospectingRecord.seeff_area_id.Value : (int?)null,
                        LightstoneIDOrCKNo = prospectingRecord.lightstone_id_or_ck_no,
                        LightstoneRegDate = prospectingRecord.lightstone_reg_date,
                        ErfNo = prospectingRecord.erf_no,
                        Comments = prospectingRecord.comments,
                        //SS_FH = prospectingRecord.ss_fh == "SS" ? "SS" : "FH", // default to FH for backward compat.
                        SSName = prospectingRecord.ss_name,
                        SSNumber = prospectingRecord.ss_number,
                        SS_ID = prospectingRecord.ss_id,
                        Unit = prospectingRecord.unit,
                        SSDoorNo = prospectingRecord.ss_door_number,
                        LastPurchPrice = prospectingRecord.last_purch_price,
                        Prospected = Convert.ToBoolean(prospectingRecord.prospected),
                        FarmName = prospectingRecord.farm_name,
                        Portion = prospectingRecord.portion_no != null ? prospectingRecord.portion_no.ToString() : "n/a",
                        LightstoneSuburb = prospectingRecord.lightstone_suburb,
                        SS_UNIQUE_IDENTIFIER = prospectingRecord.ss_unique_identifier,
                        LatestRegDateForUpdate = prospectingRecord.latest_reg_date,

                        Baths = prospectingRecord.baths,
                        Condition = prospectingRecord.condition,
                        Beds = prospectingRecord.beds,
                        DwellingSize = prospectingRecord.dwell_size,
                        ErfSize = prospectingRecord.erf_size,
                        Garages = prospectingRecord.garages,
                        Pool = prospectingRecord.pool,
                        Receptions = prospectingRecord.receptions,
                        StaffAccomodation = prospectingRecord.staff_accomodation,
                        Studies = prospectingRecord.studies,
                        ParkingBays = prospectingRecord.parking_bays,

                        IsShortTermRental = prospectingRecord.is_short_term_rental,
                        IsLongTermRental = prospectingRecord.is_long_term_rental,
                        IsCommercial = prospectingRecord.is_commercial,
                        IsAgricultural = prospectingRecord.is_agricultural,
                        IsInvestment = prospectingRecord.is_investment,

                        HasContactWithCell = prospectingRecord.has_cell,
                        HasContactWithPrimaryCell = prospectingRecord.has_primary_cell,

                        HasContactWithEmail = prospectingRecord.has_email,
                        HasContactWithPrimaryEmail = prospectingRecord.has_primary_email,

                        HasContactWithLandline = prospectingRecord.has_landline,
                        HasContactWithPrimaryLandline = prospectingRecord.has_primary_landline,

                        PropertyListingId = prospectingRecord.property_listing_id
                    };
                    switch (prospectingRecord.ss_fh)
                    {
                        case "SS": prop.SS_FH = "SS"; break;
                        case "FH": prop.SS_FH = "FH"; break;
                        case "FS": prop.SS_FH = "FS"; break;
                        case "FRM": prop.SS_FH = "FRM"; break;
                        default: prop.SS_FH = "FH"; break;
                    }

                    // contacts grouping
                    var contactsGroupings = sameLightstoneIDGroup.GroupBy(pp => pp.contact_person_id);
                    List<ProspectingContactPerson> propertyContacts = new List<ProspectingContactPerson>();
                    foreach (var sameContactPersonIdGroup in contactsGroupings)
                    {
                        if (sameContactPersonIdGroup.Key == null)
                            continue;
                        // Dealing with a single contact now
                        var contactRecord = sameContactPersonIdGroup.First();
                        List<KeyValuePair<int, int>> personPropertyrelationships = new List<KeyValuePair<int, int>>();
                        if (contactRecord.relationship_to_property.HasValue)
                        {
                            personPropertyrelationships.Add(new KeyValuePair<int, int>(contactRecord.prospecting_property_id, contactRecord.relationship_to_property.Value));
                        }
                        var newContactPerson = new ProspectingContactPerson
                        {
                            ContactPersonId = contactRecord.contact_person_id,
                            PersonPropertyRelationships = personPropertyrelationships,
                            PersonCompanyRelationshipType = contactRecord.relationship_to_company,
                            ContactCompanyId = contactRecord.contact_company_id,
                            Firstname = contactRecord.firstname,
                            Surname = contactRecord.surname,
                            IdNumber = contactRecord.id_number,
                            Title = contactRecord.person_title,
                            Gender = contactRecord.person_gender,
                            Comments = contactRecord.comments_notes,
                            IsPOPIrestricted = contactRecord.is_popi_restricted.HasValue ? contactRecord.is_popi_restricted.Value : false,
                            PropertiesOwned = null,
                            EmailOptout = contactRecord.optout_emails.HasValue ? contactRecord.optout_emails.Value : false,
                            SMSOptout = contactRecord.optout_sms.HasValue ? contactRecord.optout_sms.Value : false,
                            DoNotContact = contactRecord.do_not_contact.HasValue ? contactRecord.do_not_contact.Value : false,
                            EmailContactabilityStatus = contactRecord.email_contactability_status.HasValue ? contactRecord.email_contactability_status.Value : 1,

                            // Dracore fields
                            AgeGroup = contactRecord.age_group,
                            BureauAdverseIndicator = contactRecord.bureau_adverse_indicator,
                            Citizenship = contactRecord.citizenship,
                            DeceasedStatus = contactRecord.deceased_status,
                            Directorship = contactRecord.directorship,
                            Occupation = contactRecord.occupation,
                            Employer = contactRecord.employer,
                            PhysicalAddress = contactRecord.physical_address,
                            HomeOwnership = contactRecord.home_ownership,
                            MaritalStatus = contactRecord.marital_status,
                            Location = contactRecord.location
                        };

                        var contactDetailGroupings = sameContactPersonIdGroup.GroupBy(cp => cp.contact_detail_type);
                        List<ProspectingContactDetail> phoneNumbers = new List<ProspectingContactDetail>();
                        List<ProspectingContactDetail> emaiLAddresses = new List<ProspectingContactDetail>();
                        foreach (var sameContactDetailTypeGroup in contactDetailGroupings)
                        {
                            foreach (var contactDetailType in sameContactDetailTypeGroup)
                            {
                                if (contactDetailType.contact_detail_type.HasValue && contactDetailType.deleted == false)
                                {
                                    if (ProspectingLookupData.PhoneTypeIds.Contains(contactDetailType.contact_detail_type.Value))
                                    {
                                        phoneNumbers.Add(new ProspectingContactDetail
                                        {
                                            ItemId = contactDetailType.prospecting_contact_detail_id.ToString(),
                                            ContactItemType = "PHONE",
                                            ItemContent = contactDetailType.contact_detail,
                                            IsPrimary = contactDetailType.is_primary_contact,
                                            ItemType = contactDetailType.contact_detail_type,
                                            IsValid = true,
                                            IntDialingCode = contactDetailType.intl_dialing_code_id,
                                            IntDialingCodePrefix = contactDetailType.dialing_code_id,
                                            EleventhDigit = contactDetailType.eleventh_digit
                                        });
                                    }
                                    else if (ProspectingLookupData.EmailTypeIds.Contains(contactDetailType.contact_detail_type.Value))
                                    {
                                        emaiLAddresses.Add(new ProspectingContactDetail
                                        {
                                            ItemId = contactDetailType.prospecting_contact_detail_id.ToString(),
                                            ContactItemType = "EMAIL",
                                            ItemContent = contactDetailType.contact_detail,
                                            IsPrimary = contactDetailType.is_primary_contact,
                                            ItemType = contactDetailType.contact_detail_type,
                                            IsValid = true,
                                            IntDialingCode = contactDetailType.intl_dialing_code_id,
                                            EleventhDigit = contactDetailType.eleventh_digit
                                        });
                                    }
                                }
                            }
                        }
                        newContactPerson.PhoneNumbers = phoneNumbers;
                        newContactPerson.EmailAddresses = emaiLAddresses;
                        propertyContacts.Add(newContactPerson);
                    }
                    prop.Contacts = propertyContacts;
                    results.Add(prop);
                }

                return results;
            }

            IEnumerable<List<int>> sets = inputProperties.Partition<int>(20);

            List<System.Threading.Tasks.Task<List<ProspectingProperty>>> tasks = new List<System.Threading.Tasks.Task<List<ProspectingProperty>>>();
            foreach (var item in sets)
            {
                tasks.Add(Task<List<ProspectingProperty>>.Factory.StartNew(() => { return GetPropertiesTask(item); }));
            }

            Task.WaitAll(tasks.ToArray());

            foreach (Task<List<ProspectingProperty>> item in tasks)
            {
                results.AddRange(item.Result);
            }

            return results;
        }

        public static List<ProspectingContactPerson> LoadContacts(int[] inputProperties)
        {
            List<ProspectingContactPerson> results = new List<ProspectingContactPerson>();
            using (var prospecting = new ProspectingDataContext())
            {
                string params_ = (from n in inputProperties select n.ToString()).Aggregate((s1, s2) => s1 + "," + s2);
                var queryResults = prospecting.ExecuteQuery<FlattenedPropertyRecord>(@"SELECT        pp.prospecting_property_id, pp.lightstone_property_id, pp.latitude, pp.longitude, pp.property_address, pp.street_or_unit_no, pp.seeff_area_id, pp.lightstone_id_or_ck_no, pp.lightstone_reg_date, pp.erf_no, 
                         pp.comments, pp.ss_name, pp.ss_number, pp.ss_id, pp.unit, pp.ss_door_number, pp.last_purch_price, pp.prospected, pp.farm_name, pp.portion_no, pp.lightstone_suburb, pp.ss_fh, pp.ss_unique_identifier, 
                         pp.latest_reg_date, pp.baths, pp.condition, pp.beds, pp.dwell_size, pp.erf_size, pp.garages, pp.pool, pp.receptions, pp.staff_accomodation, pp.studies, pp.parking_bays,
                         pp.has_cell, pp.has_primary_cell, pp.has_email, pp.has_primary_email, pp.has_landline, pp.has_primary_landline, pcp.contact_person_id, 
                         ppr.relationship_to_property, NULL AS 'relationship_to_company', NULL AS 'contact_company_id', pcp.firstname, pcp.surname, pcp.id_number, pcp.person_title, pcp.person_gender, pcp.comments_notes, 
                         pcp.is_popi_restricted, pcp.optout_emails, pcp.optout_sms, pcp.do_not_contact, pcp.email_contactability_status, pcp.age_group, pcp.bureau_adverse_indicator, pcp.citizenship, pcp.deceased_status, pcp.directorship, pcp.occupation, pcp.employer, 
                         pcp.physical_address, pcp.home_ownership, pcp.marital_status, pcp.location
FROM            prospecting_property AS pp LEFT OUTER JOIN
                         prospecting_person_property_relationship AS ppr ON pp.prospecting_property_id = ppr.prospecting_property_id LEFT OUTER JOIN
                         prospecting_contact_person AS pcp ON pcp.contact_person_id = ppr.contact_person_id
WHERE        (pp.prospecting_property_id IN (" + params_ + @"))
UNION ALL
SELECT        pp.prospecting_property_id, pp.lightstone_property_id, pp.latitude, pp.longitude, pp.property_address, pp.street_or_unit_no, pp.seeff_area_id, pp.lightstone_id_or_ck_no, pp.lightstone_reg_date, pp.erf_no, 
                         pp.comments, pp.ss_name, pp.ss_number, pp.ss_id, pp.unit, pp.ss_door_number, pp.last_purch_price, pp.prospected, pp.farm_name, pp.portion_no, pp.lightstone_suburb, pp.ss_fh, pp.ss_unique_identifier, 
                         pp.latest_reg_date, pp.baths, pp.condition, pp.beds, pp.dwell_size, pp.erf_size, pp.garages, pp.pool, pp.receptions, pp.staff_accomodation, pp.studies, pp.parking_bays,
                         pp.has_cell, pp.has_primary_cell, pp.has_email, pp.has_primary_email, pp.has_landline, pp.has_primary_landline, pcp.contact_person_id, NULL 
                         AS 'relationship_to_property', ppcr.relationship_to_company, ppcr.contact_company_id, pcp.firstname, pcp.surname, pcp.id_number, pcp.person_title, pcp.person_gender, pcp.comments_notes, 
                         pcp.is_popi_restricted, pcp.optout_emails, pcp.optout_sms, pcp.do_not_contact, pcp.email_contactability_status, pcp.age_group, pcp.bureau_adverse_indicator, pcp.citizenship, pcp.deceased_status, pcp.directorship, pcp.occupation, pcp.employer, 
                         pcp.physical_address, pcp.home_ownership, pcp.marital_status, pcp.location
FROM            prospecting_property AS pp LEFT OUTER JOIN
                         prospecting_company_property_relationship AS cpr ON pp.prospecting_property_id = cpr.prospecting_property_id LEFT OUTER JOIN
                         prospecting_person_company_relationship AS ppcr ON ppcr.contact_company_id = cpr.contact_company_id LEFT OUTER JOIN
                         prospecting_contact_person AS pcp ON pcp.contact_person_id = ppcr.contact_person_id
WHERE        (pp.prospecting_property_id IN (" + params_ + @"))", new object[] { });

                var propertyGroupings = queryResults.GroupBy(fpr => fpr.lightstone_property_id);
                foreach (var sameLightstoneIDGroup in propertyGroupings)
                {
                    var lightstoneProperty = sameLightstoneIDGroup.First();  
                    // contacts grouping
                    var contactsGroupings = sameLightstoneIDGroup.GroupBy(pp => pp.contact_person_id);
                    List<ProspectingContactPerson> propertyContacts = new List<ProspectingContactPerson>();
                    foreach (var sameContactPersonIdGroup in contactsGroupings)
                    {
                        if (sameContactPersonIdGroup.Key == null)
                            continue;
                        // Dealing with a single contact now
                        var contactRecord = sameContactPersonIdGroup.First();
                        List<KeyValuePair<int, int>> personPropertyrelationships = new List<KeyValuePair<int, int>>();
                        if (contactRecord.relationship_to_property.HasValue)
                        {
                            personPropertyrelationships.Add(new KeyValuePair<int, int>(contactRecord.prospecting_property_id, contactRecord.relationship_to_property.Value));
                        }
                        var newContactPerson = new ProspectingContactPerson
                        {
                            ContactPersonId = contactRecord.contact_person_id,
                            PersonPropertyRelationships = personPropertyrelationships,
                            PersonCompanyRelationshipType = contactRecord.relationship_to_company,
                            ContactCompanyId = contactRecord.contact_company_id,
                            Firstname = contactRecord.firstname,
                            Surname = contactRecord.surname,
                            IdNumber = contactRecord.id_number,
                            Title = contactRecord.person_title,
                            Gender = contactRecord.person_gender,
                            Comments = contactRecord.comments_notes,
                            IsPOPIrestricted = contactRecord.is_popi_restricted.HasValue ? contactRecord.is_popi_restricted.Value : false,
                            PropertiesOwned = null,
                            EmailOptout = contactRecord.optout_emails.HasValue ? contactRecord.optout_emails.Value : false,
                            SMSOptout = contactRecord.optout_sms.HasValue ? contactRecord.optout_sms.Value : false,
                            DoNotContact = contactRecord.do_not_contact.HasValue ? contactRecord.do_not_contact.Value : false,
                            EmailContactabilityStatus = contactRecord.email_contactability_status.HasValue ? contactRecord.email_contactability_status.Value : 1,

                            TargetLightstonePropertyIdForComms = lightstoneProperty.lightstone_property_id,
                            TargetProspectingPropertyId = lightstoneProperty.prospecting_property_id,

                            // Dracore fields
                            AgeGroup = contactRecord.age_group,
                            BureauAdverseIndicator = contactRecord.bureau_adverse_indicator,
                            Citizenship = contactRecord.citizenship,
                            DeceasedStatus = contactRecord.deceased_status,
                            Directorship = contactRecord.directorship,
                            Occupation = contactRecord.occupation,
                            Employer = contactRecord.employer,
                            PhysicalAddress = contactRecord.physical_address,
                            HomeOwnership = contactRecord.home_ownership,
                            MaritalStatus = contactRecord.marital_status,
                            Location = contactRecord.location
                        };
           
                        propertyContacts.Add(newContactPerson);
                    }
                    results.AddRange(propertyContacts);
                }

                return results;
            }            
        }

        public static List<ProspectingProperty> GetPropertiesTask(List<int> items)
        {
            using (var prospectingDB = new ProspectingDataContext())
            {
                List<ProspectingProperty> results = new List<ProspectingProperty>();
                foreach (int lightstonePropertyId in items)
                {
                    var propRecord = prospectingDB.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropertyId);
                    ProspectingProperty propertyWithContacts = CreateProspectingProperty(prospectingDB, propRecord, true, false, false, false);
                    results.Add(propertyWithContacts);
                }
                return results;
            }
        }

        public static string GetFormattedAddress(int lightstonePropertyId)
        {
            using (var prospectingContext = new ProspectingDataContext())
            {
                var property = prospectingContext.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropertyId);
                if (property.ss_fh == "SS" || property.ss_fh == "FS")
                {
                    if (!string.IsNullOrEmpty(property.ss_door_number))
                    {
                        return "Unit " + property.unit + " (Door no.: " + property.ss_door_number + ") " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.ss_name.ToLower()).Replace("Ss ", "SS ");
                    }

                    return "Unit " + property.unit + " " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.ss_name.ToLower()).Replace("Ss ", "SS ");
                }

                return property.street_or_unit_no + " " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.property_address.ToLower());
            }
        }

        public static string GetFormattedAddress(ProspectingProperty property)
        {
            Func<string, string> encloseInQuotes = pr => "\"" + pr + "\"";

            if (property.SS_FH == "SS" || property.SS_FH == "FS")
            {
                string ssAddress = "";
                if (!string.IsNullOrEmpty(property.SSDoorNo))
                {
                    ssAddress = "Unit " + property.Unit + " (Door no.: " + property.SSDoorNo + ") " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.SSName.ToLower()).Replace("Ss ", "SS ");
                }

                ssAddress = "Unit " + property.Unit + " " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.SSName.ToLower()).Replace("Ss ", "SS ");

                if (!string.IsNullOrEmpty(property.StreetOrUnitNo) && property.StreetOrUnitNo != "n/a")
                {
                    ssAddress = encloseInQuotes(string.Concat(ssAddress, ", ", property.StreetOrUnitNo + " " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.PropertyAddress.ToLower())));
                }
                else
                {
                    ssAddress = encloseInQuotes(ssAddress);
                }

                return ssAddress;
            }

            return encloseInQuotes(property.StreetOrUnitNo + " " + new CultureInfo("en-US", false).TextInfo.ToTitleCase(property.PropertyAddress.ToLower()));
        }

        private static void CreateActivityForPropertyChangeOfOwnership(ProspectingDataContext prospecting, prospecting_property propertyRecord)
        {
            List<ProspectingContactPerson> allPropertyContacts = new List<ProspectingContactPerson>();

            var propertyContacts = ProspectingLookupData.PropertyContactsRetriever(prospecting, propertyRecord, false).ToList();
            var propertyCompanyContacts = ProspectingLookupData.PropertyCompanyContactsRetriever(prospecting, propertyRecord, false).ToList();
            allPropertyContacts.AddRange(propertyContacts);
            allPropertyContacts.AddRange(propertyCompanyContacts);
            allPropertyContacts.Distinct();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetFormattedAddress(propertyRecord.lightstone_property_id));
            if (!string.IsNullOrWhiteSpace(propertyRecord.lightstone_reg_date))
            {
                sb.AppendLine("Previous reg. date on record: " + FormatRegDate(propertyRecord.lightstone_reg_date));
            }
            if (propertyRecord.last_purch_price != null)
            {
                sb.AppendLine("Previous sale price on record: " + FormatSalePrice(propertyRecord.last_purch_price));
            }
            sb.AppendLine();
            sb.AppendLine("Details of owner(s) before update:");
            if (allPropertyContacts.Count > 0)
            {
                foreach (var contact in allPropertyContacts)
                {
                    string contactLine = contact.Fullname + " ID:" + contact.IdNumber;
                    var phoneNumbers = ProspectingLookupData.PropertyContactPhoneNumberRetriever(prospecting, contact);
                    var primaryPhoneNumber = phoneNumbers.FirstOrDefault(ph => ph.IsPrimary == true);
                    if (primaryPhoneNumber != null)
                    {
                        contactLine += " | PH: " + primaryPhoneNumber.ItemContent;
                    }

                    var emails = ProspectingLookupData.PropertyContactEmailRetriever(prospecting, contact);
                    var primaryEmail = emails.FirstOrDefault(em => em.IsPrimary == true);
                    if (primaryEmail != null)
                    {
                        contactLine += " | EMAIL: " + primaryEmail.ItemContent;
                    }

                    sb.AppendLine(contactLine);
                }
            }
            else
            {
                sb.AppendLine("No previous contact people associated with this property.");
            }

            var activityType = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "Change Of Ownership").Key;
            var activityRecord = new activity_log
            {
                lightstone_property_id = propertyRecord.lightstone_property_id,
                followup_date = null,
                allocated_to = null,
                activity_type_id = activityType,
                comment = sb.ToString(),
                created_by = RequestHandler.GetUserSessionObject().UserGuid,
                created_date = DateTime.Now,
                contact_person_id = null,
                // Add the rest later
                parent_activity_id = null,
                activity_followup_type_id = null
            };
            prospecting.activity_logs.InsertOnSubmit(activityRecord);
            prospecting.SubmitChanges();
        }

        private static string FormatSalePrice(decimal? purchPrice)
        {
            return purchPrice != null ? String.Format(new CultureInfo("en-ZA"), "{0:C0}", purchPrice.Value) : "";
        }

        private static string FormatRegDate(string regDate)
        {
            long result;
            if (!string.IsNullOrWhiteSpace(regDate) && regDate.Length == 8 && long.TryParse(regDate, out result))
            {
                return regDate.Substring(0, 4) + "-" + regDate.Substring(4, 2) + "-" + regDate.Substring(6, 2);
            }

            return regDate;
        }

        public static bool UpdatePropertyOwnership(ProspectingPropertyId property)
        {
            LightstonePropertyMatch propertyMatch = null;
            prospecting_property propertyRecord = null;

            var searchResult = FindMatchingProperties(new SearchInputPacket { PropertyID = property.LightstonePropertyId.ToString() }).FirstOrDefault();
            if (searchResult == null)
            {
                searchResult = FindMatchingProperties(new SearchInputPacket { PropertyID = property.LightstonePropertyId.ToString() }).FirstOrDefault();
                if (searchResult == null)
                {
                    //throw new Exception("Error updating property record, FindMatchingProperties returns null for this Lightstone Property ID: " + property.LightstonePropertyId);
                    return false;
                }
            }

            if (searchResult.PropertyMatches == null || searchResult.PropertyMatches.Count != 1)
            {
                return false;
            }

            if (searchResult.PropertyMatches[0].LightstonePropId != property.LightstonePropertyId)
            {
                return false;
            }
            using (var prospecting = new ProspectingDataContext())
            {
                // Find and delete existing relationships between this property and contacts and companies (if any)
                propertyRecord = prospecting.prospecting_properties.First(pp => pp.lightstone_property_id == property.LightstonePropertyId);

                // Verify that the record has not already been updated in another session
                //if (propertyRecord.latest_reg_date == null )
                //{
                //    return false;
                //}

                CreateActivityForPropertyChangeOfOwnership(prospecting, propertyRecord);

                var oldOwners = propertyRecord.prospecting_person_property_relationships.Select(cp => cp.prospecting_contact_person).ToList();

                prospecting.prospecting_company_property_relationships.DeleteAllOnSubmit(propertyRecord.prospecting_company_property_relationships);
                prospecting.prospecting_person_property_relationships.DeleteAllOnSubmit(propertyRecord.prospecting_person_property_relationships);
                prospecting.SubmitChanges();

                // Re-prospect
                propertyMatch = searchResult.PropertyMatches[0];
                propertyRecord.updated_date = DateTime.Now;
                propertyRecord.last_purch_price = !string.IsNullOrEmpty(propertyMatch.PurchPrice) ? decimal.Parse(propertyMatch.PurchPrice, CultureInfo.InvariantCulture) : (decimal?)null;
                propertyRecord.lightstone_reg_date = propertyMatch.RegDate;
                propertyRecord.latest_reg_date = null;

                // 
                if (property.DeleteOldContactsFromMyLists == true)
                {
                    // Compare old and new owners and if they differ - remove old contacts from any lists they belong to which THIS user has access:
                    List<prospecting_contact_person> contactsEligibleForRemoval = new List<prospecting_contact_person>();
                    // If the old contact record doesn't exist in the new contacts list it's eligible for deletion from lists
                    foreach (var oldOwner in oldOwners)
                    {
                        bool contactCanBeRemoved = true;
                        foreach (var newOwner in propertyMatch.Owners)
                        {
                            if (newOwner.ContactEntityType == ContactEntityType.NaturalPerson)
                            {
                                var newContact = (ProspectingContactPerson)newOwner;
                                if (oldOwner.id_number == newContact.IdNumber)
                                {
                                    contactCanBeRemoved = false;
                                }
                            }
                        }
                        if (contactCanBeRemoved)
                        {
                            contactsEligibleForRemoval.Add(oldOwner);
                        }
                    }
                    foreach (var item in contactsEligibleForRemoval)
                    {
                        SaveListAllocationForContact(new ContactListSelection { ContactPersonId = item.contact_person_id, ListAllocation = new List<ListSelected>() });
                    }
                }

                prospecting.SubmitChanges();
            }

            foreach (var owner in propertyMatch.Owners)
            {
                if (owner.ContactEntityType == ContactEntityType.NaturalPerson)
                {
                    var newContact = new ContactDataPacket { ContactPerson = (ProspectingContactPerson)owner, ProspectingPropertyId = propertyRecord.prospecting_property_id, ContactCompanyId = null };
                    SaveContactPerson(newContact);
                }
                if (owner.ContactEntityType == ContactEntityType.JuristicEntity)
                {
                    var newContact = new CompanyContactDataPacket { ContactCompany = (ProspectingContactCompany)owner, ProspectingPropertyId = propertyRecord.prospecting_property_id };
                    SaveContactCompany(newContact);
                }
            }

            return true;
        }

        public static void CreateValuation(PropertyValuation valuation)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                long? activityRecordId = null;
                var propertyRecord = prospecting.prospecting_properties.First(pp => pp.prospecting_property_id == valuation.ProspectingPropertyId);
                if (valuation.CreateActivity)
                {
                    string comment = "A new valuation has been created for this property:" + Environment.NewLine +
                                    "New value: R" + valuation.Value + Environment.NewLine +
                                    "Valuation date: " + valuation.ValuationDate + Environment.NewLine +
                                    "Is this its current Lightstone value? " + (valuation.IsCurrentValue ? "Yes" : "No");

                    var activityType = ProspectingLookupData.ActivityTypes.First(act => act.Value == "Valuation Done").Key;
                    var activityRecord = new activity_log
                    {
                        lightstone_property_id = propertyRecord.lightstone_property_id,
                        followup_date = null,
                        allocated_to = null,
                        activity_type_id = activityType,
                        comment = comment,
                        created_by = RequestHandler.GetUserSessionObject().UserGuid,
                        created_date = DateTime.Now,
                        contact_person_id = null,
                        // Add the rest later
                        parent_activity_id = null,
                        activity_followup_type_id = null
                    };
                    prospecting.activity_logs.InsertOnSubmit(activityRecord);
                    prospecting.SubmitChanges();
                    activityRecordId = activityRecord.activity_log_id;

                    if (valuation.CreateFollowup)
                    {
                        ProspectingActivity followupActivity = new ProspectingActivity();
                        int followUpTypeId = ProspectingLookupData.ActivityFollowupTypes.First(aft => aft.Value == "Valuation Complete - Follow Up").Key;
                        followupActivity.IsForInsert = true;
                        followupActivity.ActivityFollowupTypeId = followUpTypeId;
                        followupActivity.Comment = valuation.Comment;
                        followupActivity.ContactPersonId = valuation.RelatedTo > 0 ? valuation.RelatedTo : (int?)null;
                        followupActivity.FollowUpDate = valuation.FollowupDate;
                        followupActivity.LightstonePropertyId = propertyRecord.lightstone_property_id;
                        followupActivity.ParentActivityId = activityRecordId;
                        followupActivity.ActivityTypeId = activityType;
                        UpdateInsertActivity(followupActivity);
                    }
                }

                property_valuation newValuation = new property_valuation
                {
                    prospecting_property_id = propertyRecord.prospecting_property_id,
                    created_date = DateTime.Now,
                    created_by_user_guid = RequestHandler.GetUserSessionObject().UserGuid,
                    activity_log_id = activityRecordId,
                    value_estimate = valuation.Value,
                    date_valued = valuation.ValuationDate,
                    current_value = valuation.IsCurrentValue
                };

                prospecting.property_valuations.InsertOnSubmit(newValuation);
                prospecting.SubmitChanges();
            }
        }

        public static List<PropertyValuation> LoadValuations(int prospectingPropertyId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
                var valuations = (from valuation in prospecting.property_valuations
                                  where valuation.prospecting_property_id == prospectingPropertyId && !valuation.deleted
                                  orderby valuation.date_valued descending
                                  select new PropertyValuation
                                  {
                                      Value = valuation.value_estimate,
                                      ValuationDate = valuation.date_valued,
                                      CreatedByGuid = valuation.created_by_user_guid,
                                      ValuationRecordId = valuation.property_valuation_id
                                  }).ToList();

                foreach (var val in valuations)
                {
                    var createdBy = user.BusinessUnitUsers.FirstOrDefault(s => s.UserGuid == val.CreatedByGuid);
                    val.CreatedByUsername = createdBy != null ? createdBy.Fullname : "";
                }

                return valuations;
            }
        }

        public static void DeleteValuation(PropertyValuation valuation)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var target = prospecting.property_valuations.FirstOrDefault(val => val.property_valuation_id == valuation.ValuationRecordId);
                if (target != null)
                {
                    target.deleted = true;
                    target.date_deleted = DateTime.Now;
                    target.deleted_by = RequestHandler.GetUserSessionObject().UserGuid;
                    prospecting.SubmitChanges();
                }
            }
        }

        public static CompanyEnquiryResponsePacket PerformCompanyEnquiry(CompanyEnquiryInputPacket enquiryPacket)
        {
            prospecting_contact_company prospectingTargetCompany;
            using (var prospecting = new ProspectingDataContext())
            {
                prospectingTargetCompany = prospecting.prospecting_contact_companies.First(cc => cc.contact_company_id == enquiryPacket.ContactCompanyId);
            }

            CompanyEnquiryResponsePacket results = new CompanyEnquiryResponsePacket();

            decimal? walletBalance = null;
            // Variables to keep track of the state of the transaction
            bool enquirySuccessful = false, deductionMade = false, deductionReimbursed = false;
            ICompanyEnquiryService companyService = null;
            try
            {
                companyService = new DracoreCompanyLookupService(prospectingTargetCompany, enquiryPacket.ProspectingPropertyId);
                companyService.InitResponsePacket(results);
                walletBalance = companyService.DeductEnquiryCost(); // NB: Will return a value < 0 if insufficient funds
                if (walletBalance >= decimal.Zero)
                {
                    deductionMade = true;
                    companyService.DoEnquiry();
                    if (results.EnquirySuccessful && string.IsNullOrEmpty(results.ErrorMsg))
                    {
                        enquirySuccessful = true;
                    }
                    else
                    {
                        walletBalance = companyService.ReverseEnquiryCost();
                        deductionReimbursed = true;
                    }
                    results.WalletBalance = walletBalance;
                }
                else
                {
                    results.ErrorMsg = "Insufficient funds to perform enquiry.";
                }
            }
            catch (Exception ex)
            {
                results.ErrorMsg = "Error occurred performing enquiry: " + ex.Message;
                if (companyService != null)
                {
                    companyService.SetError(ex);
                }
            }
            finally
            {
                // Double check here that if enquiry failed we do not accidently bill the user.
                if (!enquirySuccessful && deductionMade && !deductionReimbursed)
                {
                    results.WalletBalance = companyService.ReverseEnquiryCost();
                }
                if (companyService != null)
                {
                    companyService.LogEnquiry();
                }
            }

            return results;
        }

        public static TrustEnquiryResultPacket PerformTrustSearch(CompanyEnquiryInputPacket inputPacket)
        {
            prospecting_contact_company prospectingTargetCompany;
            using (var prospecting = new ProspectingDataContext())
            {
                prospectingTargetCompany = prospecting.prospecting_contact_companies.First(cc => cc.contact_company_id == inputPacket.ContactCompanyId);
            }

            TrustEnquiryResultPacket results = new TrustEnquiryResultPacket();
            results.CurrentInfo = inputPacket;

            TrustEntityEnquiryService lookupService = null;
            HttpContext.Current.Session["trust_search_results"] = null;
            try
            {
                lookupService = new TrustEntityEnquiryService(prospectingTargetCompany, inputPacket.ProspectingPropertyId);
                lookupService.InitResponsePacket(results);
                lookupService.DoEnquiry();
                HttpContext.Current.Session["trust_search_results"] = results;
            }
            catch (Exception ex)
            {
                results.ErrorMsg = "Error occurred performing enquiry: " + ex.Message;
                if (lookupService != null)
                {
                    lookupService.SetError(ex);
                }
            }
            finally
            {
                if (lookupService != null)
                {
                    lookupService.LogEnquiry();
                }
            }

            return results;
        }

        public static CompanyEnquiryResponsePacket GetTrustees(TrustHashcodeInput trustHashcode)
        {
            TrustEnquiryResultPacket searchResults = HttpContext.Current.Session["trust_search_results"] as TrustEnquiryResultPacket;
            if (searchResults == null)
            {
                return new CompanyEnquiryResponsePacket
                {
                    EnquirySuccessful = false,
                    ErrorMsg = "Your session has expired for the current search request. Please log in again and retry."
                };
            }

            TrustSearchResult targetTrust = searchResults.Results.First(tr => tr.id == trustHashcode.TrustHashcode);
            CompanyEnquiryResponsePacket results = new CompanyEnquiryResponsePacket();

            decimal? walletBalance = null;
            // Variables to keep track of the state of the transaction
            bool enquirySuccessful = false, deductionMade = false, deductionReimbursed = false;
            ICompanyEnquiryService trustService = null;
            try
            {
                trustService = new TrustEnquiryServiceGetTrustees(targetTrust);
                trustService.InitResponsePacket(results);
                walletBalance = trustService.DeductEnquiryCost(); // NB: Will return a value < 0 if insufficient funds
                if (walletBalance >= decimal.Zero)
                {
                    deductionMade = true;
                    trustService.DoEnquiry();
                    if (results.EnquirySuccessful && string.IsNullOrEmpty(results.ErrorMsg))
                    {
                        enquirySuccessful = true;
                    }
                    else
                    {
                        walletBalance = trustService.ReverseEnquiryCost();
                        deductionReimbursed = true;
                    }
                    results.WalletBalance = walletBalance;
                }
                else
                {
                    results.ErrorMsg = "Insufficient funds to perform enquiry.";
                }
            }
            catch (Exception ex)
            {
                results.ErrorMsg = "Error occurred performing enquiry: " + ex.Message;
                if (trustService != null)
                {
                    trustService.SetError(ex);
                }
            }
            finally
            {
                // Double check here that if enquiry failed we do not accidently bill the user.
                if (!enquirySuccessful && deductionMade && !deductionReimbursed)
                {
                    results.WalletBalance = trustService.ReverseEnquiryCost();
                }
                if (trustService != null)
                {
                    trustService.LogEnquiry();
                }
            }

            return results;
        }

        public static void UpdateStatisticsForSuburb(int suburbId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                prospecting.update_property_contact_detail_statistics(suburbId);
            }
        }

        public static CommReportResults LoadCommunicationData(CommReportFilters filterPacket)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                CommReportResults results = new CommReportResults();
                results.EmailLogItems = new List<SentEmailLogItem>();
                results.SMSLogItems = new List<SentSMSLogItem>();
                switch (filterPacket.MessageType)
                {
                    case "EMAIL":
                        IEnumerable<email_communications_log> emailResults = from log in prospecting.email_communications_logs select log;
                        if (filterPacket.SentByMe)
                        {
                            var userGuid = RequestHandler.GetUserSessionObject().UserGuid;
                            emailResults = emailResults.Where(em => em.created_by_user_guid == userGuid);
                        }
                        else
                        {
                            var businessUnitUsers = RequestHandler.GetUserSessionObject().BusinessUnitUsers;
                            var userGuids = from u in businessUnitUsers select u.UserGuid;
                            emailResults = emailResults.Where(em => userGuids.Contains(em.created_by_user_guid));
                        }
                        if (!string.IsNullOrWhiteSpace(filterPacket.BatchName))
                        {
                            emailResults = emailResults.Where(em =>
                            {
                                if (!string.IsNullOrEmpty(em.batch_friendly_name))
                                {
                                    if (em.batch_friendly_name.ToLower().Contains(filterPacket.BatchName.ToLower()))
                                        return true;
                                }
                                return false;
                            });
                        }
                        if (filterPacket.FromDate.HasValue)
                        {
                            emailResults = emailResults.Where(em => em.created_datetime.Date >= filterPacket.FromDate);
                        }
                        if (filterPacket.ToDate.HasValue)
                        {
                            emailResults = emailResults.Where(em => em.created_datetime.Date <= filterPacket.ToDate);
                        }
                        if (!string.IsNullOrWhiteSpace(filterPacket.SentStatus))
                        {
                            switch (filterPacket.SentStatus)
                            {
                                case "statusAllMessages":
                                    // no filtering needed.
                                    break;
                                case "statusSuccessfulMessages":
                                    int successStatus = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "EMAIL_SENT").Key;
                                    emailResults = emailResults.Where(em => em.status == successStatus);
                                    break;
                                case "statusPendingMessages":
                                    int pendingSubmit = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "PENDING_SUBMIT_TO_API").Key;
                                    int awaitingResponse = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "AWAITING_RESPONSE_FROM_API").Key;
                                    emailResults = emailResults.Where(em => new[] { pendingSubmit, awaitingResponse }.Contains(em.status));
                                    break;
                                case "statusFailedMessages":
                                    int failedStatus = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "EMAIL_UNSENT").Key;
                                    emailResults = emailResults.Where(em => em.status == failedStatus);
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(filterPacket.TargetEmailAddress))
                        {
                            emailResults = emailResults.Where(em => em.target_email_address.ToLower() == filterPacket.TargetEmailAddress.ToLower());
                        }
                        if (filterPacket.TargetLightstonePropertyId.HasValue)
                        {
                            emailResults = emailResults.Where(em => em.target_lightstone_property_id == filterPacket.TargetLightstonePropertyId);
                        }

                        results.EmailLogItems = BuildCommunicationEmailResults(emailResults);
                        return results;
                    case "SMS":
                        IEnumerable<sms_communications_log> smsResults = from log in prospecting.sms_communications_logs select log;
                        if (filterPacket.SentByMe)
                        {
                            var userGuid = RequestHandler.GetUserSessionObject().UserGuid;
                            smsResults = smsResults.Where(em => em.created_by_user_guid == userGuid);
                        }
                        else
                        {
                            var businessUnitUsers = RequestHandler.GetUserSessionObject().BusinessUnitUsers;
                            var userGuids = from u in businessUnitUsers select u.UserGuid;
                            smsResults = smsResults.Where(em => userGuids.Contains(em.created_by_user_guid));
                        }
                        if (!string.IsNullOrWhiteSpace(filterPacket.BatchName))
                        {
                            smsResults = smsResults.Where(em =>
                            {
                                if (!string.IsNullOrEmpty(em.batch_friendly_name))
                                {
                                    if (em.batch_friendly_name.ToLower().Contains(filterPacket.BatchName.ToLower()))
                                        return true;
                                }
                                return false;
                            });
                        }
                        if (filterPacket.FromDate.HasValue)
                        {
                            smsResults = smsResults.Where(em => em.created_datetime.Date >= filterPacket.FromDate);
                        }
                        if (filterPacket.ToDate.HasValue)
                        {
                            smsResults = smsResults.Where(em => em.created_datetime.Date <= filterPacket.ToDate);
                        }
                        if (!string.IsNullOrWhiteSpace(filterPacket.SentStatus))
                        {
                            switch (filterPacket.SentStatus)
                            {
                                case "statusAllMessages":
                                    // no filtering needed.
                                    break;
                                case "statusSuccessfulMessages":
                                    int successStatus = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "SMS_DELIVERED").Key;
                                    int reply = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "SMS_REPLY").Key;
                                    smsResults = smsResults.Where(em => new[] { successStatus, reply }.Contains(em.status));
                                    break;
                                case "statusPendingMessages":
                                    int pendingSubmit = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "PENDING_SUBMIT_TO_API").Key;
                                    int awaitingResponse = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "AWAITING_RESPONSE_FROM_API").Key;
                                    int submitted = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "SMS_SUBMITTED").Key;
                                    smsResults = smsResults.Where(em => new[] { pendingSubmit, awaitingResponse, submitted }.Contains(em.status));
                                    break;
                                case "statusFailedMessages":
                                    int failedStatus = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "SMS_OTHER").Key;
                                    smsResults = smsResults.Where(em => em.status == failedStatus);
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(filterPacket.TargetCellNo))
                        {
                            string formattedNo = Regex.Replace(filterPacket.TargetCellNo, @"\s+", "");
                            if (formattedNo.StartsWith("0"))
                            {
                                formattedNo = "27" + formattedNo.Remove(0, 1);
                            }
                            if (formattedNo.StartsWith("+27"))
                            {
                                formattedNo = formattedNo.Remove(0, 1);
                            }
                            smsResults = smsResults.Where(em => em.target_cellphone_no == formattedNo);
                        }
                        if (filterPacket.TargetLightstonePropertyId.HasValue)
                        {
                            smsResults = smsResults.Where(em => em.target_lightstone_property_id == filterPacket.TargetLightstonePropertyId);
                        }

                        results.SMSLogItems = BuildCommunicationSMSResults(smsResults);
                        return results;
                }
                throw new Exception("Invalid message type for filtering!");
            }
        }

        private static List<SentSMSLogItem> BuildCommunicationSMSResults(IEnumerable<sms_communications_log> smsResults)
        {
            Func<Guid, string> getSenderName = guid =>
                {
                    var businessUnitUsers = RequestHandler.GetUserSessionObject().BusinessUnitUsers;
                    return (from u in businessUnitUsers
                            where u.UserGuid == guid
                            select u.Fullname).FirstOrDefault();

                };

            Func<string, string> formatCellNo = cell => cell.StartsWith("27") ? "0" + cell.Remove(0, 2) : cell;

            Func<string, int, string> getDeliveryStatus = (apiStatus, prospectingStatus) =>
            {
                if (!string.IsNullOrEmpty(apiStatus)) return apiStatus;

                string currentCommStatus = ProspectingLookupData.CommunicationStatusTypes.First(t => t.Key == prospectingStatus).Value;
                return currentCommStatus;
            };

            List<SentSMSLogItem> items = new List<SentSMSLogItem>();
            using (var prospectingDb = new ProspectingDataContext())
            {
                Func<int, int> findSeeffAreaId = lightstonePropId =>
                {
                    return prospectingDb.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropId).seeff_area_id.Value;
                };
                int index = 0;
                foreach (var record in smsResults)
                {
                    SentSMSLogItem newItem = new SentSMSLogItem
                    {
                        id = index++,
                        DateSent = record.updated_datetime.HasValue ? record.updated_datetime.Value : record.created_datetime,
                        DeliveryStatus = getDeliveryStatus(record.api_delivery_status, record.status),
                        FriendlyNameOfBatch = record.batch_friendly_name,
                        SentBy = getSenderName(record.created_by_user_guid),
                        SentTo = formatCellNo(record.target_cellphone_no),
                        TargetLightstonePropertyId = record.target_lightstone_property_id,
                        ContactPersonId = record.target_contact_person_id,
                        SeeffAreaId = findSeeffAreaId(record.target_lightstone_property_id)
                    };
                    items.Add(newItem);
                }
            }
            return items.OrderByDescending(f => f.DateSent).ToList();
        }

        private static List<SentEmailLogItem> BuildCommunicationEmailResults(IEnumerable<email_communications_log> emailResults)
        {
            Func<int, string, string> getStatusDesc = (status, errMsg) =>
            {
                if (status == ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "EMAIL_SENT").Key)
                {
                    return "Sent";
                }
                if (status == ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "PENDING_SUBMIT_TO_API").Key ||
                    status == ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "AWAITING_RESPONSE_FROM_API").Key)
                {
                    return "Pending";
                }

                if (status == ProspectingLookupData.CommunicationStatusTypes.First(t => t.Value == "EMAIL_UNSENT").Key)
                {
                    return "Unsent (" + (!string.IsNullOrEmpty(errMsg) ? errMsg : "Reason unknown") + ")";
                }

                return "";
            };

            List<SentEmailLogItem> items = new List<SentEmailLogItem>();
            using (var prospectingDb = new ProspectingDataContext())
            {
                Func<int, int> findSeeffAreaId = lightstonePropId =>
                {
                    return prospectingDb.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropId).seeff_area_id.Value;
                };

                int index = 0;
                foreach (var record in emailResults)
                {
                    SentEmailLogItem newItem = new SentEmailLogItem
                    {
                        id = index++,
                        DateSent = record.updated_datetime.HasValue ? record.updated_datetime.Value : record.created_datetime,
                        DeliveryStatus = getStatusDesc(record.status, record.error_msg),
                        FriendlyNameOfBatch = record.batch_friendly_name,
                        SentBy = record.created_by_user_name,
                        SentTo = record.target_email_address,
                        TargetLightstonePropertyId = record.target_lightstone_property_id,
                        ContactPersonId = record.target_contact_person_id,
                        SeeffAreaId = findSeeffAreaId(record.target_lightstone_property_id)
                    };
                    items.Add(newItem);
                }
            }
            return items.OrderByDescending(f => f.DateSent).ToList();
        }

        public static ReferralResponseObject GenerateReferralDetails(ReferralInputDetails inputDetails)
        {
            ReferralResponseObject result = new ReferralResponseObject { InstanceValidationErrors = new List<string>() };
            try
            {
                using (var prospectingDb = new ProspectingDataContext())
                {
                    if (inputDetails.CreateFollowup && inputDetails.FollowupDate == null)
                    {
                        result.InstanceValidationErrors.Add("If you selected 'Create a Follow-up' then you need to select a follow-up date.");
                    }

                    int propertyRecordID = inputDetails.Target.ProspectingPropertyId.Value;
                    var propertyRecord = prospectingDb.prospecting_properties.First(pp => pp.prospecting_property_id == propertyRecordID);
                    int seeffAreaID = propertyRecord.seeff_area_id.Value;

                    SpatialServiceReader spatialReader = new SpatialServiceReader();
                    var suburb = spatialReader.LoadSuburb(seeffAreaID);

                    if (suburb == null)
                    {
                        result.InstanceValidationErrors.Add("Unable to find the Seeff suburb for this property. Please contact support and quote this property ID.");
                        return result;
                    }

                    result.department = inputDetails.DepartmentType;
                    if (string.IsNullOrEmpty(result.department))
                    {
                        result.InstanceValidationErrors.Add("Please select a Referral Type");
                    }

                    if (!suburb.LicenseID.HasValue)
                    {
                        result.InstanceValidationErrors.Add("Unable to find a License ID for this property. Please contact support and quote this property ID.");
                    }
                    else
                    {
                        result.license_id_from = result.license_id_to = suburb.LicenseID.Value;
                    }

                    result.property_id = propertyRecord.lightstone_property_id;
                    result.property_desc = ProspectingCore.GetFormattedAddress(propertyRecord.lightstone_property_id);
                    result.property_lat = propertyRecord.latitude.Value;
                    result.property_long = propertyRecord.longitude.Value;
                    result.user_guid = RequestHandler.GetUserSessionObject().UserGuid.ToString();

                    var targetContactPerson = prospectingDb.prospecting_contact_persons.First(cp => cp.contact_person_id == inputDetails.Target.ContactPerson.ContactPersonId);
                    result.ContactPersonID = targetContactPerson.contact_person_id;

                    string personTitle = "";
                    if (targetContactPerson.person_title.HasValue)
                    {
                        personTitle = ProspectingLookupData.ContactPersonTitle.First(cpt => cpt.Key == targetContactPerson.person_title.Value).Value;
                    }
                    result.smart_pass_title = personTitle;
                    result.smart_pass_name = targetContactPerson.firstname;
                    result.smart_pass_surname = targetContactPerson.surname;
                    result.smart_pass_id_no = targetContactPerson.id_number;

                    string companyName = "";
                    if (inputDetails.Target.ContactCompanyId.HasValue)
                    {
                        var company = prospectingDb.prospecting_contact_companies.First(cc => cc.contact_company_id == inputDetails.Target.ContactCompanyId.Value);
                        companyName = company.company_name;
                    }
                    result.smart_pass_company = companyName;

                    string contactType = "", countryCode = "", contactNo = "";
                    if (inputDetails.Target.ContactPerson.PhoneNumbers != null &&
                        inputDetails.Target.ContactPerson.PhoneNumbers.Count() > 0 &&
                        !string.IsNullOrEmpty(inputDetails.Target.ContactPerson.PhoneNumbers.First().ItemId))
                    {
                        var contactPhoneContainer = inputDetails.Target.ContactPerson.PhoneNumbers.First();
                        int detailID = Convert.ToInt32(contactPhoneContainer.ItemId);
                        var contactNoRecord = prospectingDb.prospecting_contact_details.FirstOrDefault(cd => cd.prospecting_contact_detail_id == detailID);
                        if (contactNoRecord != null)
                        {

                            var contactItemTypeRecord = prospectingDb.prospecting_contact_detail_types.First(cdt => cdt.contact_detail_type_id == contactNoRecord.contact_detail_type);
                            contactType = contactItemTypeRecord.type_desc;

                            countryCode = contactNoRecord.prospecting_area_dialing_code.dialing_code_id.ToString();

                            contactNo = contactNoRecord.contact_detail;
                        }
                        else
                        {
                            result.InstanceValidationErrors.Add("This contact detail has been changed by another user. Please restart and try again.");
                        }
                    }
                    else
                    {
                        result.InstanceValidationErrors.Add("No contact number specified.");
                    }
                    result.smart_pass_contact_type = contactType;
                    result.smart_pass_country_code = countryCode;
                    result.smart_pass_contact_no = contactNo;

                    string emailAddress = "";
                    if (inputDetails.Target.ContactPerson.EmailAddresses != null)
                    {
                        var emailAddressItem = inputDetails.Target.ContactPerson.EmailAddresses.FirstOrDefault();
                        if (emailAddressItem != null && !string.IsNullOrEmpty(emailAddressItem.ItemId))
                        {
                            int detailID = Convert.ToInt32(emailAddressItem.ItemId);
                            var emailRecord = prospectingDb.prospecting_contact_details.FirstOrDefault(cd => cd.prospecting_contact_detail_id == detailID);
                            if (emailRecord != null)
                            {
                                emailAddress = emailRecord.contact_detail;
                            }
                            else
                            {
                                result.InstanceValidationErrors.Add("This contact detail has been changed by another user. Please restart and try again.");
                            }
                        }
                    }
                    result.smart_pass_email_address = emailAddress;

                    result.smart_pass_comment = inputDetails.Comment ?? ""; // Might need to escape this.
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                result.InstanceValidationErrors.Add(message);
            }
            return result;
        }

        public static ReferralResponseObject CreateReferral(ReferralInputDetails inputDetails)
        {
            ReferralResponseObject referralDetails = GenerateReferralDetails(inputDetails);
            if (referralDetails.InstanceValidationErrors.Count > 0)
            {
                return referralDetails;
            }

            if (RequestHandler.IsTrainingMode())
            {
                return referralDetails;
            }

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://bossservices.seeff.com/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.PostAsJsonAsync<ReferralResponseObject>("api/BOSS/CreatePropertyReferral", referralDetails).Result;
                    response.EnsureSuccessStatusCode();
                    ReferralResponseObject referralResponse = new ReferralResponseObject();
                    referralResponse = response.Content.ReadAsAsync<ReferralResponseObject>().Result;

                    if (referralResponse.pErrorNo != -1)
                    {
                        string internalError = "Error creating referral: ";
                        switch (referralResponse.pErrorNo)
                        {
                            case 1:
                                internalError += "Service Failed";
                                break;
                            case 2:
                                internalError += "Please indicate if this referral is a Sale (S) or a Rental (R).";
                                break;
                            case 3:
                                internalError += "Please indicate the license id of the license this referral is from.";
                                break;
                            case 4:
                                internalError += "Please indicate the license id of the license this referral is going to.";
                                break;
                            case 5:
                                internalError += "No property id was passed to the service.";
                                break;
                            case 6:
                                internalError += "No property description was passed to the service.";
                                break;
                            case 7:
                                internalError += "No user guid found.";
                                break;
                            case 8:
                                internalError += "Failed to locate a user id for 'Guid Passed In'";
                                break;
                            case 9:
                                internalError += "Please give this contact person a title";
                                break;
                            case 10:
                                internalError += "Please enter the contact person's name";
                                break;
                            case 11:
                                internalError += "Please enter the contact person's surname";
                                break;
                            case 12:
                                internalError += "Please enter the contact person's contact no. type";
                                break;
                            case 13:
                                internalError += "Please enter the contact person's contact no. country code";
                                break;
                            case 14:
                                internalError += "Please enter the contact person's contact no.";
                                break;
                            case 101:
                                internalError += "Add New Person Exception";
                                break;
                            case 102:
                                internalError += "Update Person Exception";
                                break;
                            case 103:
                                internalError += "Adding smart_pass_participants Exception";
                                break;
                            case 104:
                                internalError += "Adding smart_pass_comment Exception";
                                break;
                            case 105:
                                internalError += "Sending the email Exception";
                                break;
                            case 106:
                                internalError += "Catch All Error Exception";
                                break;
                        }

                        referralDetails.InstanceValidationErrors.Add(internalError);
                        return referralDetails;
                    }
                    else
                    {
                        using (var prospecting = new ProspectingDataContext())
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("A new referral has been created for this property (Smart Pass ID: " + referralResponse.pSmart_pass_id + ")");
                            sb.AppendLine("<a target='_blank' style='text-decoration: underline;' href='http://boss.seeff.com/smart_pass_update.aspx?id=" + referralResponse.pSmart_pass_id + "'>Click here to view referral</a>");

                            if (!string.IsNullOrWhiteSpace(inputDetails.Comment))
                            {
                                sb.AppendLine();
                                sb.AppendLine("Comment:");
                                sb.AppendLine(inputDetails.Comment);
                            }

                            var activityType = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "New Referral").Key;
                            var activityRecord = new activity_log
                            {
                                lightstone_property_id = referralDetails.property_id,
                                followup_date = null,
                                allocated_to = null,
                                activity_type_id = activityType,
                                comment = sb.ToString(),
                                created_by = RequestHandler.GetUserSessionObject().UserGuid,
                                created_date = DateTime.Now,
                                contact_person_id = referralDetails.ContactPersonID,
                                // Add the rest later
                                parent_activity_id = null,
                                activity_followup_type_id = null
                            };
                            prospecting.activity_logs.InsertOnSubmit(activityRecord);
                            prospecting.SubmitChanges();

                            // Next create a follow-up if specified
                            if (inputDetails.CreateFollowup && inputDetails.FollowupDate != null)
                            {
                                var followupType = ProspectingLookupData.ActivityFollowupTypes.First(act => act.Value == "Referral Follow-up").Key;
                                string comment = "Follow-up on referral created on " + activityRecord.created_date.ToString(System.Threading.Thread.CurrentThread.CurrentCulture)
                                    + Environment.NewLine + Environment.NewLine;
                                comment += "Referral Details: " + Environment.NewLine;
                                comment += sb.ToString();
                                var followupActivity = new ProspectingActivity
                                {
                                    ActivityFollowupTypeId = followupType,
                                    ActivityTypeId = activityType,
                                    Comment = comment,
                                    ContactPersonId = referralDetails.ContactPersonID,
                                    FollowUpDate = inputDetails.FollowupDate,
                                    LightstonePropertyId = referralDetails.property_id,
                                    ParentActivityId = activityRecord.activity_log_id,
                                    IsForInsert = true,
                                    IsForUpdate = false
                                };

                                UpdateInsertActivity(followupActivity);
                            }
                        }
                    }

                    return referralResponse;
                }
                catch (Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    referralDetails.InstanceValidationErrors.Add(message);
                    return referralDetails;
                }
            }
        }

        public static ReferralsHistory RetrieveReferralsHistoryForProperty(ProspectingPropertyId input)
        {
            using (var client = new HttpClient())
            {
                ReferralsHistory referralsHistory = new ReferralsHistory();
                try
                {
                    client.BaseAddress = new Uri("http://bossservices.seeff.com/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.PostAsJsonAsync<int>("api/BOSS/GetReferralDetails", input.LightstonePropertyId).Result;
                    response.EnsureSuccessStatusCode();
                    List<ReferralItem> referralResponse = response.Content.ReadAsAsync<List<ReferralItem>>().Result;

                    referralsHistory.ListOfReferrals = referralResponse;
                    referralsHistory.ListOfReferrals = referralsHistory.ListOfReferrals.OrderByDescending(rf => rf.SmartPassId).ToList();
                    return referralsHistory;
                }
                catch (Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    referralsHistory.ErrorMessage = message;
                    return referralsHistory;
                }
            }
        }

        public static void UpdateDoNotContactStatusForContact(ProspectingContactPerson contactToUpdate)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var targetContactPerson = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == contactToUpdate.ContactPersonId);
                targetContactPerson.do_not_contact = contactToUpdate.DoNotContact;

                prospecting.SubmitChanges();
                AddClientSynchronisationRequest(targetContactPerson.contact_person_id);

                var activityType = ProspectingLookupData.ActivityTypes.First(act => act.Value == "General").Key;
                string comment;
                if (contactToUpdate.DoNotContact)
                {
                    comment = "The 'Do not contact' option has been set for the related contact person, indicating that they are not to be contacted telephonically.";
                }
                else
                {
                    comment = "The related contact person may be contacted telephonically. The 'Do not contact' option for this contact has been removed.";
                }
                ProspectingActivity pa = new ProspectingActivity
                {
                    IsForInsert = true,
                    LightstonePropertyId = contactToUpdate.TargetLightstonePropertyIdForComms.Value,
                    ActivityTypeId = activityType,
                    Comment = comment,
                    ContactPersonId = contactToUpdate.ContactPersonId
                };
                UpdateInsertActivity(pa);
            }
        }

        public static ProspectingContactCompany UpdateCompanyRegNo(ProspectingContactCompany companyWithNewRegNo)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var targetCompany = prospecting.prospecting_contact_companies.First(cc => cc.contact_company_id == companyWithNewRegNo.ContactCompanyId);
                targetCompany.CK_number = companyWithNewRegNo.CKNumber;
                prospecting.SubmitChanges();

                return new ProspectingContactCompany { CKNumber = targetCompany.CK_number };
            }
        }

        public static ProspectingContactCompany UpdateCompanyName(ProspectingContactCompany companyWithNewName)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var targetCompany = prospecting.prospecting_contact_companies.First(cc => cc.contact_company_id == companyWithNewName.ContactCompanyId);
                targetCompany.company_name = companyWithNewName.CompanyName;
                prospecting.SubmitChanges();

                return new ProspectingContactCompany { CKNumber = targetCompany.CK_number, CompanyName = targetCompany.company_name };
            }
        }

        public static ActivitiesFollowupsFilterResponse FilterActivitiesFollowupsForBusinessUnit(ActivitiesFollowupsFilterInputs filterPacket)
        {
            if (filterPacket.ActivityTypes.Count == 1 && filterPacket.ActivityTypes.First() == 0)
            {
                filterPacket.ActivityTypes = ProspectingLookupData.ActivityTypes.Select(item => item.Key).ToList();
            }

            if (filterPacket.ActivityFollowupTypes.Count == 1 && filterPacket.ActivityFollowupTypes.First() == 0)
            {
                filterPacket.ActivityFollowupTypes = ProspectingLookupData.ActivityFollowupTypes.Select(item => item.Key).ToList();
            }

            using (var prospecting = new ProspectingDataContext())
            {
                // localise to entire BU and suburb., what about no activity types/followup types
                // reset when navigating away from filter mode
                // test/impersonate Debbie, and corrobarate counts by logging in as different users

                UserDataResponsePacket user = RequestHandler.GetUserSessionObject();
                var businessUnitUsers = user.BusinessUnitUsers.Select(bu => bu.UserGuid).ToList();
                var propertiesInSuburb = prospecting.prospecting_properties.Where(pp => pp.seeff_area_id == filterPacket.CurrentSuburbID)
                                                                           .Select(pp => pp.lightstone_property_id);

                var baseSet = prospecting.activity_logs.Where(al => businessUnitUsers.Contains(al.created_by) &&
                                                                    propertiesInSuburb.Contains(al.lightstone_property_id));

                IEnumerable<activity_log> filteredSet = null;
                if (filterPacket.ShowingActivityTypes)
                {
                    // activity types only
                    filteredSet = baseSet.Where(al => filterPacket.ActivityTypes.Contains(al.activity_type_id));
                }
                 else
                {
                    // else follow-up types only
                    var filterTypes = filterPacket.ActivityFollowupTypes.Select(i => (int?)i);
                    filteredSet = baseSet.Where(al => filterTypes.Contains(al.activity_followup_type_id) &&
                                                    filterPacket.ActivityTypes.Contains(al.activity_type_id)); // must have been initially user-created.
                }

                if (filterPacket.CreatedBy.HasValue)
                {
                    filteredSet = filteredSet.Where(al => al.created_by == filterPacket.CreatedBy.Value);
                }

                if (filterPacket.AllocatedTo.HasValue)
                {
                    filteredSet = filteredSet.Where(al => al.allocated_to == filterPacket.AllocatedTo.Value);
                }

                if (filterPacket.FollowupDateFrom.HasValue)
                {
                    filteredSet = filteredSet.Where(al =>
                    {
                        if (al.followup_date.HasValue)
                        {
                            return al.followup_date.Value.Date >= filterPacket.FollowupDateFrom.Value.Date;
                        }
                        return false;
                    });
                }

                if (filterPacket.FollowupDateTo.HasValue)
                {
                    filteredSet = filteredSet.Where(al =>
                    {
                        if (al.followup_date.HasValue)
                        {
                            return al.followup_date.Value.Date <= filterPacket.FollowupDateTo.Value.Date;
                        }
                        return false;
                    });
                }

                if (filterPacket.CreatedDateFrom.HasValue)
                {
                    filteredSet = filteredSet.Where(al => al.created_date.Date >= filterPacket.CreatedDateFrom.Value.Date);
                }

                if (filterPacket.CreatedDateTo.HasValue)
                {
                    filteredSet = filteredSet.Where(al => al.created_date.Date <= filterPacket.CreatedDateTo.Value.Date);
                }

                List<ActivitiesFollowupsPivotRow> rows = new List<ActivitiesFollowupsPivotRow>();
                if (filterPacket.ShowingActivityTypes)
                {
                    // activity types
                    foreach (var item in filteredSet)
                    {
                        UserDataResponsePacket createdByUser = user.BusinessUnitUsers.FirstOrDefault(bu => bu.UserGuid == item.created_by);
                        ActivitiesFollowupsPivotRow row = new ActivitiesFollowupsPivotRow();

                        row.CreatedBy = createdByUser.Fullname;
                        row.ActivityType = ProspectingLookupData.ActivityTypes.First(at => at.Key == item.activity_type_id).Value; // NB only user types - not system types

                        UserDataResponsePacket allocatedToUser = user.BusinessUnitUsers.FirstOrDefault(bu => bu.UserGuid == item.allocated_to);
                        row.AllocatedTo = allocatedToUser != null ? allocatedToUser.Fullname : "n/a";
                        row.FollowupType = item.activity_followup_type_id.HasValue ? ProspectingLookupData.ActivityFollowupTypes.First(aft => aft.Key == item.activity_followup_type_id).Value : "system";

                        rows.Add(row);
                    }
                }
                else
                {
                    // followup types
                    foreach (var item in filteredSet)
                    {
                        UserDataResponsePacket allocatedToUser = user.BusinessUnitUsers.FirstOrDefault(bu => bu.UserGuid == item.allocated_to.Value);
                        ActivitiesFollowupsPivotRow row = new ActivitiesFollowupsPivotRow();
                       
                            row.AllocatedTo = allocatedToUser != null ? allocatedToUser.Fullname : "n/a";
                        row.FollowupType = item.activity_followup_type_id.HasValue ? ProspectingLookupData.ActivityFollowupTypes.First(aft => aft.Key == item.activity_followup_type_id).Value : "system";
                       

                        UserDataResponsePacket createdByUser = user.BusinessUnitUsers.FirstOrDefault(bu => bu.UserGuid == item.created_by);
                        row.CreatedBy = createdByUser.Fullname;
                        row.ActivityType = ProspectingLookupData.ActivityTypes.First(at => at.Key == item.activity_type_id).Value; // NB only user types - not system types

                        rows.Add(row);
                    }
                }

                ActivitiesFollowupsFilterResponse response = new ActivitiesFollowupsFilterResponse()
                {
                    OutputRows = new List<ActivitiesFollowupsPivotRow>(),
                    FilteredProperties = new List<int>()
                };
                response.OutputRows = rows;
                response.FilteredProperties = filteredSet.Select(i => i.lightstone_property_id).ToList();

                return response;
            }
        }

        public static IDValidationResult HasValidSAIdentityNumber(string idNumber)
        {
            int d = -1;
            DateTime dobResult = default(DateTime);
            try
            {
                if (string.IsNullOrWhiteSpace(idNumber) || idNumber.Length != 13)
                    return new IDValidationResult { Result = false };
                string dob = idNumber.Substring(0, 6);
                if (!DateTime.TryParseExact(dob, "yyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dobResult))
                    return new IDValidationResult { Result = false };
                if (!new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Any(ch => ch == idNumber[6]))
                    return new IDValidationResult { Result = false };
                if (!new char[] { '0', '1' }.Any(ch => ch == idNumber[10]))
                    return new IDValidationResult { Result = false };

                int a = 0;
                for (int i = 0; i < 6; i++)
                {
                    a += int.Parse(idNumber[2 * i].ToString());
                }
                int b = 0;
                for (int i = 0; i < 6; i++)
                {
                    b = b * 10 + int.Parse(idNumber[2 * i + 1].ToString());
                }
                b *= 2;
                int c = 0;
                do
                {
                    c += b % 10;
                    b = b / 10;
                }
                while (b > 0);
                c += a;
                d = 10 - (c % 10);
                if (d == 10) d = 0;
            }
            catch {/*ignore*/}
            bool result = d != -1 && idNumber[12].ToString() == d.ToString();

            return new IDValidationResult
            {
                Result = result,
                DateOfBirth = dobResult != default(DateTime) ? dobResult : (DateTime?)null
            };
        }

        public static string GeneratePseudoIdentifier()
        {
            string newIdentifier = "#";
            Random random = new Random();
            while (true)
            {
                for (int i = 0; i < 12; i++)
                {
                    int digit = random.Next(0, 9);
                    newIdentifier += digit;
                }

                using (var prospecting = new ProspectingDataContext())
                {
                    var contactWithExistingIDNumber = prospecting.prospecting_contact_persons.FirstOrDefault(cp => cp.id_number == newIdentifier);
                    if (contactWithExistingIDNumber == null)
                        break;
                }
                newIdentifier = "#";
            }

            return newIdentifier;
        }

        //public static MandateSaveResult SaveMandate(NewMandateInputs newMandateData)
        //{
        //    MandateSaveResult result = new MandateSaveResult();
        //    try
        //    {
        //        var user = RequestHandler.GetUserSessionObject();
        //        string agentNamesList = null;

        //        var lookupData = LoadMandateLookupData();
        //        int seeffAgencyID = lookupData.MarketshareAgencies.First(ag => ag.AgencyName == "Seeff").AgencyID; // SHOULD ALWAYS BE "1"
        //        if (newMandateData.MandateAgencyID == seeffAgencyID)
        //        {
        //            string[] agencyIDStrings = !string.IsNullOrWhiteSpace(newMandateData.MandateAgents) ? newMandateData.MandateAgents.Split(new[] { ',' }) : null;
        //            if (agencyIDStrings != null)
        //            {
        //                int[] agencyIDs = agencyIDStrings.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => int.Parse(s.Trim())).ToArray();
        //                string[] agentNames = lookupData.SeeffAgents.Where(a => agencyIDs.Any(sa => sa == a.AgentID)).Select(a => a.AgentName).ToArray();

        //                agentNamesList = string.Join(", ", agentNames);
        //            }
        //        }
        //        else
        //        {
        //            agentNamesList = newMandateData.MandateAgents;
        //        }

        //        using (var prospecting = new ProspectingDataContext())
        //        {
        //            var newRecord = new property_mandate { lightstone_property_id = newMandateData.LightstonePropertyId };
        //            prospecting.property_mandates.InsertOnSubmit(newRecord);
        //            prospecting.SubmitChanges();

        //            int propertyMandateID = newRecord.property_mandate_id;

        //            ProspectingActivity activity = new ProspectingActivity();
        //            activity.IsForInsert = true;
        //            activity.FollowUpDate = newMandateData.MandateFollowupDate;
        //            activity.LightstonePropertyId = newMandateData.LightstonePropertyId;
        //            var activityType = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "New Mandate").Key;
        //            activity.ActivityTypeId = activityType;
        //            StringBuilder sb = new StringBuilder();
        //            sb.AppendLine("*** New Mandate Created For Property ***");
        //            if (newMandateData.MandateFollowupDate.HasValue)
        //            {
        //                sb.AppendLine("");
        //                UserDataResponsePacket allocatedToUser = user.BusinessUnitUsers.FirstOrDefault(bu => bu.UserGuid == newMandateData.FollowupAllocatedTo);
        //                string allocatedToUserName = "";
        //                if (allocatedToUser != null)
        //                {
        //                    allocatedToUserName = allocatedToUser.Fullname;
        //                }
        //                else
        //                {
        //                    allocatedToUserName = user.Fullname;
        //                }
        //                sb.AppendLine("A follow up is scheduled to be sent to " + allocatedToUserName + " on " + newMandateData.MandateFollowupDate.Value.ToString("dddd dd MMMM yyyy"));
        //            }
        //            activity.Comment = sb.ToString();
        //            long activityID = UpdateInsertActivity(activity);
        //            long? followupActivityID = null;
        //            if (newMandateData.MandateFollowupDate.HasValue)
        //            {
        //                var followupType = ProspectingLookupData.ActivityFollowupTypes.First(act => act.Value == newMandateData.MandateFollowupTypeText).Key;
        //                string comment = newMandateData.FollowupComment;
        //                var followupActivity = new ProspectingActivity
        //                {
        //                    ActivityFollowupTypeId = followupType,
        //                    AllocatedTo = newMandateData.FollowupAllocatedTo,
        //                    ActivityTypeId = activityType,
        //                    Comment = comment,
        //                    FollowUpDate = newMandateData.MandateFollowupDate,
        //                    LightstonePropertyId = newMandateData.LightstonePropertyId,
        //                    ParentActivityId = activityID,
        //                    IsForInsert = true,
        //                    IsForUpdate = false
        //                };
        //                followupActivityID = UpdateInsertActivity(followupActivity);
        //            }

        //            var newMandateAgency = new property_mandate_agency
        //            {
        //                property_mandate_id = propertyMandateID,
        //                agency_id = newMandateData.MandateAgencyID,
        //                mandate_type = newMandateData.MandateType,
        //                mandate_status = newMandateData.MandateStatus,
        //                listing_price = newMandateData.ListingPrice,
        //                mandate_expiry_date = null,
        //                followup_activity_id = followupActivityID,
        //                created_by = user.UserGuid,
        //                created_date = DateTime.Now,
        //                agents = agentNamesList,
        //                seeff_agents = agentNamesList
        //            };
        //            prospecting.property_mandate_agencies.InsertOnSubmit(newMandateAgency);
        //            prospecting.SubmitChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        using (var prospectingDb = new ProspectingDataContext())
        //        {
        //            var errorRec = new exception_log
        //            {
        //                friendly_error_msg = ex.Message,
        //                exception_string = ex.ToString(),
        //                user = RequestHandler.GetUserSessionObject().UserGuid,
        //                date_time = DateTime.Now
        //            };
        //            prospectingDb.exception_logs.InsertOnSubmit(errorRec);
        //            prospectingDb.SubmitChanges();
        //        }
        //        result.ErrorMessage = ex.Message;
        //    }

        //    return result;
        //}

        //public static MandateLookupDataPacket LoadMandateLookupData()
        //{
        //    MandateLookupDataPacket results = new MandateLookupDataPacket();
        //    try
        //    {
        //        using (var lsBase = new ls_baseEntities())
        //        {
        //            foreach (var agency in lsBase.agency)
        //            {
        //                results.MarketshareAgencies.Add(new MarketShareAgency { AgencyID = agency.agency_id, AgencyName = agency.agency_name });
        //            }

        //            foreach (var seeffAgent in lsBase.seeff_agents)
        //            {
        //                results.SeeffAgents.Add(new SeeffAgent { AgentID = seeffAgent.agentId, AgentName = seeffAgent.agentName });
        //            }
        //        }

        //        using (var prospecting = new ProspectingDataContext())
        //        {
        //            foreach (var item in prospecting.property_mandate_types)
        //            {
        //                results.MandateTypes.Add(new MandateType { MandateTypeID = item.property_mandate_type_id, TypeDescription = item.type_description });
        //            }
        //            foreach (var item in prospecting.property_mandate_status)
        //            {
        //                results.MandateStatuses.Add(new MandateStatus { MandateStatusID = item.property_mandate_status_id, StatusDescription = item.status_description });
        //            }
        //            var mandateFollowupTypeNames = new[] { "Mandate Expiry", "Lease Expiry", "After Sales Service", "Short-term Rental Client Care" };
        //            foreach (var item in ProspectingLookupData.ActivityFollowupTypes)
        //            {
        //                if (mandateFollowupTypeNames.Any(mft => mft == item.Value))
        //                {
        //                    results.MandateFollowupTypes.Add(new KeyValuePair<int, string>(item.Key, item.Value));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        results.ErrorMessage = ex.Message;
        //    }

        //    return results;
        //}

        public static CommunicationBatchStatus CreateOptInRequestForEmailComms(ProspectingContactPerson contact)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var targetContactRecord = prospecting.prospecting_contact_persons.FirstOrDefault(cp => cp.contact_person_id == contact.ContactPersonId);
                if (targetContactRecord != null && targetContactRecord.email_contactability_status == 4)
                {
                    string optInEmailContent = GenerateOptInEmailContent(targetContactRecord.contact_person_id);
                    optInEmailContent = Convert.ToBase64String(Encoding.ASCII.GetBytes(optInEmailContent));
                    List<ProspectingContactPerson> recipient = new List<ProspectingContactPerson>();
                    var recipientEmailAddresses = ProspectingLookupData.PropertyContactEmailRetriever(prospecting, contact);
                    foreach (var emailAddress in recipientEmailAddresses)
                    {
                        recipient.Add(new ProspectingContactPerson
                        {
                            ContactPersonId = contact.ContactPersonId,
                            TargetLightstonePropertyIdForComms = contact.TargetLightstonePropertyIdForComms,
                            TargetContactEmailAddress = emailAddress.ItemContent,
                            Firstname = targetContactRecord.firstname,
                            Surname = targetContactRecord.surname
                        });
                    }
                    EmailBatch optInRequest = new EmailBatch
                    {
                        Recipients = recipient,
                        IncludeUnsubscribeLink = false, // test that this is true normally?
                        NameOfBatch = "",
                        Attachments = new List<EmailAttachment>(),
                        EmailBodyHTMLRaw = optInEmailContent,
                        EmailSubjectRaw = Convert.ToBase64String(Encoding.ASCII.GetBytes("Join the Seeff Family")),
                        TemplateActivityTypeId = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "Opt-in Request").Key
                    };

                    var status = SubmitEmailBatch(optInRequest);
                    if (status.SuccessfullySubmitted)
                    {
                        targetContactRecord.email_contactability_status = 3;
                        prospecting.SubmitChanges();
                    }

                    return status;
                }
                return new CommunicationBatchStatus { SuccessfullySubmitted = false, ErrorMessage = "Contact with the following ContactPersonId cannot be targeted at this time: " + contact.ContactPersonId };
            }
        }

        private static string GenerateOptInEmailContent(int contactId)
        {
            StringBuilder mailContent = new StringBuilder();
            //Home banner randomizor
            List<string> randomHeaderImage = new List<string>();

            randomHeaderImage.Add("MyMailHeader4.jpg");
            randomHeaderImage.Add("MyMailHeader3.jpg");
            randomHeaderImage.Add("MyMailHeader2.jpg");
            randomHeaderImage.Add("MyMailHeader1.jpg");

            Random rnd = new Random();
            int r = rnd.Next(randomHeaderImage.Count);

            mailContent.Append("<html>" + Environment.NewLine);
            mailContent.Append("<body style='font-family:arial; color:#001849; width:600px; margin-left:auto; margin-right:auto;'>" + Environment.NewLine);
            mailContent.Append("<div id='body'>" + Environment.NewLine);
            mailContent.Append("<img src='https://www.seeff.com/Images/MyMail/Headers/" + (string)randomHeaderImage[r] + "' width='600' border='n'></a>" + Environment.NewLine);

            string optInCallback = "http://154.70.214.213/ProspectingTaskScheduler/api/Email/Optin?contactPersonId=" + contactId;
            string optOutCallback = "http://154.70.214.213/ProspectingTaskScheduler/api/Email/Optout?contactPersonId=" + contactId;

            mailContent.Append("<h1  style='margin-bottom:5px; margin-top:20px; font-weight:normal; margin-left:5px; font-size:30px; '>Dear Client </h1>" + Environment.NewLine);
            mailContent.Append("<h2  style='margin-bottom:30px;  width:600px; font-weight:normal; margin-left:5px; font-size:16px;' width='680'>As a member of the \"Seeff Family\", we would like to send you property news, updates and other related articles that we believe will be of interest to you. <br/><br/>Kindly indicate that you are happy to <a href='" + optInCallback + "' style='color:#F81530; font-weight:bold; text-decoration:none; '>receive future communication</a> from us by clicking the following link: " + Environment.NewLine);
            mailContent.Append("<br/><br/><a href='" + optInCallback + "'><img src='https://www.seeff.com/Images/Buttons/SignUpButton.PNG'></a>" + Environment.NewLine);
            mailContent.Append("        <div id='footer' width='600' style='width:600px; margin-top:50px; font-size:12px;'>" + Environment.NewLine);
            mailContent.Append("            <p width='600' style='width:600px; margin-top:10px; font-size:12px;'>" + Environment.NewLine);
            mailContent.Append("            <div style='height:1px; width:600px; background-color:#F81530; margin-bottom:10px;'></div>" + Environment.NewLine);
            mailContent.Append("            Seeff Properties &copy; " + DateTime.Now.Year.ToString() + Environment.NewLine);
            mailContent.Append("            <b>|</b>  " + Environment.NewLine);
            mailContent.Append("            <a href='" + optOutCallback + "' style='color:#F81530; text-decoration:none; '> Click here</a> to opt out of any future communication" + Environment.NewLine);
            mailContent.Append("            <p><img src='validation link' style='display:none; width:1px; height:1px;' border='n'></p>" + Environment.NewLine);
            mailContent.Append("        </div>" + Environment.NewLine);
            mailContent.Append("    </body>" + Environment.NewLine);
            mailContent.Append("</html>" + Environment.NewLine);

            return mailContent.ToString();
        }

        public static List<list> GetListsForUserPermissionLevel()
        {
            using (var clientDB = new clientEntities())
            {
                var currentUser = RequestHandler.GetUserSessionObject();
                List<list> targetRecords = new List<list>();
                string permissionLevel = currentUser.PermissionLevelForLists;
                switch (permissionLevel)
                {
                    case "User":
                        targetRecords = clientDB.list.Where(li => li.fk_created_by_user_id == currentUser.RegistrationId && !li.deleted).ToList();
                        break;
                    case "Branch":
                        targetRecords = clientDB.list.Where(li => (li.fk_branch_id == currentUser.BranchID || li.fk_created_by_user_id == currentUser.RegistrationId) && !li.deleted).ToList();
                        break;
                    case "License":
                        var targetUsers = currentUser.BusinessUnitUsers.Select(bu => bu.RegistrationId).Concat(new[] { currentUser.RegistrationId });
                        targetRecords = clientDB.list.Where(li => targetUsers.Contains(li.fk_created_by_user_id) && !li.deleted).ToList();
                        break;
                }
                return targetRecords;
            }
        }

        public static List<ContactList> RetrieveListsForUser(ProspectingContactPerson currentContact)
        {
            var currentUser = RequestHandler.GetUserSessionObject();

            Func<int, KeyValuePair<int, string>> getListType = typeId =>
            {
                var target = ProspectingLookupData.ContactListUserTypes.FirstOrDefault(kvp => kvp.Key == typeId);
                if (target.Equals(default(KeyValuePair<int, string>)))
                {
                    target = ProspectingLookupData.ContactListSystemTypes.First(kvp => kvp.Key == typeId);
                }

                return target;
            };

            Func<KeyValuePair<int, string>, string> getListTypeDesc = type => 
            {
                return type.Value == "No filtering" ? "None" : type.Value;
            };

            Func<int, string> getCreatedByUser =  regId => 
            {
                var target = currentUser.BusinessUnitUsers.FirstOrDefault(bu => bu.RegistrationId == regId);
                return target != null ? target.Fullname : "n/a";
            };

            using (var clientDB = new clientEntities())
                using (var prospecting =  new ProspectingDataContext())
            {
                var targetRecords = GetListsForUserPermissionLevel();
                List<ContactList> results = new List<ContactList>();
                int? currentContactID = currentContact.ContactPersonId;
                foreach (var item in targetRecords.OrderByDescending(lt => lt.fk_list_type_id))
                {
                    var listType = getListType(item.fk_list_type_id);
                    var contactIDs = prospecting.contact_person_lists.Where(l => l.fk_list_id == item.pk_list_id).Select(c => c.contact_person_id).ToList();
                    bool currentContactIsMember = currentContactID.HasValue ? contactIDs.Contains(currentContactID.Value) : false;
                    string createdByUser = getCreatedByUser(item.fk_created_by_user_id);
                    results.Add(new ContactList
                    {
                        id = item.pk_list_id,
                        ListId = item.pk_list_id,
                        ListName = item.list_name,
                        ListType = listType,
                        ListTypeDescription = getListTypeDesc(listType),
                        MemberCount = contactIDs.Count,
                        CurrentContactIsMember = currentContactIsMember,
                        CreatedByUser = createdByUser,
                        UpdatedDate = item.updated_date.HasValue ? item.updated_date.Value.ToString("yyyy-MM-dd") : item.created_date.ToString("yyyy-MM-dd")
                    });
                }
                return results;
            }
        }

        public static object SaveListAllocationForContact(ContactListSelection listSelection)
        {
            var loggedInUser = RequestHandler.GetUserSessionObject();
            using (var clientDB = new clientEntities())
            using (var prospecting = new ProspectingDataContext())
            {
                var currentUser = RequestHandler.GetUserSessionObject();
                var listsForUser = GetListsForUserPermissionLevel().Select(l => l.pk_list_id).ToList();

                var targetsToRemove = prospecting.contact_person_lists.Where(cpl => cpl.contact_person_id == listSelection.ContactPersonId && listsForUser.Contains(cpl.fk_list_id));
                prospecting.contact_person_lists.DeleteAllOnSubmit(targetsToRemove);
                prospecting.SubmitChanges();

                var targetContactPerson = prospecting.prospecting_contact_persons.First(cp => cp.contact_person_id == listSelection.ContactPersonId);
                foreach (var item in listSelection.ListAllocation)
                {
                    if (item.Selected)
                    {
                        var newListRecord = new contact_person_list
                        {
                            contact_person_id = listSelection.ContactPersonId,
                            fk_list_id = item.ListId,
                            prospecting_property_id = listSelection.TargetPropertyId,
                            created_by = loggedInUser.UserGuid,
                            created_date = DateTime.Now                             
                        };
                        prospecting.contact_person_lists.InsertOnSubmit(newListRecord);
                        prospecting.SubmitChanges();
                    }

                    var targetList = clientDB.list.First(l => l.pk_list_id == item.ListId);
                    targetList.updated_date = DateTime.Now;
                    clientDB.SaveChanges();
                }
            }
            return true;
        }

        public static object SaveSelectionToSelectedLists(MultiContactListSelection multiSelection)
        {
            using (var clientDB = new clientEntities())
            using (var prospecting = new ProspectingDataContext())
            {
                var loggedInUser = RequestHandler.GetUserSessionObject();
                List<ProspectingContactPerson> targets = new List<ProspectingContactPerson>();
                if (multiSelection.TargetContactsList != null && multiSelection.TargetContactsList.Count > 0)
                {
                    targets = multiSelection.TargetContactsList;
                }
                else if (multiSelection.VisibleProperties != null && multiSelection.VisibleProperties.Count > 0)
                {
                    targets = LoadContacts(multiSelection.VisibleProperties.ToArray());
                }

                var targetIDs = targets.Select(g => g.ContactPersonId.ToString());
                foreach (var list in multiSelection.SelectedLists)
                {
                    var targetsForInsert = new List<ProspectingContactPerson>();
                    var queryResults = prospecting.ExecuteQuery<FlattenedPropertyRecord>(@"SELECT contact_person_id FROM contact_person_list WHERE fk_list_id = " + list.ListId +
                                                                    " AND contact_person_id in (" + targetIDs.Aggregate((n1, n2) => n1 + "," + n2) + ")", new object[] { });
                    List<int> x = new List<int>(queryResults.Select(c => c.contact_person_id.Value));
                    targetsForInsert = targets.Where(cp => !x.Contains(cp.ContactPersonId.Value)).ToList();
                    foreach (var contact in targetsForInsert)
                    {
                        var newListRecord = new contact_person_list
                        {
                            contact_person_id = contact.ContactPersonId.Value,
                            fk_list_id = list.ListId,
                            prospecting_property_id = contact.TargetProspectingPropertyId.Value,
                            created_by = loggedInUser.UserGuid,
                            created_date = DateTime.Now
                        };
                        prospecting.contact_person_lists.InsertOnSubmit(newListRecord);
                    }
                    prospecting.SubmitChanges();

                    var targetList = clientDB.list.First(l => l.pk_list_id == list.ListId);
                    targetList.updated_date = DateTime.Now;
                    clientDB.SaveChanges();
                }

                return true;
            }
        }

        public static ExportSaveResult ExportList(ListExportSelection export)
        {
            Func<IGrouping<int, ContactListExportRecord>, string, string> formatPhoneNumber = (record, phone) =>
            {
                if (phone == "") return "";
                string result = phone;
                var target = record.First(fp => fp.contact_detail == phone);
                if (target.code_desc != "South Africa +27") result = target.code_desc + " " + result;
                if (target.eleventh_digit != null) result = result + target.eleventh_digit;

                return result;
            };

            Func<string, string> encloseInQuotesIfNeeded = pr => pr.Contains(",") ?  "\"" + pr + "\"" : pr;
          

            try
            {
                using (var client = new clientEntities())
                using (var prospecting = new ProspectingDataContext())
                {
                    // only when not null (person-to-company relation AND company-to-property relation) perform population of the 2 columns
                    string baseQuery = @"select 
                                    cp.contact_person_id,
                                    pt.person_title,
                                    cp.firstname,
                                    cp.surname,
                                    cp.id_number,
                                    cp.is_popi_restricted,
                                    cp.optout_emails,
                                    cp.optout_sms,
                                    cp.do_not_contact,
                                    pp.street_or_unit_no,
                                    pp.unit,
                                    pp.ss_fh,
                                    pp.prospecting_property_id,
                                    pp.property_address,
                                    pprt.relationship_desc as relationship_to_property,
                                    pcrt.relationship_desc as relationship_to_company,
                                    cc.company_name,
                                    cc.CK_number,
                                    pp.ss_name,
                                    pp.ss_door_number,
                                    cd.contact_detail_type,
                                    cd.contact_detail,
                                    cd.eleventh_digit,
                                    adc.code_desc,
                                    cd.is_primary_contact
                                    from contact_person_list cpl
                                    join prospecting_contact_person cp on cp.contact_person_id = cpl.contact_person_id
                                    left join prospecting_property pp on pp.prospecting_property_id = cpl.prospecting_property_id
                                    left join prospecting_person_property_relationship ppr on (ppr.prospecting_property_id = pp.prospecting_property_id and ppr.contact_person_id = cp.contact_person_id)
                                    left join prospecting_person_property_relationship_type pprt on pprt.person_property_relationship_type_id = ppr.relationship_to_property
                                    left join prospecting_company_property_relationship cpr on cpr.prospecting_property_id = pp.prospecting_property_id
                                    left join prospecting_contact_company cc on cc.contact_company_id = cpr.contact_company_id
                                    left join prospecting_person_company_relationship pcr on (pcr.contact_company_id = cc.contact_company_id and pcr.contact_person_id = cp.contact_person_id)
                                    left join prospecting_person_company_relationship_type pcrt on pcrt.person_company_relationship_type_id = pcr.relationship_to_company
                                    left join prospecting_person_title pt on cp.person_title = pt.prospecting_person_title_id
                                    left join prospecting_contact_detail cd on cd.contact_person_id = cp.contact_person_id
                                    left join prospecting_area_dialing_code adc on adc.prospecting_area_dialing_code_id = cd.intl_dialing_code_id
                                    where cpl.fk_list_id = {0} and (cd.deleted is null or cd.deleted = 0)";

                    string listType = client.list.First(li => li.pk_list_id == export.ListId).list_type.type_description;
                    var results = prospecting.ExecuteQuery<ContactListExportRecord>(baseQuery, export.ListId);
                    var contacts = results.GroupBy(c => c.contact_person_id);
                    List<ListOutputRecord> outputRecords = new List<ListOutputRecord>();
                    foreach (var contactGrouping in contacts)
                    {
                        ContactListExportRecord contactRecord = contactGrouping.First();
                        ListOutputRecord record = new ListOutputRecord();
                        record.Title = contactRecord.person_title;
                        record.Firstname = contactRecord.firstname;
                        record.Surname = contactRecord.surname;
                        record.IdNumber = contactRecord.id_number;
                        record.PopiOptOut = contactRecord.is_popi_restricted;
                        record.EmailOptOut = contactRecord.optout_emails;
                        record.SmsOptOut = contactRecord.optout_sms;
                        record.DoNotContact = contactRecord.do_not_contact;
                        record.ProspectingPropertyId = contactRecord.prospecting_property_id;
                        record.SectionalTitle = contactRecord.ss_name;
                        record.LegalEntity = contactRecord.company_name;
                        record.LegalEntityNumber = contactRecord.CK_number;
                        record.RelationshipType = contactRecord.relationship_to_property ?? contactRecord.relationship_to_company;

                        if (export.Columns.Contains("[Property Address]"))
                        {
                            ProspectingProperty property = new ProspectingProperty { SS_FH = contactRecord.ss_fh, Unit = contactRecord.unit, StreetOrUnitNo = contactRecord.street_or_unit_no, SSDoorNo = contactRecord.ss_door_number, SSName = contactRecord.ss_name, PropertyAddress = contactRecord.property_address };
                            record.PropertyAddress = GetFormattedAddress(property);
                        }

                        string emailAddress = "";
                        string cellPhone = "";
                        string homeLandline = "";
                        string workLandline = "";
                        if (export.UsePrimaryContactDetailOnly)
                        {
                            var targetEmail = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && new[] { 4, 5 }.Contains(cd.contact_detail_type.Value));
                            if (targetEmail != null)
                            {
                                emailAddress = targetEmail.contact_detail;
                            }
                            var targetCellphone = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && cd.contact_detail_type == 3);
                            if (targetCellphone != null)
                            {
                                cellPhone = targetCellphone.contact_detail;
                            }
                            var targetHomeLandline = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && cd.contact_detail_type == 1);
                            if (targetHomeLandline != null)
                            {
                                homeLandline = targetHomeLandline.contact_detail;
                            }
                            var targetWorkLandline = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && cd.contact_detail_type == 2);
                            if (targetWorkLandline != null)
                            {
                                workLandline = targetWorkLandline.contact_detail;
                            }
                        }
                        else
                        {
                            var targetEmail = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && new[] { 4, 5 }.Contains(cd.contact_detail_type.Value));
                            if (targetEmail == null)
                            {
                                targetEmail = contactGrouping.FirstOrDefault(cd => cd.contact_detail_type.HasValue && new[] { 4, 5 }.Contains(cd.contact_detail_type.Value));
                                emailAddress = targetEmail != null ? targetEmail.contact_detail : "";
                            }
                            else
                            {
                                emailAddress = targetEmail.contact_detail;
                            }
                            var targetCellphone = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && cd.contact_detail_type == 3);
                            if (targetCellphone == null)
                            {
                                targetCellphone = contactGrouping.FirstOrDefault(cd => cd.contact_detail_type.HasValue && cd.contact_detail_type == 3);
                                cellPhone = targetCellphone != null ? targetCellphone.contact_detail : "";
                            }
                            else
                            {
                                cellPhone = targetCellphone.contact_detail;
                            }
                            var targetHomeLandline = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && cd.contact_detail_type == 1);
                            if (targetHomeLandline == null)
                            {
                                targetHomeLandline = contactGrouping.FirstOrDefault(cd => cd.contact_detail_type.HasValue && cd.contact_detail_type == 1);
                                homeLandline = targetHomeLandline != null ? targetHomeLandline.contact_detail : "";
                            }
                            else
                            {
                                homeLandline = targetHomeLandline.contact_detail;
                            }
                            var targetWorkLandline = contactGrouping.FirstOrDefault(cd => cd.is_primary_contact == true && cd.contact_detail_type.HasValue && cd.contact_detail_type == 2);
                            if (targetWorkLandline == null)
                            {
                                targetWorkLandline = contactGrouping.FirstOrDefault(cd => cd.contact_detail_type.HasValue && cd.contact_detail_type == 2);
                                workLandline = targetWorkLandline != null ? targetWorkLandline.contact_detail : "";
                            }
                            else
                            {
                                workLandline = targetWorkLandline.contact_detail;
                            }
                        }

                        record.EmailAddress = emailAddress;
                        record.Cellphone = formatPhoneNumber(contactGrouping, cellPhone);
                        record.HomeLandline = formatPhoneNumber(contactGrouping, homeLandline);
                        record.WorkLandline = formatPhoneNumber(contactGrouping, workLandline);

                        outputRecords.Add(record);
                    }

                    // address other options here
                    List<ListOutputRecord> netOutputRecords = new List<ListOutputRecord>();
                    foreach (var record in outputRecords)
                    {
                        ListOutputRecord netOutputRecord = new ListOutputRecord();
                        netOutputRecord.Title = record.Title != null ? record.Title : "";
                        netOutputRecord.Firstname = encloseInQuotesIfNeeded(record.Firstname);
                        netOutputRecord.Surname = encloseInQuotesIfNeeded(record.Surname);
                        netOutputRecord.IdNumber = record.IdNumber;
                        netOutputRecord.PropertyAddress = record.PropertyAddress;
                        netOutputRecord.EmailAddress = record.EmailAddress;
                        netOutputRecord.Cellphone = record.Cellphone;
                        netOutputRecord.HomeLandline = record.HomeLandline;
                        netOutputRecord.WorkLandline = record.WorkLandline;
                        netOutputRecord.PopiOptOut = record.PopiOptOut;
                        netOutputRecord.EmailOptOut = record.EmailOptOut;
                        netOutputRecord.SmsOptOut = record.SmsOptOut;
                        netOutputRecord.DoNotContact = record.DoNotContact;
                        netOutputRecord.ProspectingPropertyId = record.ProspectingPropertyId;
                        netOutputRecord.SectionalTitle = record.SectionalTitle != null ? record.SectionalTitle : "";
                        netOutputRecord.LegalEntity = record.LegalEntity != null ? (record.LegalEntity + " (" + record.LegalEntityNumber + ")") : "";
                        netOutputRecord.RelationshipType = record.RelationshipType != null ? record.RelationshipType : "";

                        if (export.OmitRecordIfEmptyField)
                        {
                            if (string.IsNullOrEmpty(netOutputRecord.Title) && export.Columns.Contains("[Title]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.Firstname) && export.Columns.Contains("[First Name]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.Surname) && export.Columns.Contains("[Surname]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.IdNumber) && export.Columns.Contains("[ID number]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.PropertyAddress) && export.Columns.Contains("[Property Address]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.EmailAddress) && export.Columns.Contains("[Email Address]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.Cellphone) && export.Columns.Contains("[Cellphone]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.HomeLandline) && export.Columns.Contains("[Home Landline]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.WorkLandline) && export.Columns.Contains("[Work Landline]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.RelationshipType) && export.Columns.Contains("[Relationship Type]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.SectionalTitle) && export.Columns.Contains("[Sectional Title]"))
                                continue;
                            if (string.IsNullOrEmpty(netOutputRecord.LegalEntity) && export.Columns.Contains("[Legal Entity]"))
                                continue;
                        }

                        if (export.ExcludeRecordIfPopiChecked)
                        {
                            if (netOutputRecord.PopiOptOut) continue;
                        }
                        if (export.ExcludeRecordIfEmailOptOut)
                        {
                            if (netOutputRecord.EmailOptOut) continue;
                        }
                        if (export.ExcludeRecordIfSmsOptOut)
                        {
                            if (netOutputRecord.SmsOptOut) continue;
                        }
                        if (export.ExcludeRecordIfDoNotContactChecked)
                        {
                            if (netOutputRecord.DoNotContact) continue;
                        }

                        netOutputRecords.Add(netOutputRecord);
                    }

                    if (export.ExcludeDuplicateEmailAddress)
                    {
                        var uniqueByEmail = netOutputRecords.GroupBy(r => r.EmailAddress).ToList();
                        netOutputRecords.Clear();
                        var emptyEmailGroup = uniqueByEmail.FirstOrDefault(gr => gr.First().EmailAddress == "");
                        if (emptyEmailGroup != null && emptyEmailGroup.Count() > 0)
                        {
                            foreach (var item in emptyEmailGroup)
                            {
                                netOutputRecords.Add(item);
                            }
                        }
                        foreach (var item in uniqueByEmail.Where(gr => gr.First().EmailAddress != ""))
                        {
                            netOutputRecords.Add(item.First());
                        }
                    }

                    if (listType == "Today's Birthdays" || listType == "Today's Anniversaries")
                    {
                        switch (listType)
                        {
                            case "Today's Birthdays":
                                List<ListOutputRecord> bdayList = new List<ListOutputRecord>();
                                foreach (var item in netOutputRecords)
                                {
                                    var idValidation = ProspectingCore.HasValidSAIdentityNumber(item.IdNumber);
                                    if (idValidation.Result) // success
                                    {
                                        var dateOfBirth = idValidation.DateOfBirth.Value;
                                        if (dateOfBirth.Month == DateTime.Today.Month && dateOfBirth.Day == DateTime.Today.Day)
                                        {
                                            bdayList.Add(item);
                                        }
                                    }
                                }
                                netOutputRecords = bdayList;
                                break;
                            case "Today's Anniversaries":
                                List<ListOutputRecord> propertyAnniversaryList = new List<ListOutputRecord>();
                                foreach (var item in netOutputRecords)
                                {
                                    var prospectingProperty = prospecting.prospecting_properties.First(pp => pp.prospecting_property_id == item.ProspectingPropertyId);
                                    if (!string.IsNullOrEmpty(prospectingProperty.lightstone_reg_date) && prospectingProperty.lightstone_reg_date.Length == 8)
                                    {
                                        DateTime regDate;
                                        if (DateTime.TryParseExact(prospectingProperty.lightstone_reg_date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out regDate))
                                        {
                                            var todaysDate = DateTime.Today;
                                            int regYear = regDate.Year;
                                            if (regYear < todaysDate.Year)
                                            {
                                                if (regDate.Month == todaysDate.Month && regDate.Day == todaysDate.Day)
                                                {
                                                    propertyAnniversaryList.Add(item);
                                                }
                                            }
                                        }
                                    }
                                }
                                netOutputRecords = propertyAnniversaryList;
                                break;
                        }
                    }

                    // column selection here
                    if (export.OutputFormat == ".xlsx")
                    {
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            string listName = client.list.First(li => li.pk_list_id == export.ListId).list_name;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(listName + " - " + DateTime.Now.ToShortDateString());
                            worksheet.Cells["A1"].LoadFromDataTable(new DataTable(), true);

                            worksheet.Row(1).Style.Font.Bold = true;
                            worksheet.Row(1).Height = 20;
                            worksheet.Row(2).Height = 18;

                            List<PropertyInfo> recordProps = new List<PropertyInfo>();
                            for (int i = 1; i <= export.Columns.Count; i++)
                            {
                                worksheet.Cells[1, i].Value = export.Columns[i - 1].Replace("[", "").Replace("]", "");

                                var recordProp = typeof(ListOutputRecord).GetProperties().First(prop =>
                                {
                                    ColumnMappingAttribute MyAttribute = (ColumnMappingAttribute)Attribute.GetCustomAttribute(prop, typeof(ColumnMappingAttribute));
                                    return MyAttribute != null ? MyAttribute.ColumnName == export.Columns[i - 1] : false;
                                });
                                recordProps.Add(recordProp);
                            }

                            int rowNumber = 2;
                            foreach (var record in netOutputRecords)
                            {
                                int columnCounter = 1;
                                foreach (var col in export.Columns)
                                {
                                    var x = recordProps[columnCounter - 1].GetValue(record);
                                    worksheet.Cells[rowNumber, columnCounter++].Value = x.ToString();
                                }
                                rowNumber++;
                            }

                            for (int i = 1; i <= export.Columns.Count; i++)
                            {
                                worksheet.Column(i).AutoFit();
                            }

                            string fileName = listName + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                            return SaveFile(fileName, package);
                        }
                    }

                    if (export.OutputFormat == ".csv")
                    {
                        string listName = client.list.First(li => li.pk_list_id == export.ListId).list_name;
                        string fileName = listName + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";

                        StringBuilder sb = new StringBuilder();
                        List<PropertyInfo> recordProps = new List<PropertyInfo>();
                        for (int i = 1; i <= export.Columns.Count; i++)
                        {
                            var recordProp = typeof(ListOutputRecord).GetProperties().First(prop =>
                            {
                                ColumnMappingAttribute MyAttribute = (ColumnMappingAttribute)Attribute.GetCustomAttribute(prop, typeof(ColumnMappingAttribute));
                                return MyAttribute != null ? MyAttribute.ColumnName == export.Columns[i - 1] : false;
                            });
                            recordProps.Add(recordProp);
                        }

                        string header = export.Columns.Aggregate((s1, s2) => s1.Replace("[", "").Replace("]", "") + "," + s2.Replace("[", "").Replace("]", ""));
                        sb.AppendLine(header);

                        foreach (var record in netOutputRecords)
                        {
                            List<string> recordFields = new List<string>();
                            int columnCounter = 1;
                            foreach (var col in export.Columns)
                            {
                                var x = recordProps[columnCounter - 1].GetValue(record);
                                recordFields.Add(x.ToString());
                                columnCounter++;
                            }

                            string recordRow = recordFields.Aggregate((s1, s2) => s1 + "," + s2);
                            sb.AppendLine(recordRow);
                        }

                        return SaveFile(fileName, sb);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ExportSaveResult { Success = false, Error = ex.ToString() };
            }

            return new ExportSaveResult { Success = false };
        }

        public static ExportSaveResult SaveFile(string fileName, ExcelPackage package)
        {
            var currentUser = RequestHandler.GetUserSessionObject();
            int branchId = currentUser.BranchID;
            string exportPath = "/ExportOutput/" + branchId + "/" + fileName;

            var path = HttpContext.Current.Server.MapPath("~" + exportPath);
            FileInfo fs = new FileInfo(path);
            fs.Directory.Create();
            package.SaveAs(fs);

            string appFolderPath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath;
            appFolderPath = Path.Combine(appFolderPath, exportPath);
            return new ExportSaveResult { Success = true, Filepath = appFolderPath };
        }

        public static ExportSaveResult SaveFile(string fileName, StringBuilder sb)
        {
            var currentUser = RequestHandler.GetUserSessionObject();
            int branchId = currentUser.BranchID;
            string exportPath = "/ExportOutput/" + branchId + "/" + fileName;

            var path = HttpContext.Current.Server.MapPath("~" + exportPath);
            FileInfo fs = new FileInfo(path);
            fs.Directory.Create();
            System.IO.File.WriteAllText(path, sb.ToString());

            string appFolderPath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath;
            appFolderPath = Path.Combine(appFolderPath, exportPath);
            return new ExportSaveResult { Success = true, Filepath = appFolderPath };
        }

        public static List<ContactListType> RetrieveListTypes()
        {
            List<ContactListType> types = new List<ContactListType>();
            using (var client = new clientEntities())
            {
                types = (from lt in client.list_type
                         select new ContactListType
                         {
                             ListTypeId = lt.pk_list_type_id,
                             ListTypeDescription = lt.type_description
                         }).ToList();

                return types;
            }
        }

        public static bool CreateNewListForUser(ContactList input)
        {
            var currentUser = RequestHandler.GetUserSessionObject();
            using (var client = new clientEntities())
            {
                list newList = new list
                {
                    list_name = input.ListName,
                    fk_list_type_id = input.ListType.Key,
                    fk_branch_id = currentUser.BranchID,
                    fk_created_by_user_id = currentUser.RegistrationId,
                    created_date = DateTime.Now,
                    updated_date = DateTime.Now,
                    deleted = false
                };
                client.list.Add(newList);
                client.SaveChanges();
                return true;
            }
        }

        public static bool DeleteList(ContactList input)
        {
            using (var client = new clientEntities())
            {
                var targetList = client.list.First(li => li.pk_list_id == input.ListId);
                targetList.deleted = true;
                targetList.updated_date = DateTime.Now;
                client.SaveChanges();
                return true;
            }
        }

        private static void LogLightstoneCall(string srcLocation, string serviceInputs)
        {
            var currentUser = RequestHandler.GetUserSessionObject();
            using (var prospecting = new ProspectingDataContext())
            {
                lightstone_call_log newEntry = new lightstone_call_log
                {
                    user = currentUser.UserGuid,
                    date_time = DateTime.Now,
                    call_location_src = srcLocation,
                    search_inputs = serviceInputs != null ? serviceInputs.ToString() : null
                };
                prospecting.lightstone_call_logs.InsertOnSubmit(newEntry);
                prospecting.SubmitChanges();
            }
        }
    }
}

public static class ListExtensions
{
    public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
    {
        for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
            yield return new List<T>(source.Skip(size * i).Take(size));
    }
}