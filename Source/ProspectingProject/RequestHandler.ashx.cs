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
            request = ProspectingCore.Deserialise<BaseDataRequestPacket>(json);
            try
            {
                // This handles the authorisation request
                if (request.Instruction == "load_application")
                {
                    var loadData = LoadApplication();
                    context.Response.Write(loadData);
                    return;
                }

                // This handles un-authenticated requests, ie public requests
                if (request.Instruction == "contact_optout")
                {
                    VerifyOptoutContact(json);
                    return;
                }

                // This line will ensure that the user has a valid session before making any of the following requests - none should be publicly accessible. 
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
                    errorJSON = ProspectingCore.SerializeToJsonWithDefaults(sessionExpired);
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
                    errorJSON = ProspectingCore.SerializeToJsonWithDefaults(errorObject);
                    context.Response.Write(errorJSON);
                }
            }
        }

        private void VerifyOptoutContact(string json)
        {
            try
            {
                var request = ProspectingCore.Deserialise<ContactOptoutRequest>(json);
                ProspectingCore.VerifyOptoutContact(request);
            }
            catch
            {
                // We should not worry about errors that may occur here, because this method can be invoked from a public location and out-of-session.
            }
        }

        private string RetrieveUserSignature()
        {
            var sig = ProspectingCore.RetrieveUserSignature();
            return ProspectingCore.SerializeToJsonWithDefaults(sig);
        }

        private void SaveCommunicationRecord(string json)
        {
            var commObject = ProspectingCore.Deserialise<CommunicationRecord>(json);
            ProspectingCore.SaveCommunicationRecord(commObject);
        }

        private string LoadProperties(string json)
        {
            var inputProperties = ProspectingCore.Deserialise<LightstonePropertyIDPacket>(json);
            var results = ProspectingCore.LoadProperties(inputProperties.LightstonePropertyIDs);
            return ProspectingCore.SerializeToJsonWithDefaults(new { Properties = results });
        }

        private string SendSMS(string json)
        {
            SmsInputPacket inputPacket = ProspectingCore.Deserialise<SmsInputPacket>(json);
            return ProspectingCore.SendSMS(inputPacket);
        }

        private int? FindAreaId(string json)
        {
            GeoLocation location = ProspectingCore.Deserialise<GeoLocation>(json);
            return ProspectingCore.FindAreaId(location);
        }

        private void UnlockCurrentProspectingRecord()
        {
            ProspectingCore.UnlockCurrentProspectingRecord();
        }

        private string LoadActivitiesForUser(string json)
        {
            var results = ProspectingCore.LoadUserActivities();
            return ProspectingCore.SerializeToJsonWithDefaults(results);
        }

        private void MakeDefaultContactDetail(string json)
        {
            ProspectingContactDetail detail = ProspectingCore.Deserialise<ProspectingContactDetail>(json);
            ProspectingCore.MakeDefaultContactDetail(detail.ItemId);
        }

        private string GetFollowUps(string json)
        {
            UserDataResponsePacket user = GetUserSessionObject();
            ExistingFollowups existingItems = ProspectingCore.Deserialise<ExistingFollowups>(json);
            var results = ProspectingCore.LoadFollowups(user.UserGuid, user.BusinessUnitUsers, existingItems.ExistingFollowupItems).Followups;
            return ProspectingCore.SerializeToJsonWithDefaults(results);
        }

        private string GetActivityLookupData()
        {
            var results = ProspectingCore.GetActivityLookupData();
            return ProspectingCore.SerializeToJsonWithDefaults(results);
        }

        private void SaveActivity(string json)
        {
            ProspectingActivity act = ProspectingCore.Deserialise<ProspectingActivity>(json);
            ProspectingCore.UpdateInsertActivity(act);
        }

        private void UpdateProspectedStatus(string json)
        {
            var prospectingPropertyStatus = ProspectingCore.Deserialise<PropertyProspectedStatus>(json);
            ProspectingCore.MarkAsProspected(prospectingPropertyStatus.LightstonePropertyId, prospectingPropertyStatus.Prospected);
        }

        private string CreateProspectingEntities(string json)
        {
            var prospectingEntityBundle = ProspectingCore.Deserialise<ProspectingEntityInputBundle>(json);
            List<NewProspectingEntity> searchResults = (List<NewProspectingEntity>)HttpContext.Current.Session["ProspectingEntities"];
            var results = ProspectingCore.CreateNewProspectingEntities(prospectingEntityBundle, searchResults);
            return ProspectingCore.SerializeToJsonWithDefaults(results);
        }

        private string GetProspectedProperty(string json)
        {
            var propertyId = ProspectingCore.Deserialise<ProspectingPropertyId>(json);
            ProspectingProperty Prop = ProspectingCore.GetProspectingProperty(propertyId);
            return ProspectingCore.SerializeToJsonWithDefaults(Prop);
        }

        private string SearchForPropertiesWithMatchingDetails(string json)
        {
            var searchInputValues = ProspectingCore.Deserialise<SearchInputPacket>(json);
            var searchResults = ProspectingCore.FindMatchingProperties(searchInputValues);
            HttpContext.Current.Session["ProspectingEntities"] = null;
            HttpContext.Current.Session["ProspectingEntities"] = searchResults;
            return ProspectingCore.SerializeToJsonWithDefaults(searchResults);
        }

        private string SearchForExistingContactWithDetails(string json)
        {
            var contactDetails = ProspectingCore.Deserialise<ContactDetails>(json);
            ProspectingContactPerson contactPerson = ProspectingCore.SearchForExistingContactWithDetails(contactDetails);
            return ProspectingCore.SerializeToJsonWithDefaults(contactPerson);     
        }

        private string LoadProspectingSuburb(string json)
        {
            var suburbDataRequest = ProspectingCore.Deserialise<SuburbDataRequestPacket>(json);
            ProspectingSuburb suburb = ProspectingCore.LoadProspectingSuburb(suburbDataRequest);           
            return ProspectingCore.SerializeToJsonWithDefaults(suburb);
        }

        private string SaveContact(string json)
        {
            var contactDataPacket = ProspectingCore.Deserialise<ContactDataPacket>(json);
            var contact = ProspectingCore.SaveContactPerson(contactDataPacket);

            int deletedContactDetailsCount = Convert.ToInt32(HttpContext.Current.Session["deleted_item_count"]);
            if (deletedContactDetailsCount > 40)
            {
                UserDataResponsePacket user = GetUserSessionObject();
                if (!user.IsProspectingManager)
                {
                    contact.ContactIsCompromised = true;
                    ProspectingCore.SendWarningNotificationToManager(user);
                    HttpContext.Current.Session["user_guid"] = null;
                }
            }

            return ProspectingCore.SerializeToJsonWithDefaults(contact);
        }

        private string UpdateProspectingProperty(string json)
        {
            ProspectingInputData dataPacket = ProspectingCore.Deserialise<ProspectingInputData>(json);            
            ProspectingCore.UpdateProspectingRecord(dataPacket);
            return ProspectingCore.SerializeToJsonWithDefaults("success");
        }

        private string LoadMatchingLightstoneAddresses(string json)
        {
            ProspectingInputData dataPacket = ProspectingCore.Deserialise<ProspectingInputData>(json);
            var matches = ProspectingCore.GetMatchingAddresses(dataPacket);
            HttpContext.Current.Session["ProspectingEntities"] = null;
            HttpContext.Current.Session["ProspectingEntities"] = matches;
            return ProspectingCore.SerializeToJsonWithDefaults(matches);
        }

        private string LookupPersonDetails(string json)
        {
            ProspectingInputData dataPacket = ProspectingCore.Deserialise<ProspectingInputData>(json);
            PersonEnquiryResponsePacket results = ProspectingCore.PerformLookup(dataPacket);
            return ProspectingCore.SerializeToJsonWithDefaults(results);            
        }

        private string LoadApplication()
        {
            var guid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            var sessionKey = Guid.Parse((string)HttpContext.Current.Session["session_key"]);

            UserDataResponsePacket user = ProspectingCore.LoadUser(guid, sessionKey);
            HttpContext.Current.Session["user"] = user;
            HttpContext.Current.Session["deleted_item_count"] = 0;
            return ProspectingCore.SerializeToJsonWithDefaults(user);
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

