using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json;
using ProspectingProject.Services;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Data.Linq;

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

        public static List<GeoLocation> LoadPolyCoords(int suburbId, string resCommAgri = "R")
        {
            using (var prospectingDB = new ProspectingDataContext())
            {
                return (from entry in prospectingDB.prospecting_kml_areas
                        where entry.prospecting_area_id == suburbId && entry.area_type == resCommAgri.ToCharArray()[0]
                        orderby entry.seq ascending
                        select new GeoLocation
                        {
                            Lat = entry.latitude,
                            Lng = entry.longitude
                        }).ToList();
            }
        }

        public static string GetAreaName(int suburbId)
        {
            using (var prospectingDB = new ProspectingDataContext())
            {
                return prospectingDB.prospecting_areas.First(n => n.prospecting_area_id == suburbId).area_name;
            }
        }      

        private static ProspectingProperty CreateProspectingProperty(ProspectingDataContext prospectingContext, prospecting_property prospectingRecord, bool loadContactsAndCompanies, bool loadOwnedProperties, bool loadActivities)
        {
            var latLng = new GeoLocation { Lat= prospectingRecord.latitude.Value, Lng = prospectingRecord.longitude.Value};
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
                Prospected =  Convert.ToBoolean(prospectingRecord.prospected),
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
                ParkingBays = prospectingRecord.parking_bays 
            };

            switch (prospectingRecord.ss_fh)
            {
                case "SS": prop.SS_FH = "SS"; break;
                case "FH": prop.SS_FH = "FH"; break;
                case "FS": prop.SS_FH = "FS"; break;
                case "FRM": prop.SS_FH = "FRM"; break;
                default: prop.SS_FH = "FH"; break;
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
                int? seeffAreaId = prospectingContext.prospecting_properties.First(pp => pp.lightstone_property_id == activity.lightstone_property_id).seeff_area_id;
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
                    ActivityFollowupTypeId = activity.activity_followup_type_id,
                    ActivityFollowupTypeName = activityFollowupTypeName,
                    SeeffAreaId = seeffAreaId
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
                ParkingBays = prospectingRecord.parking_bays 
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
                   if (areasClause == ""){ areasClause = prospectingAreas;}
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
                var areaIds = areaIdList.Split(new[] { ',' }).Select(id => Convert.ToInt32(id));
                return (from n in prospecting.prospecting_areas
                        join pp in prospecting.prospecting_properties on n.prospecting_area_id equals pp.seeff_area_id
                        into sr
                        from x in sr.DefaultIfEmpty()
                        where areaIds.Contains(n.prospecting_area_id)
                        select new UserSuburb
                        {
                            SuburbId = n.prospecting_area_id,
                            SuburbName = n.area_name,
                            TotalFullyProspected = sr.Select(p => p.prospecting_property_id).Count(),
                            PropertiesRequireAttention = sr.Where(p => p.latest_reg_date != null).Count()
                        }).Distinct().OrderBy(a => a.SuburbName).ToList();
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
                       SMSOptout = pcp.optout_sms
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

            using (var prospecting = new ProspectingDataContext())
            {
                int areaId = recordToCreate.SeeffAreaId.HasValue ? recordToCreate.SeeffAreaId.Value : prospecting.find_area_id(standaloneUnit.LatLng.Lat, standaloneUnit.LatLng.Lng, "R", null);
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
                    portion_no =!string.IsNullOrEmpty(standaloneUnit.Portion) ? int.Parse(standaloneUnit.Portion) : (int?)null,
                    lightstone_suburb = !string.IsNullOrEmpty(standaloneUnit.LightstoneSuburb) ? standaloneUnit.LightstoneSuburb : null,

                    created_by = Guid.Parse((string)HttpContext.Current.Session["user_guid"]),
                    created_date = DateTime.Now
                };

                prospecting.prospecting_properties.InsertOnSubmit(newPropRecord);
                try
                {
                    prospecting.SubmitChanges(); // Create the property first before adding contacts
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot insert duplicate key in object"))
                    {
                        throw new DuplicatePropertyRecordException { ErrorMsg = "Property already exists in the system.", SeeffAreaId = areaId };
                    }

                    throw;
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

                var property = CreateProspectingProperty(prospecting, newPropRecord, true, true, false);
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
            using (var prospecting = new ProspectingDataContext())
            {
                var availableSuburbs = (List<UserSuburb>)HttpContext.Current.Session["user_suburbs"];
                foreach (int suburbId in availableSuburbs.Select(s => s.SuburbId))
                {
                    if (prospecting.inside_seeff_area(geoLocation.Lat, geoLocation.Lng, suburbId) == 1)
                    {
                        return true;
                    }
                }

                return false;
            }            
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

                prospecting.SubmitChanges();
            }
        }

        /// <summary>
        /// *** Prospecting user authorisation web methods ***
        /// </summary>

        public static UserDataResponsePacket LoadUser(Guid userGuid, Guid sessionKey)
        {
            using (var authService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                var userAuthPacket = authService.AuthenticateAndGetUserInfo(userGuid, sessionKey);
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
                             UserSurname = bu.UserSurname
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
                      BusinessUnitID = userAuthPacket.BusinessUnitID
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
                    string propAddress = propertyRecord.street_or_unit_no + " " + propertyRecord.property_address;

                    var activityType = prospecting.activity_types.FirstOrDefault(t => t.activity_type_id == act.activity_type_id);
                    string activityTypeName = "";
                    if (activityType != null) 
                    {
                        activityTypeName = activityType.activity_name;
                    }
                    var activityFollowupType =prospecting.activity_followup_types.FirstOrDefault(t => t.activity_followup_type_id == act.activity_followup_type_id);
                    string activityFollowupTypeName = "";
                    if (activityFollowupType != null) {
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
                                            SeeffAreaId = seeffAreaId
                                        };
                    followups.Add(followup);
                }
            }

            fb.Followups = followups.OrderByDescending(s => s.FollowupDate).ToList();
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
                var contact = (from c in prospecting.prospecting_contact_persons
                               where c.contact_person_id == dataPacket.ContactPerson.ContactPersonId
                               select c).FirstOrDefault();
                if (contact != null)
                {
                    // Update contact person
                    contact.person_title = incomingContact.Title.HasValue ? (int?)incomingContact.Title.Value : null;
                    contact.firstname = incomingContact.Firstname;
                    contact.surname = incomingContact.Surname;
                    contact.id_number = incomingContact.IdNumber;
                    contact.person_gender = incomingContact.Gender;
                    contact.updated_date = DateTime.Now;
                    contact.comments_notes = incomingContact.Comments;
                    contact.is_popi_restricted = incomingContact.IsPOPIrestricted;

                   contact.deceased_status =  incomingContact.DeceasedStatus;
                   contact.age_group = incomingContact.AgeGroup;
                   contact.location = incomingContact.Location;
                   contact.marital_status = incomingContact.MaritalStatus;
                   contact.home_ownership = incomingContact.HomeOwnership;
                   contact.directorship = incomingContact.Directorship;
                   contact.physical_address = incomingContact.PhysicalAddress;
                   contact.employer = incomingContact.Employer;
                   contact.occupation = incomingContact.Occupation;
                   contact.bureau_adverse_indicator = incomingContact.BureauAdverseIndicator;
                   contact.citizenship = incomingContact.Citizenship;

                    contact.optout_emails = incomingContact.EmailOptout;
                    contact.optout_sms = incomingContact.SMSOptout;

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
                        }
                        prospecting.SubmitChanges();

                        // *** delete any existing relationship person-property relationship 
                        var personPropertyRelation = (from r in prospecting.prospecting_person_property_relationships
                                                      where r.contact_person_id == contact.contact_person_id &&
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
                                                      where r.contact_person_id == contact.contact_person_id &&
                                                      r.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                      select r).FirstOrDefault();
                        if (incomingContact.PersonPropertyRelationships == null || incomingContact.PersonPropertyRelationships.Count == 0)
                        {
                            // TODO: this line makes an incorrect assumption: that the existing contact linked to the new property is an "owner". Fix this.
                            incomingContact.PersonPropertyRelationships = new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(dataPacket.ProspectingPropertyId.Value,ProspectingLookupData.PersonPropertyRelationshipTypes.First().Key) };
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
                                contact_person_id = contact.contact_person_id,
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
                                                     where r.contact_person_id == contact.contact_person_id && q.prospecting_property_id == dataPacket.ProspectingPropertyId
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
                }
                else
                {
                    string idNumberOfNewContact = dataPacket.ContactPerson.IdNumber;
                    // Search for a contact with this id number
                    var contactWithExistingIDNumber = (from c in prospecting.prospecting_contact_persons
                                                       where c.id_number == idNumberOfNewContact
                                                       select c).FirstOrDefault();
                    if (contactWithExistingIDNumber != null)
                    {
                        // A contact with this id number already exists, we must ensure that they are linked to the property
                        var existingRelationshipToProperty = from ppr in prospecting.prospecting_person_property_relationships
                                                             where ppr.contact_person_id == contactWithExistingIDNumber.contact_person_id
                                                             && ppr.prospecting_property_id == dataPacket.ProspectingPropertyId
                                                             select ppr;

                        // TODO: This code will need to change to accomodate multiple records for the same person who changed their relationship to the property over time.
                        if (existingRelationshipToProperty.Count()==0)
                        {
                            incomingContact.PropertiesOwned = LoadPropertiesOwnedByThisContact(idNumberOfNewContact, prospecting);

                            // New person-property relationship
                            var personPropertyRelation = new prospecting_person_property_relationship
                            {
                                contact_person_id = contactWithExistingIDNumber.contact_person_id,
                                prospecting_property_id = dataPacket.ProspectingPropertyId.Value,
                                relationship_to_property = GetPersonRelationshipToProperty(dataPacket.ProspectingPropertyId, incomingContact.PersonPropertyRelationships),
                                created_date = DateTime.Now
                            };
                            prospecting.prospecting_person_property_relationships.InsertOnSubmit(personPropertyRelation);
                            prospecting.SubmitChanges();
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
                        };
                        prospecting.prospecting_contact_persons.InsertOnSubmit(newContact);
                        prospecting.SubmitChanges();
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
                        , "", "", "", "", searchInputValues.OwnerName, searchInputValues.OwnerIdNumber, searchInputValues.EstateName, "", propertyID, 1000, "", "", 0, 0);
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
                                                    contactDetails.EmailAddresses.Contains(cd.contact_detail.ToLower())) && cp.id_number != contactDetails.IdNumber
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
                        ProspectingProperty prop = CreateProspectingProperty(prospectingDB, prospectable, false, false,false);
                        prospectables.Add(prop);
                    }
                }
            }

            return prospectables;
        }

        public static ProspectingSuburb LoadProspectingSuburb(SuburbDataRequestPacket suburbDataRequest)
        {
                       
            ProspectingSuburb suburb = new ProspectingSuburb();
            suburb.LocationID = suburbDataRequest.SuburbId;

            suburb.PolyCoords = ProspectingCore.LoadPolyCoords(suburbDataRequest.SuburbId);
            suburb.ProspectingProperties = ProspectingCore.CreateProspectableProperties(suburbDataRequest.SuburbId);
            suburb.LocationName = ProspectingCore.GetAreaName(suburbDataRequest.SuburbId);

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
                    SSName = row["SECTIONAL_TITLE"] != null ? row["SECTIONAL_TITLE"].ToString() : string.Empty,
                    SS_FH = row["property_type"] != null ? row["property_type"].ToString() : string.Empty,
                    PurchPrice = row["PURCHASE_PRICE"] != null ? row["PURCHASE_PRICE"].ToString() : string.Empty,
                    Unit = row["UNIT"] != null ? row["UNIT"].ToString(): string.Empty,
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
                var unitsToInclude = matchesForSuburb.Where(m => {
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
                        PropertyMatches = new[] {match}.ToList(),
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
                PersonPropertyRelationships = new List<KeyValuePair<int,int>>(),//ProspectingStaticData.PersonPropertyRelationshipTypes.First(t => t.Value == "Owner").Key,
                 PersonCompanyRelationshipType = null,
                //PropertiesOwned = LoadPropertiesOwnedByThisContact(idNumber, null),
                 ContactEntityType = ContactEntityType.NaturalPerson
            };
        }

        private static ProspectingContactCompany CreateCompanyContactFromDataRow(DataRow dr)
        {
            var ckNo = dr["BUYER_IDCK"].ToString();
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
                                                  ParkingBays = pp.parking_bays                                                   
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
            if (string.IsNullOrEmpty(dr["BUYER_IDCK"].ToString()))
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
                // Take the whole surname. If the surname is fewer than 7 characters then pad the rest
                if (string.IsNullOrWhiteSpace(surname) || surname.Length < 7)
                {
                    surname = surname.PadRight(7, 'X');
                }

                if ((surname.Length + idNumber.Length) > 13)
                {
                    surname = surname.Substring(0, 7);
                }
                
                string result = string.Concat(idNumber, surname);
                if (result.Length > 13)
                {
                    result = new string(result.Take(13).ToArray());
                }
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
            // Use the current province (from Google)
            using (var prospecting = new ProspectingDataContext())
            {
                List<LightstonePropertyMatch> results = new List<LightstonePropertyMatch>();
                foreach (var match in distinctMatches)
                {
                    if (prospecting.inside_seeff_area(match.LatLng.Lat, match.LatLng.Lng, suburbId) == 1)
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

            using (var prospecting = new ProspectingDataContext())
            {
                prospecting.CommandTimeout = 4 * 60;

                int areaId = sectionalTitle.SeeffAreaId.HasValue ? sectionalTitle.SeeffAreaId.Value : prospecting.find_area_id(latLng.Lat, latLng.Lng, "R", null);

                // Ensure that we are creating ALL units for this sectional title, not just one
                using (lightstoneSeeffService.Seeff service = new lightstoneSeeffService.Seeff())
                {
                    string erf = sectionalTitle.PropertyMatches.First(s => !string.IsNullOrWhiteSpace(s.ErfNo)).ErfNo;
                    string ssName = sectionalTitle.SectionalScheme;
                    DataSet result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", "", erf, "", ssName, "", ""
                       , "", "", "", "", "", "", "", "", "", "", "", 0, 1000, "", "", 0.0, 0.0);

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

                    prospecting.prospecting_properties.InsertOnSubmit(newPropRecord);
                    try
                    {
                        prospecting.SubmitChanges(); // Create the property first before adding contacts
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Cannot insert duplicate key in object"))
                        {
                            throw new DuplicatePropertyRecordException { ErrorMsg = "Property already exists in the system.", SeeffAreaId = areaId };
                        }

                        throw;
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

                    var property = CreateProspectingProperty(prospecting, newPropRecord, false, false, false);
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

                ProspectingProperty property = CreateProspectingProperty(prospectingDB, propRecord, true, true, dataPacket.LoadActivities);

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
                SendEmail("danie@learnit.co.za", manager.UserName, "reports@seeff.com", "", "reports@seeff.com", "TEST EMAIL >>>>" + "Prospecting system notification", "TEST EMAIL >>>>" + emailTemplate);
            }
        }

        private static void SendEmail(string toAddress, string displayName, string fromAddress, string ccAddress, string bccAddress, string subject, string body)
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
            message.Bcc.Add("danie@learnit.co.za");

            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;

            message.IsBodyHtml = true;
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
                }
            }
        }

        public static ActivityBundle LoadUserActivities()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                return LoadProspectingActivities(prospecting, null);
            }
        }

        // NB. might need to remove occurrences of find_area_id that use the prospecting_area_layer tbl.
        public static int? FindAreaId(GeoLocation location)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://spatial.seeff.com//"); // This is the web application's root server address
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            HttpContent content = new ObjectContent<GeoLocation>(location, jsonFormatter);
            var resp = client.PostAsync("api/SeeffSpatialLookup/GetAreaId", content).Result;
            int? areaId = resp.Content.ReadAsAsync<int?>().Result;

            return areaId;
        }        

        public static List<ProspectingProperty> LoadProperties(int[] inputProperties)
        {
            UnlockCurrentProspectingRecord();
            List<ProspectingProperty> results = new List<ProspectingProperty>();
            using (var prospectingDB = new ProspectingDataContext())
            {
                foreach (int lightstonePropertyId in inputProperties)
                {
                    var propRecord = prospectingDB.prospecting_properties.First(pp => pp.lightstone_property_id == lightstonePropertyId);
                    ProspectingProperty propertyWithContacts = CreateProspectingProperty(prospectingDB, propRecord, true, false, false);
                    results.Add(propertyWithContacts);
                }
            }
            return results;
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
            sb.AppendLine("Details of the previous owner(s):");
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
            using (var prospecting = new ProspectingDataContext())
            {
                // Find and delete existing relationships between this property and contacts and companies (if any)
                propertyRecord = prospecting.prospecting_properties.First(pp => pp.lightstone_property_id == property.LightstonePropertyId);

                // Verify that the record has not already been updated in another session
                if (propertyRecord.latest_reg_date == null)
                {
                    return false;
                }

                CreateActivityForPropertyChangeOfOwnership(prospecting, propertyRecord);

                prospecting.prospecting_company_property_relationships.DeleteAllOnSubmit(propertyRecord.prospecting_company_property_relationships);
                prospecting.prospecting_person_property_relationships.DeleteAllOnSubmit(propertyRecord.prospecting_person_property_relationships);
                prospecting.SubmitChanges();

                // Re-prospect
                propertyMatch = searchResult.PropertyMatches[0];
                propertyRecord.updated_date = DateTime.Now;
                propertyRecord.last_purch_price = !string.IsNullOrEmpty(propertyMatch.PurchPrice) ? decimal.Parse(propertyMatch.PurchPrice, CultureInfo.InvariantCulture) : (decimal?)null;
                propertyRecord.lightstone_reg_date = propertyMatch.RegDate;
                propertyRecord.latest_reg_date = null;
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
                                    "Is this its current value: " + (valuation.IsCurrentValue ? "Yes" : "No");

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
    }
}