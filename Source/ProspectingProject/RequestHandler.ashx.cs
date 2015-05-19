using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace ProspectingProject
{
    /// <summary>
    /// This class handles ajax requests from the frontend.
    /// Don't put any business logic directly in here, put it into a method in ProspectingDomain.cs, and call it from here.
    /// </summary>
    public class RequestHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            BaseDataRequestPacket request = null;
            string json = string.Empty;
            // This is a POST from an internal request
            try
            {
                json = context.Request.Form[0];
            }
            catch
            {
                json = HttpUtility.UrlDecode(context.Request.Form[0]);
            }
            request = ProspectingDomain.Deserialise<BaseDataRequestPacket>(json);
            try
            {
                if (request.Instruction == "load_application")
                {
                    var loadData = LoadApplication();
                    context.Response.Write(loadData);
                    return;
                }

                // This line will ensure that the user has a valid session before continuing the request. 
                GetUserSessionObject();
                switch (request.Instruction)
                {
                    case "get_prop_owner_details":
                        var ownerDetailsResults = LookupPersonDetails(json);
                        context.Response.Write(ownerDetailsResults);
                        break;
                    case "get_matching_addresses":
                        var matchingAddressesFromLightstone = LoadMatchingLightstoneAddresses(json);
                        context.Response.Write(matchingAddressesFromLightstone);
                        break;
                    case "update_prospecting_property":
                        var updatedProspectResult = UpdateProspectingProperty(json);
                        context.Response.Write(updatedProspectResult);
                        break;
                    case "save_contact":
                        var newContact = SaveContact(json);
                        context.Response.Write(newContact);
                        break;
                    case "load_suburb":
                        var suburb = LoadProspectingSuburb(json);
                        context.Response.Write(suburb);
                        break;
                    case "check_for_existing_contact":
                        var existingContact = SearchForExistingContactWithDetails(json);
                        context.Response.Write(existingContact);
                        break;
                    case "search_for_matches":
                        var results = SearchForPropertiesWithMatchingDetails(json);
                        context.Response.Write(results);
                        break;
                    case "get_existing_prospecting_property":
                        var existingProspect = GetProspectedProperty(json);
                        context.Response.Write(existingProspect);
                        break;
                    case "create_new_prospects":
                        var prospectingEntities = CreateProspectingEntities(json);
                        context.Response.Write(prospectingEntities);
                        break;
                    case "update_prospected_flag":
                        UpdateProspectedStatus(json);
                        break;
                    case "save_activity":
                        SaveActivity(json);
                        break;
                    case "load_activity_lookup_data":
                        var activityLookupData = GetActivityLookupData();
                        context.Response.Write(activityLookupData);
                        break;
                    case "load_followups":
                        var followups = GetFollowUps(json);
                        context.Response.Write(followups);
                        break;
                    case "make_default_contact_detail":
                        MakeDefaultContactDetail(json);
                        break;
                    case "load_activities_for_user":
                        var activities = LoadActivitiesForUser(json);
                        context.Response.Write(activities);
                        break;
                    case "unlock_prospecting_record":
                        UnlockCurrentProspectingRecord();
                        break;
                    case "find_area_id":
                        int? seeffAreaId = FindAreaId(json);
                        context.Response.Write(seeffAreaId);
                        break;
                    case "load_properties":
                        var properties = LoadProperties(json);
                        context.Response.Write(properties);
                        break;
                    case "save_communication":
                        SaveCommunicationRecord(json);
                        break;
                    case"retrieve_user_signature":
                        string userSignature = RetrieveUserSignature();
                        context.Response.Write(userSignature);
                        break;
                    case "send_sms":
                        string response = SendSMS(json);
                        context.Response.Write(response);
                        break;
                }
            }
            catch (Exception ex)
            {
                string errorJSON = string.Empty;
                // Session Exception - handle on front-end
                if (ex is UserSessionExpiredException)
                {
                    var sessionExpired = new { SessionExpired = true };
                    errorJSON = ProspectingDomain.SerializeToJsonWithDefaults(sessionExpired);
                    context.Response.Write(errorJSON);
                }
                else
                {
                    // Other excepion - log + display on front-end + contact support 
                    using (var prospectingDb = new ProspectingDataContext())
                    {
                        var errorRec = new exception_log
                        {
                            friendly_error_msg = ex.Message,
                            exception_string = ex.ToString(),
                            user = GetUserSessionObject().UserGuid,
                            date_time = DateTime.Now
                        };
                        prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                        prospectingDb.SubmitChanges();
                    }
                    var errorObject = new { ErrorMessage = ex.Message };
                    errorJSON = ProspectingDomain.SerializeToJsonWithDefaults(errorObject);
                    context.Response.Write(errorJSON);
                }
            }
        }

        private string RetrieveUserSignature()
        {
            var sig = ProspectingDomain.RetrieveUserSignature();
            return ProspectingDomain.SerializeToJsonWithDefaults(sig);
        }

        private void SaveCommunicationRecord(string json)
        {
            var commObject = ProspectingDomain.Deserialise<CommunicationRecord>(json);
            ProspectingDomain.SaveCommunicationRecord(commObject);
        }

        private string LoadProperties(string json)
        {
            var inputProperties = ProspectingDomain.Deserialise<LightstonePropertyIDPacket>(json);
            var results = ProspectingDomain.LoadProperties(inputProperties.LightstonePropertyIDs);
            return ProspectingDomain.SerializeToJsonWithDefaults(new { Properties = results });
        }

        private string SendSMS(string json)
        {
            SmsInputPacket inputPacket = ProspectingDomain.Deserialise<SmsInputPacket>(json);
            return ProspectingDomain.SendSMS(inputPacket);
        }

        private int? FindAreaId(string json)
        {
            GeoLocation location = ProspectingDomain.Deserialise<GeoLocation>(json);
            return ProspectingDomain.FindAreaId(location);
        }

        private void UnlockCurrentProspectingRecord()
        {
            ProspectingDomain.UnlockCurrentProspectingRecord();
        }

        private string LoadActivitiesForUser(string json)
        {
            var results = ProspectingDomain.LoadUserActivities();
            return ProspectingDomain.SerializeToJsonWithDefaults(results);
        }

        private void MakeDefaultContactDetail(string json)
        {
            ProspectingContactDetail detail = ProspectingDomain.Deserialise<ProspectingContactDetail>(json);
            ProspectingDomain.MakeDefaultContactDetail(detail.ItemId);
        }

        private string GetFollowUps(string json)
        {
            UserDataResponsePacket user = GetUserSessionObject();
            ExistingFollowups existingItems = ProspectingDomain.Deserialise<ExistingFollowups>(json);
            var results = ProspectingDomain.LoadFollowups(user.UserGuid, user.BusinessUnitUsers, existingItems.ExistingFollowupItems).Followups;
            return ProspectingDomain.SerializeToJsonWithDefaults(results);
        }

        private string GetActivityLookupData()
        {
            var results = ProspectingDomain.GetActivityLookupData();
            return ProspectingDomain.SerializeToJsonWithDefaults(results);
        }

        private void SaveActivity(string json)
        {
            ProspectingActivity act = ProspectingDomain.Deserialise<ProspectingActivity>(json);
            ProspectingDomain.UpdateInsertActivity(act);
        }

        private void UpdateProspectedStatus(string json)
        {
            var prospectingPropertyStatus = ProspectingDomain.Deserialise<PropertyProspectedStatus>(json);
            ProspectingDomain.MarkAsProspected(prospectingPropertyStatus.LightstonePropertyId, prospectingPropertyStatus.Prospected);
        }

        private string CreateProspectingEntities(string json)
        {
            var prospectingEntityBundle = ProspectingDomain.Deserialise<ProspectingEntityInputBundle>(json);
            List<NewProspectingEntity> searchResults = (List<NewProspectingEntity>)HttpContext.Current.Session["ProspectingEntities"];
            var results = ProspectingDomain.CreateNewProspectingEntities(prospectingEntityBundle, searchResults);
            return ProspectingDomain.SerializeToJsonWithDefaults(results);
        }

        private string GetProspectedProperty(string json)
        {
            var propertyId = ProspectingDomain.Deserialise<ProspectingPropertyId>(json);
            ProspectingProperty Prop = ProspectingDomain.GetProspectingProperty(propertyId);
            return ProspectingDomain.SerializeToJsonWithDefaults(Prop);
        }

        private string SearchForPropertiesWithMatchingDetails(string json)
        {
            var searchInputValues = ProspectingDomain.Deserialise<SearchInputPacket>(json);
            var searchResults = ProspectingDomain.FindMatchingProperties(searchInputValues);
            HttpContext.Current.Session["ProspectingEntities"] = null;
            HttpContext.Current.Session["ProspectingEntities"] = searchResults;
            return ProspectingDomain.SerializeToJsonWithDefaults(searchResults);
        }

        private string SearchForExistingContactWithDetails(string json)
        {
            var contactDetails = ProspectingDomain.Deserialise<ContactDetails>(json);
            ProspectingContactPerson contactPerson = ProspectingDomain.SearchForExistingContactWithDetails(contactDetails);
            return ProspectingDomain.SerializeToJsonWithDefaults(contactPerson);     
        }

        private string LoadProspectingSuburb(string json)
        {
            var suburbDataRequest = ProspectingDomain.Deserialise<SuburbDataRequestPacket>(json);
            ProspectingSuburb suburb = ProspectingDomain.LoadProspectingSuburb(suburbDataRequest);           
            return ProspectingDomain.SerializeToJsonWithDefaults(suburb);
        }

        private string SaveContact(string json)
        {
            var contactDataPacket = ProspectingDomain.Deserialise<ContactDataPacket>(json);
            var contact = ProspectingDomain.SaveContactPerson(contactDataPacket);

            int deletedContactDetailsCount = Convert.ToInt32(HttpContext.Current.Session["deleted_item_count"]);
            if (deletedContactDetailsCount > 40)
            {
                UserDataResponsePacket user = GetUserSessionObject();
                if (!user.IsProspectingManager)
                {
                    contact.ContactIsCompromised = true;
                    ProspectingDomain.SendWarningNotificationToManager(user);
                    HttpContext.Current.Session["user_guid"] = null;
                }
            }

            return ProspectingDomain.SerializeToJsonWithDefaults(contact);
        }

        private string UpdateProspectingProperty(string json)
        {
            ProspectingInputData dataPacket = ProspectingDomain.Deserialise<ProspectingInputData>(json);            
            ProspectingDomain.UpdateProspectingRecord(dataPacket);
            return ProspectingDomain.SerializeToJsonWithDefaults("success");
        }

        private string LoadMatchingLightstoneAddresses(string json)
        {
            ProspectingInputData dataPacket = ProspectingDomain.Deserialise<ProspectingInputData>(json);
            var matches = ProspectingDomain.GetMatchingAddresses(dataPacket);
            HttpContext.Current.Session["ProspectingEntities"] = null;
            HttpContext.Current.Session["ProspectingEntities"] = matches;
            return ProspectingDomain.SerializeToJsonWithDefaults(matches);
        }

        private string LookupPersonDetails(string json)
        {
            ProspectingInputData dataPacket = ProspectingDomain.Deserialise<ProspectingInputData>(json);
            PersonEnquiryResponsePacket results = ProspectingDomain.PerformLookup(dataPacket);
            return ProspectingDomain.SerializeToJsonWithDefaults(results);            
        }

        private string LoadApplication()
        {
            var guid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            var sessionKey = Guid.Parse((string)HttpContext.Current.Session["session_key"]);

            UserDataResponsePacket user = ProspectingDomain.LoadUser(guid, sessionKey);
            HttpContext.Current.Session["user"] = user;
            HttpContext.Current.Session["deleted_item_count"] = 0;
            return ProspectingDomain.SerializeToJsonWithDefaults(user);
        }

        public static UserDataResponsePacket GetUserSessionObject()
        {
            try
            {
                UserDataResponsePacket user = HttpContext.Current.Session["user"] as UserDataResponsePacket;
                if (user == null)
                {
                    throw new UserSessionExpiredException();
                }

                return user;
            }
            catch
            {
                throw new UserSessionExpiredException();
            }
        } 

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

          
    }
}

