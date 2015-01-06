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
using ProspectingProject.ProspectingUserAuthService;

namespace ProspectingProject
{
    /// <summary>
    /// This class must contain all business logic specific to Prospecting. 
    /// It is the only class that should handle the .dbml data context objects directly.
    /// Do not inherit off this type.
    /// </summary>
    public sealed class ProspectingDomain
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

        //private static ProspectingProperty CreateProspectingProperty(LightstoneListing lightstoneListing, List<LightstoneListing> allLightstoneListings)
        //{
        //    ProspectingProperty prop = new ProspectingProperty
        //    {
        //        ProspectingPropertyId = null, // This record can be inserted
        //        LightstonePropertyId = lightstoneListing.PropertyId,
        //        Contacts = new List<ProspectingContactPerson>(), // Cannot have contacts   
        //        HasContactsWithDetails = false,
        //        HasTracePSEnquiry = false,
        //         LatLng = new GeoLocation { Lat = lightstoneListing.LatLong.Lat, Lng= lightstoneListing.LatLong.Lng },
        //        SSUnits = LoadPropertiesInSS(lightstoneListing.PropertyId, allLightstoneListings),
        //        PropertyAddress = lightstoneListing.PropertyAddress,
        //        StreetOrUnitNo = lightstoneListing.StreetOrUnitNo,
        //        SeeffAreaId = lightstoneListing.SeeffAreaId,

        //        LightstoneSales = LoadLightstoneListingsForPoprId(lightstoneListing.PropertyId.Value, allLightstoneListings)
        //    };

        //    return prop;
        //}       

        private static ProspectingProperty CreateProspectingProperty(ProspectingDataContext prospectingContext, prospecting_property prospectingRecord, bool loadOwnedProperties)
        {
            var latLng = new GeoLocation { Lat= prospectingRecord.latitude.Value, Lng = prospectingRecord.longitude.Value};
            ProspectingProperty prop = new ProspectingProperty
            {
                ProspectingPropertyId = prospectingRecord.prospecting_property_id,
                LightstonePropertyId = prospectingRecord.lightstone_property_id,
                
                Contacts = LoadContacts(prospectingContext, prospectingRecord, loadOwnedProperties),
                ContactCompanies = LoadContactCompanies(prospectingContext, prospectingRecord),
                
                //--HasContactsWithDetails = DetermineIfAnyContactsHaveDetails(prospectingRecord.prospecting_property_id),
                //--HasTracePSEnquiry = HasTracePSEnquiry(prospectingRecord.prospecting_property_id),
                 LatLng = latLng,
                PropertyAddress = prospectingRecord.property_address,
                StreetOrUnitNo = prospectingRecord.street_or_unit_no,
                SeeffAreaId = prospectingRecord.seeff_area_id.HasValue ? prospectingRecord.seeff_area_id.Value : (int?)null,
                LightstoneIDOrCKNo = prospectingRecord.lightstone_id_or_ck_no,
                LightstoneRegDate = prospectingRecord.lightstone_reg_date,
                ErfNo = prospectingRecord.erf_no,
                Comments = prospectingRecord.comments,
                SS_FH = prospectingRecord.ss_fh == "SS" ? "SS" : "FH", // default to FH for backward compat.
                SSName = prospectingRecord.ss_name,
                SSNumber = prospectingRecord.ss_number,
                SS_ID = prospectingRecord.ss_id,
                Unit = prospectingRecord.unit,
                SSDoorNo = prospectingRecord.ss_door_number,
                LastPurchPrice = prospectingRecord.last_purch_price,
                Prospected =  Convert.ToBoolean(prospectingRecord.prospected)
            };

            return prop;
        }

