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
            string json = context.Request.Form[0];
            BaseDataRequestPacket request = ProspectingDomain.Deserialise<BaseDataRequestPacket>(json);
            switch (request.Instruction)
            {
                case "load_application":
                    var loadData = LoadApplication();
                    context.Response.Write(loadData);
                    break;
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
                case "save_property_notes":
                    var success = SavePropertyNotesComments(json);
                    context.Response.Write(success);
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
            }
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

        private string SavePropertyNotesComments(string json)
        {
            var propNotesContainer = ProspectingDomain.Deserialise<PropertyCommentsNotes>(json);
            ProspectingDomain.SavePropertyNotesComments(propNotesContainer);
            return ProspectingDomain.SerializeToJsonWithDefaults("success");            
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
            return ProspectingDomain.SerializeToJsonWithDefaults(contact);
        }

        private string UpdateProspectingProperty(string json)
        {
            ProspectingPropertyInputData dataPacket = ProspectingDomain.Deserialise<ProspectingPropertyInputData>(json);            
            ProspectingDomain.UpdateProspectingRecord(dataPacket);
            return ProspectingDomain.SerializeToJsonWithDefaults("success");
        }

        private string LoadMatchingLightstoneAddresses(string json)
        {
            ProspectingPropertyInputData dataPacket = ProspectingDomain.Deserialise<ProspectingPropertyInputData>(json);
            var matches = ProspectingDomain.GetMatchingAddresses(dataPacket);
            HttpContext.Current.Session["ProspectingEntities"] = null;
            HttpContext.Current.Session["ProspectingEntities"] = matches;
            return ProspectingDomain.SerializeToJsonWithDefaults(matches);
        }

        private string LookupPersonDetails(string json)
        {
            ProspectingPropertyInputData dataPacket = ProspectingDomain.Deserialise<ProspectingPropertyInputData>(json);
            ProspectingDataResponsePacket results = ProspectingDomain.PerformLookupTransaction(dataPacket);
            return ProspectingDomain.SerializeToJsonWithDefaults(results);            
        }

        private string LoadApplication()
        {
            var guid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            UserDataResponsePacket user = ProspectingDomain.LoadUser(guid);                           
            return ProspectingDomain.SerializeToJsonWithDefaults(user);
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