        private static ProspectingProperty LoadProspectingProperty(prospecting_property prospectingRecord, bool loadOwnedProperties)
        {
            var latLng = new GeoLocation { Lat = prospectingRecord.latitude.Value, Lng = prospectingRecord.longitude.Value };
            ProspectingProperty prop = new ProspectingProperty
            {
                ProspectingPropertyId = prospectingRecord.prospecting_property_id,
                LightstonePropertyId = prospectingRecord.lightstone_property_id,

                //--HasContactsWithDetails = DetermineIfAnyContactsHaveDetails(prospectingRecord.prospecting_property_id),
                //--HasTracePSEnquiry = HasTracePSEnquiry(prospectingRecord.prospecting_property_id),
                LatLng = latLng,
                PropertyAddress = prospectingRecord.property_address,
                StreetOrUnitNo = prospectingRecord.street_or_unit_no,
                SeeffAreaId = prospectingRecord.seeff_area_id.HasValue ? prospectingRecord.seeff_area_id.Value : (int?)null,
                LightstoneIDOrCKNo = prospectingRecord.lightstone_id_or_ck_no,
                LightstoneRegDate = prospectingRecord.lightstone_reg_date,
                ErfNo = prospectingRecord.erf_no,
                Comments = prospectingRecord.comments,
                SS_FH = prospectingRecord.ss_fh == "SS" ? "SS" : "FH", // default to FH for backward compat.
                SSName = prospectingRecord.ss_name,
                SSNumber = prospectingRecord.ss_number,
                SS_ID = prospectingRecord.ss_id,
                Unit = prospectingRecord.unit,
                SSDoorNo = prospectingRecord.ss_door_number,
                LastPurchPrice = prospectingRecord.last_purch_price,
                Prospected = Convert.ToBoolean(prospectingRecord.prospected)
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

        private static bool HasTracePSEnquiry(int prospectingPropertyId)
        {
            using (var prospectingDB = new ProspectingDataContext())
            {
                return prospectingDB.prospecting_trace_ps_enquiries.Any(s => s.prospecting_property_id == prospectingPropertyId);
            }
        }

        private static bool DetermineIfAnyContactsHaveDetails(int prospectingPropertyId)
        {
            using (var prospectingDB = new ProspectingDataContext())
            {
                var contactsWithDetails = from cd in prospectingDB.prospecting_contact_details
                                          join ppr in prospectingDB.prospecting_person_property_relationships
                                          on cd.contact_person_id equals ppr.contact_person_id
                                          where ppr.prospecting_property_id == prospectingPropertyId
                                          select cd;
                return contactsWithDetails.Count() > 0;
            }
        }

        //private static List<int?> LoadPropertiesInSS(int? lightstonePropertyId, List<LightstoneListing> allLightstoneListings)
        //{
        //    if (lightstonePropertyId == null || allLightstoneListings == null)
        //    {
        //        return new List<int?>();
        //    }

        //    // Load all units that are part of the sectional scheme this unit belongs to
        //    var unit = allLightstoneListings.First(s => s.PropertyId == lightstonePropertyId);
        //    return (from ss in allLightstoneListings
        //            where ss.PropertyId != unit.PropertyId && ss.LatLong.Equals(unit.LatLong)
        //            select ss.PropertyId).ToList();
        //}

        //private static List<LightstoneListing> LoadLightstoneListingsForPoprId(int? propId, List<LightstoneListing> listings)
        //{
        //    if (propId == null || listings == null) return null;

        //    return listings.Where(c => c.PropertyId == propId).OrderByDescending(a => a.RegDate).ToList();
        //}

        //private static bool HasContacts(prospecting_property property)
        //{
        //    //if (property == null) return false;

        //    //using (var ls_base = new LightStoneDataContext())
        //    //{
        //    //    var existingContacts = from contact in ls_base.prospecting_contacts
        //    //                           where contact.prospecting_property_id == property.prospecting_property_id
        //    //                           select contact;
        //    //    return existingContacts.Count() > 0;
        //    //}
        //    return false;
        //}
        public static string LoadSuburbInfoForUser(string suburbsWithPermissions)
        {
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
            List<UserSuburb> lList = new List<UserSuburb>();

            DataSet DS = null;
            string SQL = null;
            SeeffGlobal.clsData da = new SeeffGlobal.clsData();
            SQL = "  SELECT        prospecting_area.prospecting_area_id, prospecting_area.area_name, COUNT(prospecting_property.prospecting_property_id) AS prospected";
            SQL += " FROM            prospecting_area LEFT OUTER JOIN";
            SQL += "                          prospecting_property ON prospecting_area.prospecting_area_id = prospecting_property.seeff_area_id";
            SQL += " WHERE        (prospecting_area.prospecting_area_id IN (" + areaIdList + "))";
            SQL += " GROUP BY prospecting_area.prospecting_area_id, prospecting_area.area_name";
            SQL += " ORDER BY prospecting_area.area_name";
            DS = da.CommandSQL(SQL, "boss.dbo.license", (int)(SeeffGlobal.clsData.App.prospecting), HttpContext.Current.Server.MapPath("/data") + "/DBSettings.xml");
            if (!string.IsNullOrEmpty(da.CommandSQLErrMsg))
            {
                //Add error logging
                string test = da.CommandSQLErrMsg;
            }
            else
            {
                foreach (DataRow dtRow in DS.Tables["boss.dbo.license"].Rows)
                {
                    var outUserSuburb = new UserSuburb {
                        SuburbId = da.valInteger(dtRow, "prospecting_area_id"),
                        SuburbName = da.valString(dtRow, "area_name"),
                        TotalFullyProspected = da.valInteger(dtRow, "prospected")
                    };
                    lList.Add(outUserSuburb);
                }
            }
            return lList; 
        }

        private static int GetNumberPartiallyProspectedProperties(int areaId, ProspectingDataContext context)
        {
            return 0;
            int count = 0;
            var prospectingProperties = context.prospecting_properties.Where(pp => pp.seeff_area_id == areaId);
            foreach (var property in prospectingProperties)
            {
                var contactsForProperty = from cpp in context.prospecting_person_property_relationships
                                          join c in context.prospecting_contact_persons on cpp.contact_person_id equals c.contact_person_id
                                          where cpp.prospecting_property_id == property.prospecting_property_id
                                          select c;
                var numContactsWithCOntactDetails = from c in contactsForProperty
                                                    join cpd in context.prospecting_contact_details on c.contact_person_id equals cpd.contact_person_id
                                                    select c;
                if (numContactsWithCOntactDetails.Count() == 0)
                {
                    count++;
                }
            }

            return count;
        }

        private static int GetNumberFullyProspectedProperties(int areaId, ProspectingDataContext context)
        {
            var prospectingPropertiesWithContactableOwners = (from pp in context.prospecting_properties
                                                          from cpp in context.prospecting_person_property_relationships
                                                          join c in context.prospecting_contact_persons on cpp.contact_person_id equals c.contact_person_id
                                                          join cpd in context.prospecting_contact_details on c.contact_person_id equals cpd.contact_person_id
                                                          where cpp.prospecting_property_id == pp.prospecting_property_id && pp.seeff_area_id == areaId
                                                          select pp).Distinct();

            // do this, then check DATA!
            //
            //

            var prospectingPropertiesOwnedByCompaniesWithContactableOwners = (from a in context.prospecting_properties
                                                                              join b in context.prospecting_company_property_relationships on a.prospecting_property_id equals b.prospecting_property_id
                                                                              join c in context.prospecting_person_company_relationships on b.contact_company_id equals c.contact_company_id
                                                                              join d in context.prospecting_contact_persons on c.contact_person_id equals d.contact_person_id
                                                                              join cpd in context.prospecting_contact_details on d.contact_person_id equals cpd.contact_person_id
                                                                              where a.seeff_area_id == areaId
                                                                              select a).Distinct();

                // Ensure that there is no duplication in the 2 lists

            return prospectingPropertiesWithContactableOwners.Union(prospectingPropertiesOwnedByCompaniesWithContactableOwners).Count();
        }  

        public static string SerialiseStaticProspectingData()
        {
            var a = new
            {
                ContactDetailTypes = ProspectingStaticData.ContactDetailTypes,
                PersonPropertyRelationshipTypes = ProspectingStaticData.PersonPropertyRelationshipTypes,
                ContactPersonTitle = ProspectingStaticData.ContactPersonTitle,
                PersonPersonRelationshipTypes = ProspectingStaticData.PersonPersonRelationshipTypes,
                ContactEmailTypes = ProspectingStaticData.ContactEmailTypes,
                ContactPhoneTypes = ProspectingStaticData.ContactPhoneTypes,
                IntlDialingCodes = ProspectingStaticData.IntlDialingCodes,
                PersonCompanyRelationshipTypes = ProspectingStaticData.PersonCompanyRelationshipTypes
            };

            return ProspectingDomain.SerializeToJsonWithDefaults(a);
        }


        public static List<ProspectingContactPerson> LoadContacts(ProspectingDataContext prospecting, prospecting_property property, bool loadOwnedProperties)
        {
            List<ProspectingContactPerson> contactsAssociatedWithProperties = ProspectingStaticData.PropertyContactsRetriever(prospecting, property, loadOwnedProperties).ToList();
            List<ProspectingContactPerson> contactsAssociatedWithCompanies = ProspectingStaticData.PropertyCompanyContactsRetriever(prospecting, property, loadOwnedProperties).ToList();

            foreach (var cp in contactsAssociatedWithProperties)
            {
                cp.PhoneNumbers = ProspectingStaticData.PropertyContactPhoneNumberRetriever(prospecting, cp).ToList();
                cp.EmailAddresses = ProspectingStaticData.PropertyContactEmailRetriever(prospecting, cp).ToList();
            }


            foreach (var cp in contactsAssociatedWithCompanies)
            {
                cp.PhoneNumbers = ProspectingStaticData.PropertyContactPhoneNumberRetriever(prospecting, cp).ToList();
                cp.EmailAddresses = ProspectingStaticData.PropertyContactEmailRetriever(prospecting, cp).ToList();
            }

            // Combine 2 lists and return 
            contactsAssociatedWithProperties.AddRange(contactsAssociatedWithCompanies);
            return contactsAssociatedWithProperties;
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
        public static ProspectingProperty CreateNewProspectingRecord(ProspectingPropertyInputData dataPacket)
        {
            if (!ProspectWithinOwnedSuburbs(dataPacket.LatLng))
            {
                return new ProspectingProperty {  CreateError = "Cannot create new prospect: The property falls outside of your available suburbs." };
            }

            using (var prospecting = new ProspectingDataContext())
            {
                int areaId = prospecting.find_area_id(dataPacket.LatLng.Lat, dataPacket.LatLng.Lng, "R", null); // not needed
                var newPropRecord = new prospecting_property
                {
                    lightstone_property_id = dataPacket.LightstoneId,
                    latitude = dataPacket.LatLng.Lat,
                    longitude = dataPacket.LatLng.Lng,
                    property_address = dataPacket.PropertyAddress,
                    street_or_unit_no = dataPacket.StreetOrUnitNo,
                    seeff_area_id = areaId,
                    lightstone_id_or_ck_no = dataPacket.LightstoneIDOrCKNo,
                    lightstone_reg_date = dataPacket.LightstoneRegDate,
                    erf_no = dataPacket.ErfNo,
                    ss_fh = dataPacket.SS_FH,
                    ss_id = dataPacket.SS_ID,
                    ss_name = !string.IsNullOrEmpty(dataPacket.SSName) ? dataPacket.SSName : null,
                    ss_number = !string.IsNullOrEmpty(dataPacket.SSNumber) ? dataPacket.SSNumber : null,
                    unit = !string.IsNullOrEmpty(dataPacket.Unit) ? dataPacket.Unit : null,
                    ss_door_number = !string.IsNullOrEmpty(dataPacket.SSDoorNo) ? dataPacket.SSDoorNo : null,
                    last_purch_price = dataPacket.LastPurchPrice
                };

                prospecting.prospecting_properties.InsertOnSubmit(newPropRecord);
                prospecting.SubmitChanges(); // Create the property first before adding contacts

                foreach (var owner in dataPacket.ContactPersons)
                {
                    var newContact = new ContactDataPacket { ContactPerson = owner, ProspectingPropertyId = newPropRecord.prospecting_property_id, ContactCompanyId = null };
                    SaveContactPerson(newContact);
                }

                foreach (var owner in dataPacket.ContactCompanies)
                {
                    var newContact = new CompanyContactDataPacket { ContactCompany = (ProspectingContactCompany)owner, ProspectingPropertyId = newPropRecord.prospecting_property_id };
                    SaveContactCompany(newContact);
                }

                var property = CreateProspectingProperty(prospecting, newPropRecord, true);

                //TODO : Run the Prospected Stored Proc
                var newId = newPropRecord.prospecting_property_id;


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
                            relationship_to_property = ProspectingStaticData.CompanyPropertyRelationshipTypes.First(s => s.Value == "Owner").Key,
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
                        relationship_to_property = ProspectingStaticData.CompanyPropertyRelationshipTypes.First(s => s.Value == "Owner").Key,
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

        public static int? FindSeeffAreaId(GeoLocation point)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                int? result = null;
                foreach (var provId in new[] { 2, 11, 12, 13, 14, 15, 16, 17, 18 })
                {
                    result = prospecting.find_area_id(point.Lat, point.Lng, "R", provId);
                    if (result > 0)
                    {
                        return result;
                    }
                }
            }
            return null;
        }


        public static void UpdateProspectingRecord(ProspectingPropertyInputData dataPacket)
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

                prospecting.SubmitChanges();
            }
        }

        /// <summary>
        /// *** Prospecting user authorisation web methods ***
        /// </summary>

        public static UserDataResponsePacket LoadUser(Guid userGuid)
        {
            string endpoint = HttpContext.Current.IsDebuggingEnabled ? "LOCALHOST" : "BasicHttpBinding_ISeeffProspectingAuthService";

            using (var authService = new SeeffProspectingAuthServiceClient(endpoint))
            {
                var userAuthPacket = authService.GetUserInfo(userGuid);
                return new UserDataResponsePacket
                {
                    UserGuid = userGuid,
                    AvailableSuburbs = LoadSuburbInfoForUser(userAuthPacket.SuburbsList),
                    StaticProspectingData = SerialiseStaticProspectingData(),
                    AvailableCredit = userAuthPacket.AvailableCredit
                };
            }
        }

        public static int ReimburseTracePsCredit(Guid guid)
        {
            string endpoint = HttpContext.Current.IsDebuggingEnabled ? "LOCALHOST" : "BasicHttpBinding_ISeeffProspectingAuthService";

            using (var authService = new SeeffProspectingAuthServiceClient(endpoint))
            {
                return authService.ReimburseOneCredit(guid);
            }
        }

        public static int DeductTracePsCredit(Guid guid)
        {
            string endpoint = HttpContext.Current.IsDebuggingEnabled ? "LOCALHOST" : "BasicHttpBinding_ISeeffProspectingAuthService";

            using (var authService = new SeeffProspectingAuthServiceClient(endpoint))
            {
                return authService.TakeOneCredit(guid);
            }
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
                            incomingContact.PersonPropertyRelationships = new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(dataPacket.ProspectingPropertyId.Value,ProspectingStaticData.PersonPropertyRelationshipTypes.First().Key) };
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
                            ProspectingStaticData.ContactPhoneTypes,
                            incomingContact.PhoneNumbers,
                            existingContactItems,
                            prospecting);
                    }
                    if (incomingContact.EmailAddresses != null)
                    {
                        UpdateContactDetails(incomingContact.ContactPersonId.Value,
                            ProspectingStaticData.ContactEmailTypes,
                            incomingContact.EmailAddresses,
                            existingContactItems, prospecting);
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
                             created_date = DateTime.Now
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
                rel = ProspectingStaticData.PersonPropertyRelationshipTypes.First(s => s.Value == "Owner").Key;
            }

            return rel;
        }


        // TODO: investigate deleting records from contact details table and keep tabs on record counts coming in as new records are added/removed..


        public static void UpdateContactDetails(int contactPersonId, List<KeyValuePair<int, string>> affectedContactTypes, IEnumerable<ProspectingContactDetail> incomingDetails, IEnumerable<prospecting_contact_detail> existingContactItems, ProspectingDataContext prospecting)
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

            // SCENARIO (2)
            if (incomingDetails.Count() == 0)
            {
                prospecting.prospecting_contact_details.DeleteAllOnSubmit(from c in prospecting.prospecting_contact_details
                                                                          where c.contact_person_id == contactPersonId
                                                                          && affectedContactTypeIds.Contains(c.contact_detail_type)
                                                                          select c);
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
            prospecting.prospecting_contact_details.DeleteAllOnSubmit(from c in prospecting.prospecting_contact_details
                                                                      where c.contact_person_id == contactPersonId
                                                                      && affectedContactTypeIds.Contains(c.contact_detail_type) // contact item is either a phone or email
                                                                      && !newContactIDs.Contains(c.prospecting_contact_detail_id) // not affected by new insert/update
                                                                      select c);
        }

        public static List<NewProspectingLocation> FindMatchingProperties(SearchInputPacket searchInputValues)
        {
            List<LightstonePropertyMatch> matches = new List<LightstonePropertyMatch>();
            // Step 1:  Search lightstone for matches
            using (lightstoneSeeffService.Seeff service = new lightstoneSeeffService.Seeff())
            {
                 DataSet result = null;
                 try
                 {
                     result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", searchInputValues.DeedTown, searchInputValues.ErfNo, ""
                         , searchInputValues.SSName, searchInputValues.Unit, searchInputValues.Suburb, searchInputValues.StreetName, searchInputValues.StreetOrUnitNo, ""
                         , "", "", "", "", "", "", searchInputValues.EstateName, "", 0, 1000, "", "", 0, 0);
                     if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                     {
                         foreach (DataRow row in result.Tables[1].Rows)
                         {
                           // here..   
                             AddLightstonePropertyRow(row, matches);
                         }
                     }
                 }
                 catch { }
                // Remove outliers
            }

            // For every unique (prop ID) and sectional scheme group of units, we need to create one NewProspectingLocation object

            // Assumptions: we must assume that the matches contains a list of both FH and SS units. We assume because the FH units all have unique property IDs
            // it's only the SS that we must group
            // First get a list of all SS present in the matches
            List<string> uniqueSS = new List<string>();
            foreach (var match in matches)
            {
		        if (match.SS_FH == "SS") 
                {
                    if (!uniqueSS.Contains(match.SS_ID)) {
                        uniqueSS.Add(match.SS_ID);
                    }
                }
            }

              List<NewProspectingLocation> locations = new List<NewProspectingLocation>();
             // Load all SS's
              foreach (var item in uniqueSS)
              {
                  var allUnitsForSS = from m in matches
                                      where m.SS_FH == "SS" && m.SS_ID == item
                                      select m;
                  List<LightstonePropertyMatch> ssMatches = new List<LightstonePropertyMatch>(allUnitsForSS);
                  NewProspectingLocation ssLoc = GenerateOutputForProspectingLocation(ssMatches);
                  locations.Add(ssLoc);
              }
            // Load all FH's
              foreach (var m in matches)
              {
                  if (m.SS_FH == "FH")
                  {
                      NewProspectingLocation ssLoc = GenerateOutputForProspectingLocation(new [] {m}.ToList());
                      locations.Add(ssLoc);
                  }
              }

            // Step 2: determine any matching property IDs, exclude any licence ids that do not fall with the session licence id.        

              return locations;
        }

        public static void SavePropertyNotesComments(PropertyCommentsNotes propNotesContainer)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var prospectingProperty = (from prop in prospecting.prospecting_properties
                                           where prop.prospecting_property_id == propNotesContainer.ProspectingPropertyId
                                           select prop).FirstOrDefault();
                if (prospectingProperty != null)
                {
                    prospectingProperty.comments = propNotesContainer.CommentsNotes;
                }
                prospecting.SubmitChanges();
            }
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
                    var phoneTypeIds = ProspectingStaticData.ContactPhoneTypes.Select(k => k.Key);
                var emailTypeIds = ProspectingStaticData.ContactEmailTypes.Select(k => k.Key);
                    ProspectingContactPerson person = new ProspectingContactPerson
                    {
                        Firstname = existingContactWithDetail.firstname,
                        Surname = existingContactWithDetail.surname,
                        IdNumber = existingContactWithDetail.id_number,
                        PropertiesOwned = LoadPropertiesOwnedByThisContact(existingContactWithDetail.id_number, prospecting),
                        ContactPersonId = existingContactWithDetail.contact_person_id,
                        PersonPropertyRelationships = new List<KeyValuePair<int,int>>(),//ProspectingStaticData.PersonPropertyRelationshipTypes.First(s => s.Value == "Owner").Key,
                         PersonCompanyRelationshipType = null,
                         Gender = existingContactWithDetail.person_gender,
                          Title = existingContactWithDetail.person_title,
                           
                          PhoneNumbers = (from det in prospecting.prospecting_contact_details
                                            where det.contact_person_id == existingContactWithDetail.contact_person_id
                                            && phoneTypeIds.Contains(det.contact_detail_type)
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
                                            }).ToList(),

                    EmailAddresses = (from det in prospecting.prospecting_contact_details
                                              where det.contact_person_id == existingContactWithDetail.contact_person_id
                                              && emailTypeIds.Contains(det.contact_detail_type)
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
                        ProspectingProperty prop = CreateProspectingProperty(prospectingDB, prospectable, false);
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

            suburb.PolyCoords = ProspectingDomain.LoadPolyCoords(suburbDataRequest.SuburbId);
            suburb.ProspectingProperties = ProspectingDomain.CreateProspectableProperties(suburbDataRequest.SuburbId);
            suburb.LocationName = ProspectingDomain.GetAreaName(suburbDataRequest.SuburbId);

            return suburb;
        }

        public static ProspectingDataResponsePacket PerformLookupTransaction(ProspectingPropertyInputData dataPacket)
        {
            ProspectingDataResponsePacket results = new ProspectingDataResponsePacket();
            if (!string.IsNullOrEmpty(dataPacket.LightstoneIDOrCKNo))
            {
                var guid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
                bool creditTaken = false;
                try
                {
                    int newBalanceLessOne = ProspectingDomain.DeductTracePsCredit(guid);
                    if (newBalanceLessOne > -1)
                    {
                        creditTaken = true;
                        results = MakeTracePsEnquiry(dataPacket.LightstoneIDOrCKNo);
                        SaveEnquiryAgainstProperty(dataPacket.ProspectingPropertyId, results);
                        results.AvailableTracePsCredits = newBalanceLessOne;
                    }
                    else
                    {
                        results.AvailableTracePsCredits = 0;
                        results.ErrorMsg = "You have no prospecting credits available.";
                    }
                }
                finally
                {
                    if (!results.EnquirySuccessful && creditTaken)
                    {
                        int newBalancePlusOne = ProspectingDomain.ReimburseTracePsCredit(guid);
                        results.AvailableTracePsCredits = newBalancePlusOne;
                    }
                }
            }
            else
            {
                results = new ProspectingDataResponsePacket { ErrorMsg = "No ID number associated with this property." };
            }
            return results;
        }

        public static NewProspectingLocation GetMatchingAddresses(ProspectingPropertyInputData dataPacket)
        {            
            List<LightstonePropertyMatch> matches = new List<LightstonePropertyMatch>();
            using (lightstoneSeeffService.Seeff service = new lightstoneSeeffService.Seeff())
            {
                DataSet result = null;
                double lat = Convert.ToDouble(dataPacket.LatLng.Lat);
                double lng = Convert.ToDouble(dataPacket.LatLng.Lng);
                try
                {
                    //result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", "", "", "", "", "", ""
                    //    , dataPacket.StreetName, dataPacket.StreetOrUnitNo, "", "", "", "", "", "", "", "", "", 0, 1000, "", "", 0, 0);
                    result = service.ReturnProperties_Seef("a44c998b-bb46-4bfb-942d-44b19a293e3f", "", "", "", "", "", "", "", ""
                        , "", "", "", "", "", "", "", "", "", "", "", 0, 1000, "", "", lng, lat);
                }
                catch { return new NewProspectingLocation(); }
                if (result.Tables[0] != null && result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        AddLightstonePropertyRow(row, matches);                   
                    }
                }
            }

            var matchesForSuburb = GetMatchesForCurrentSuburbOnly(matches, dataPacket.SeeffAreaId.Value);

            var responsePacket = GenerateOutputForProspectingLocation(matchesForSuburb);
            return responsePacket;
        }

        private static void AddLightstonePropertyRow(DataRow row, List<LightstonePropertyMatch> matches)
        {
            Func<object, object, GeoLocation> toLatLng = (lat, lng) =>
            {
                GeoLocation latLng = new GeoLocation();
                latLng.Lat = Convert.ToDecimal(lat);
                latLng.Lng = Convert.ToDecimal(lng);
                return latLng;
            };

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
                    else
                    {
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
                // This is a new property that must be added
                LightstonePropertyMatch match = new LightstonePropertyMatch
                {
                    // Property details
                    City = row["MUNICNAME"].ToString(),
                    Suburb = row["DEEDTOWN"].ToString(),
                    StreetName = row["STREET_NAME"].ToString() + " " + row["STREET_TYPE"].ToString(),
                    StreetOrUnitNo = streetOrUnitNo,
                    LightstonePropId = lightstonePropId,
                    RegDate = row["REG_DATE"].ToString(),
                    LatLng = toLatLng(row["Y"], row["X"]),
                    SSName = row["SECTIONAL_TITLE"].ToString(),
                    SS_FH = row["property_type"].ToString(),
                    PurchPrice = row["PURCHASE_PRICE"].ToString(),
                    Unit = row["UNIT"].ToString(),
                    SSNumber = row["SS_NUMBER"].ToString(),
                    SS_UnitNoFrom = row["SS_UnitNoFrom"].ToString(),
                    SS_UnitTo = row["SS_UnitTo"].ToString(),
                    LightstoneIdExists = PropertyExists(lightstonePropId),   
                    ErfNo = row["ERF"].ToString(),
                    SS_ID = row["SS_ID"].ToString()
                };

                var owner = GetOwnerFromDataRow(row);
                var owners = owner != null ? new List<IProspectingContactEntity>(new[] { owner }) : new List<IProspectingContactEntity>();
                match.Owners = owners;
                matches.Add(match);
            }  
        }

        private static IProspectingContactEntity DetermineIfOwnerExists(List<IProspectingContactEntity> owners, IProspectingContactEntity propOwner)
        {
            return owners.FirstOrDefault(o => o.IsSameEntity(propOwner));
        }

        private static bool PropertyExists(int lightstonePropId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                return prospecting.prospecting_properties.Any(pp => pp.lightstone_property_id == lightstonePropId);
            }
        }

        private static NewProspectingLocation GenerateOutputForProspectingLocation(List<LightstonePropertyMatch> matchesForSuburb)
        {
            NewProspectingLocation propertyPacket = new NewProspectingLocation();
            propertyPacket.PropertyMatches = matchesForSuburb;
            propertyPacket.SSExists = false;
            propertyPacket.IsSectionalScheme = false;
            if  (propertyPacket.PropertyMatches.Count == 0)
            {
                return propertyPacket;
            }

            // The following code tests for whether we should treat this location as a Sectional Scheme (block of flats)
            // We need to assume at this level that the address of all matches is the same
            // If *any* matches are marked as a "SS" we must perform the test.
            if (propertyPacket.PropertyMatches.Any(m => m.SS_FH.ToLower() == "ss"))
            {
                // If *all* matches are marked as "SS", have a purch price and same address (assumption as above), we can take it this is a block of flats
                if (propertyPacket.PropertyMatches.All(m => m.SS_FH.ToLower() == "ss" /*&& !string.IsNullOrEmpty(m.PurchPrice)*/ ))
                {
                    propertyPacket.IsSectionalScheme = true;
                    propertyPacket.SectionalScheme = propertyPacket.PropertyMatches.First(p => !string.IsNullOrEmpty(p.SSName)).SSName;
                }
                else
                {
                    // If some matches are "SS" but do not comply with all the pure conditions for this being an SS, it may *still* be an SS                  
                    // In order to qualify as an SS, we must test the following conditions:
                    var anomalousUnits = propertyPacket.PropertyMatches.Where(s => s.SS_FH.ToLower() != "ss");
                    if ((anomalousUnits.Count(f => f.SS_FH == "FH") == 1) || (anomalousUnits.Any(f => string.IsNullOrEmpty(f.PurchPrice))))
                    {
                        propertyPacket.IsSectionalScheme = propertyPacket.PropertyMatches.Except(anomalousUnits).All(m => m.SS_FH.ToLower() == "ss");
                    }
                    // If this is determined to be a SS, remove the anamolous units/matches
                    if (propertyPacket.IsSectionalScheme)
                    {
                        propertyPacket.PropertyMatches = propertyPacket.PropertyMatches.Except(anomalousUnits).ToList();
                        propertyPacket.SectionalScheme = propertyPacket.PropertyMatches.First(p => !string.IsNullOrEmpty(p.SSName)).SSName;
                    }
                    else
                    {
                        // If there are *any* units that have a SS name, but the block as a whole does not comply with SS requirements, then we just don't know
                        string address = matchesForSuburb[0].StreetOrUnitNo + " " + matchesForSuburb[0].StreetName + " " + matchesForSuburb[0].Suburb + " " + matchesForSuburb[0].City;
                        string msg = @"Warning: The location selected contains a mixture of SS and non-SS properties.<br \>
                               Cannot determine the type of entity under the clicked location (please contact support)<br />" +
                                      "Address: " + address;
                        propertyPacket.ErrorMessage = msg;
                    }
                }
            }

            if (propertyPacket.IsSectionalScheme)
            {
                propertyPacket.PropertyMatches = propertyPacket.PropertyMatches.OrderBy(m => m.Unit).ToList();
                propertyPacket.SSExists = SSExists(propertyPacket.SectionalScheme);
            }
            return propertyPacket;
        }

        private static bool SSExists(string ssName)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                return prospecting.prospecting_properties.Any(pp => pp.ss_name == ssName);
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
                PropertiesOwned = LoadPropertiesOwnedByThisContact(idNumber, null),
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
                CompanyContacts = LoadCompanyContacts(ckNo),
                ProspectingProperties = LoadPropertiesOwnedByCompany(ckNo),
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
                                      LastPurchPrice = pp.last_purch_price
                                  }).ToList();
                return properties;
            }
        }

        private static List<ProspectingContactPerson> LoadCompanyContacts(string ckNo)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var phoneTypeIds = ProspectingStaticData.ContactPhoneTypes.Select(k => k.Key);
                var emailTypeIds = ProspectingStaticData.ContactEmailTypes.Select(k => k.Key);
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
                                           PhoneNumbers = (from det in prospecting.prospecting_contact_details
                                                           where det.contact_person_id == cpr.contact_person_id
                                                           && phoneTypeIds.Contains(det.contact_detail_type)
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
                                                           }),
                                           EmailAddresses = (from det in prospecting.prospecting_contact_details
                                                             where det.contact_person_id == cpr.contact_person_id
                                                             && emailTypeIds.Contains(det.contact_detail_type)
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
                return string.Concat(idNumber, surname);
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

        private static void SaveEnquiryAgainstProperty(int? prospectingPropertyId, ProspectingDataResponsePacket results)
        {
            try
            {
                using (var prospecting = new ProspectingDataContext())
                {
                    // This is an UPSERT                               
                    var newRecord = new prospecting_trace_ps_enquiry
                    {
                        prospecting_property_id = prospectingPropertyId.Value,
                        user = Guid.Parse((string)HttpContext.Current.Session["user_guid"]),
                        date_of_enquiry = DateTime.Now,
                        successful = string.IsNullOrEmpty(results.ErrorMsg),
                        id_number = results.IdCkNo
                    };
                    prospecting.prospecting_trace_ps_enquiries.InsertOnSubmit(newRecord);

                    // If there is no primary ID number associated with this property, set it now:
                    var existingProspectingRecord = (from prop in prospecting.prospecting_properties
                                                     where prop.prospecting_property_id == prospectingPropertyId
                                                     select prop).FirstOrDefault();
                    if (existingProspectingRecord != null)
                    {
                        if (string.IsNullOrEmpty(existingProspectingRecord.lightstone_id_or_ck_no))
                        {
                            existingProspectingRecord.lightstone_id_or_ck_no = results.IdCkNo;
                        }
                    }

                    prospecting.SubmitChanges();
                }
            }
            catch
            {
                // For now just supress an error here. TODO: fix this.
            }
        }

        private static ProspectingDataResponsePacket MakeTracePsEnquiry(string idOrCK)
        {
            ProspectingDataResponsePacket results = new ProspectingDataResponsePacket();
            results.EnquirySuccessful = true;
            XDocument outputXml = null;// XDocument.Parse(TracePSTemp.Data);

            idOrCK = idOrCK.Replace("/", "%2F");
            idOrCK = HttpContext.Current.Server.UrlEncode(idOrCK);
            Uri uri = new Uri(@"http://ws.traceps.co.za/ws/idnLookup/11222122/" + idOrCK);

            try
            {
                WebRequest req = WebRequest.Create(uri);
                req.Timeout = 15 * 1000;
                req.Method = "GET";
                req.Credentials = GetCredentialsForUser();
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        string xml = reader.ReadToEnd();
                        outputXml = XDocument.Parse(xml);
                    }
                }
            }
            catch
            {
                // *Note*: If the request fails at some point, we could set the EnquirySuccessful flag to false here. 
                // However, Adam has requested that in this case, we cannot be sure whether the TracePS enquiry succeeded or failed,
                // and as such must assume success (The licensee must fit the bill in a case of uncertainty.)

                //results.EnquirySuccessful = false;
                results.ErrorMsg = "Error occurred during service call. Please contact support.";
                return results;
            }
            foreach (var element in outputXml.Descendants("category"))
            {
                if (element.Attribute("name").Value == "contact")
                {
                    results.ContactRows = (from e in element.Descendants("row")
                                           select new ContactRow
                                           {
                                               Phone = e.Descendants("field").Where(f => f.Attribute("name").Value == "phone").FirstOrDefault().Value,
                                               Type = e.Descendants("field").Where(f => f.Attribute("name").Value == "type").FirstOrDefault().Value,
                                               Date = e.Descendants("field").Where(f => f.Attribute("name").Value == "date").FirstOrDefault().Value
                                           }).OrderByDescending(d => 
                                               {
                                                   if (!string.IsNullOrEmpty(d.Date))
                                                   {
                                                       return Convert.ToDateTime(d.Date);
                                                   }
                                                   return DateTime.Now;  // TODO: what the fuck do you do now?
                                               }).ToList();
                }

                if (element.Attribute("name").Value == "person")
                {
                    string surname = element.Descendants().First(s => s.Attribute("name").Value == "surname").Value;
                    string name1 = element.Descendants().First(s => s.Attribute("name").Value == "name1").Value;
                    string name2 = element.Descendants().First(s => s.Attribute("name").Value == "name2").Value;
                    results.OwnerName = string.Concat(name1, " ", name2);
                    results.OwnerSurname = surname;
                }
            }

            string outputXmlString = outputXml.ToString();
            if (outputXmlString.Contains("ERR04") && outputXmlString.Contains("No result found"))
            {
                results.EnquirySuccessful = false;
            }
            if (string.IsNullOrWhiteSpace(results.OwnerName) || results.ContactRows.Count == 0)
            {
                results.ErrorMsg = "No contact information could be found for this property.";
            }

            results.IdCkNo = idOrCK;
            results.OwnerGender = DetermineOwnerGender(idOrCK);
            return results;
        }

        private static string DetermineOwnerGender(string idOrCK)
        {
            try
            {
                if (!string.IsNullOrEmpty(idOrCK) && idOrCK.Length > 7)
                {
                    string G = idOrCK.Substring(6, 1);
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

        private static NetworkCredential GetCredentialsForUser()
        {
            return new NetworkCredential("GEM001", "gEm25m");
        }


        public static List<ProspectingProperty> CreateSectionalTitle(SectionalTitle sectionalTitle)
        {
            // First determine whether the sectional scheme falls within the user's available suburbs by examining the first record.
            // NB.: handle the null case!!
            var latLng = sectionalTitle.Units[0].LatLng;
            if (!ProspectWithinOwnedSuburbs(latLng))
            {
                return new List<ProspectingProperty> { new ProspectingProperty { CreateError = "Cannot create sectional title: The building falls outside your available suburbs." }  };
            }
            List<ProspectingProperty> units = new List<ProspectingProperty>();
            using (var prospecting = new ProspectingDataContext())
            {
                int areaId = prospecting.find_area_id(latLng.Lat, latLng.Lng, "R", null);
                foreach (var unit in sectionalTitle.Units)
                {
                    var newPropRecord = new prospecting_property
                    {
                        lightstone_property_id = unit.LightstonePropertyId,// dataPacket.LightstoneId,
                        latitude = unit.LatLng.Lat,// dataPacket.LatLng.Lat,
                        longitude = unit.LatLng.Lng,
                        property_address = unit.PropertyAddress,// dataPacket.PropertyAddress,
                        street_or_unit_no = unit.StreetOrUnitNo,// dataPacket.StreetOrUnitNo,
                        seeff_area_id = areaId,
                        lightstone_id_or_ck_no = unit.LightstoneIDOrCKNo,// dataPacket.LightstoneIDOrCKNo,
                        lightstone_reg_date = unit.LightstoneRegDate,// dataPacket.LightstoneRegDate,
                        erf_no = unit.ErfNo,// dataPacket.ErfNo,
                        ss_fh = unit.SS_FH,// dataPacket.SS_FH,
                        ss_name = !string.IsNullOrEmpty(unit.SSName) ? unit.SSName : null,
                        ss_number = !string.IsNullOrEmpty(unit.SSNumber) ? unit.SSNumber : null,
                        unit = !string.IsNullOrEmpty(unit.Unit) ? unit.Unit : null,
                        ss_id = !string.IsNullOrEmpty(unit.SS_ID) ? unit.SS_ID : null,
                        last_purch_price = unit.LastPurchPrice
                    };

                    prospecting.prospecting_properties.InsertOnSubmit(newPropRecord);
                    prospecting.SubmitChanges(); // Create the property first before adding contacts

                    //foreach (var owner in unit.Contacts/*.Owners*/)
                    //{
                    //    ContactDataPacket newContact = new ContactDataPacket { ContactPerson = owner, ProspectingPropertyId = newPropRecord.prospecting_property_id };
                    //    SaveContactPerson(newContact);
                    //}

                    //
                    foreach (var owner in unit.Contacts)
                    {
                        if (owner.ContactEntityType == ContactEntityType.NaturalPerson)
                        {
                            var newContact = new ContactDataPacket { ContactPerson = owner, ProspectingPropertyId = newPropRecord.prospecting_property_id, ContactCompanyId = null };
                            SaveContactPerson(newContact);
                        }
                    }

                    foreach (var owner in unit.ContactCompanies)
                    {
                        var newContact = new CompanyContactDataPacket { ContactCompany = (ProspectingContactCompany)owner, ProspectingPropertyId = newPropRecord.prospecting_property_id };
                        SaveContactCompany(newContact);
                    }
                    //

                    var property = CreateProspectingProperty(prospecting, newPropRecord, true);
                    units.Add(property);
                }

                return units;
            }
        }

        public static ProspectingProperty GetProspectingProperty(ProspectingPropertyId dataPacket)
        {
            ProspectingProperty prop = new ProspectingProperty();
            var prospectingDB =  new ProspectingDataContext();
            int prospecting_property_id = (int)(dataPacket.PropertyId);

            var propertiesOwned = from pp in prospectingDB.prospecting_properties
                                  where pp.prospecting_property_id == prospecting_property_id
                                  select pp;
            foreach (var prospectable in propertiesOwned)
            {
                prop =  CreateProspectingProperty(prospectingDB, prospectable, false);
            }
            return prop;
        }

        public static void MarkAsProspected(int propertyId)
        {
            DataSet DS = null;
            string SQL = null;
            SeeffGlobal.clsData da = new SeeffGlobal.clsData();
            SQL = "  SELECT [dbo].[prospected_propety_contact_count] (" + propertyId + ") AS [contact_count] ";
            DS = da.CommandSQL(SQL, "contact_count", (int)(SeeffGlobal.clsData.App.prospecting), HttpContext.Current.Server.MapPath("/data") + "/DBSettings.xml");
            if (!string.IsNullOrEmpty(da.CommandSQLErrMsg))
            {
                //Add error logging
                string test = da.CommandSQLErrMsg;
            }
            else
            {
                foreach (DataRow dtRow in DS.Tables["contact_count"].Rows)
                {
                    string propertyHasContacts = "";
                    if (da.valInteger(dtRow, "contact_count") == 0)
                    { propertyHasContacts = "0"; }
                    else
                    { propertyHasContacts = "1"; };
                    UpdateAsProspected(propertyId, propertyHasContacts);
                }
            }
        }

        public static void UpdateAsProspected(int propertyId, string propertyHasContacts)
        {
            string SQL = null;
            SeeffGlobal.clsData da = new SeeffGlobal.clsData();
            SQL = " UPDATE seeff_prospecting.dbo.prospecting_property ";
            SQL += "   SET prospected = " + propertyHasContacts;
            SQL += " WHERE prospecting_property_id = " + propertyId;
            da.ExecSql(SQL, (int)(SeeffGlobal.clsData.App.prospecting), HttpContext.Current.Server.MapPath("/data") + "/DBSettings.xml");
            if (!string.IsNullOrEmpty(da.CommandSQLErrMsg))
            {
                //Add error logging
                string test = da.CommandSQLErrMsg;
            }
        }
    }
}