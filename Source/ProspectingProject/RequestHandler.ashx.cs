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
                try
                {
                    json = context.Request.Form[0];
                }
                catch (Exception ex)
                {
                    json = HttpUtility.UrlDecode(context.Request.Form[0]);
                }
                request = ProspectingCore.Deserialise<BaseDataRequestPacket>(json);

                // This handles the authorisation request
                if (request.Instruction == "load_application")
                {
                    var loadData = LoadApplication();
                    context.Response.Write(loadData);
                    return;
                }

                // This line will ensure that the user has a valid session before making any of the following requests - none should be publicly accessible. 
                GetUserSessionObject();
                switch (request.Instruction)
                {
                    case "server_tap": // purely to allow client a moment to breathe so we can refresh the UI
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
                        var seeffArea = FindAreaId(json);
                        context.Response.Write(seeffArea);
                        break;
                    case "load_properties":
                        var properties = LoadProperties(json);
                        context.Response.Write(properties);
                        break;
                    case"retrieve_user_signature":
                        string userSignature = RetrieveUserSignature();
                        context.Response.Write(userSignature);
                        break;
                    case "submit_sms":
                        string response = SubmitSMSBatch(json);
                        context.Response.Write(response);
                        break;
                    case "submit_emails":
                        var status = SubmitEmailBatch(json);
                        context.Response.Write(status);
                        break;
                    case "get_template":
                        string template = GetTemplate(json);
                        context.Response.Write(template);
                        break;
                    case "add_update_template":
                        AddOrUpdateTemplate(json);
                        break;
                    case "delete_template":
                        DeleteTemplate(json);
                        break;
                    case "get_user_template_list":
                        string userTemplates = GetUserTemplates(json);
                        context.Response.Write(userTemplates);
                        break;
                    case "get_system_template_list":
                        string sysTemplates = GetSystemTemplates(json);
                        context.Response.Write(sysTemplates);
                        break;
                    case "calculate_cost_email_batch":
                        string result = CalculateCostOfEmailBatch(json);
                        context.Response.Write(result);
                        break;
                    case "calculate_cost_sms_batch":
                         result = CalculateCostOfSmsBatch(json);
                        context.Response.Write(result);
                        break;
                    case "update_property_ownership":
                        string updatedProperty = UpdatePropertyOwnership(json);
                        context.Response.Write(updatedProperty);
                        break;
                    case "create_valuation":
                        CreateValuation(json);
                        break;
                    case "load_valuations":
                        string valuations = LoadValuations(json);
                        context.Response.Write(valuations);
                        break;
                    case "delete_valuation":
                        DeleteValuation(json);
                        break;
                    case"perform_company_enquiry":
                        string enquiryResults = PerformCompanyEnquiry(json);
                        context.Response.Write(enquiryResults);
                        break;
                    case "load_comm_report":
                        string messagesReport = LoadCommunicationData(json);
                        context.Response.Write(messagesReport);
                        break;
                    case "validate_person_id":
                        string valid = ValidatePersonIdNumber(json);
                        context.Response.Write(valid);
                        break;
                    case "enable_suburb_filtering":
                        EnableFilteringForSuburb(json);
                        break;
                    case "generate_referral_details":
                        string referralDetails = GenerateReferralDetails(json);
                        context.Response.Write(referralDetails);
                        break;
                    case "create_referral":
                        string referralOutput = CreateReferral(json);
                        context.Response.Write(referralOutput);
                        break;
                    case "get_referrals_history_for_property":
                        string referralsHistoryForProperty = GetReferralsHistoryForProperty(json);
                        context.Response.Write(referralsHistoryForProperty);
                        break;
                    case "update_do_not_contact_status":
                        UpdateDoNotContactStatusForContact(json);
                        break;
                    case "perform_trust_enquiry":
                        string trustEnquiryResponse = PerformTrustEnquiry(json);
                        context.Response.Write(trustEnquiryResponse);
                        break;
                    case "perform_trust_enquiry_get_trustees":
                        string trusteesResponse = PerformTrustEnquiryGetTrustees(json);
                        context.Response.Write(trusteesResponse);
                        break;
                    case "update_company_reg_no":
                        string updatedCompany = UpdateCompanyRegNo(json);
                        context.Response.Write(updatedCompany);
                        break;
                    case "filter_activities_followups_for_business_unit":
                        string activitiesFollowupsFilteringResult = FilterActivitiesFollowupsForBusinessUnit(json);
                        context.Response.Write(activitiesFollowupsFilteringResult);
                    //    break;
                    //case "load_current_mandate_set":
                    //    string mandateSet = LoadCurrentMandateSet(json);
                    //    context.Response.Write(mandateSet);
                    //    break;
                    //case "load_mandate_lookup_data":
                    //    string mandateData = LoadMandateLookupData();
                    //    context.Response.Write(mandateData);
                    //    break;
                    //case "save_mandate":
                    //    string mandateSaveResult = SaveMandate(json);
                    //    context.Response.Write(mandateSaveResult);
                        break;
                    case "update_company_name":
                        string updatedCompanyName = UpdateCompanyName(json);
                        context.Response.Write(updatedCompanyName);
                        break;
                    case "generate_pseudo_identifier":
                        string identifier = GeneratePseudoIdentifier();
                        context.Response.Write(identifier);
                        break;
                    case "send_email_opt_in_request":
                        string requestStatus = SendOptInRequest(json);
                        context.Response.Write(requestStatus);
                        break;
                    case "retrieve_lists_for_branch":
                        string listsForBranch = RetrieveListsForBranch(json);
                        context.Response.Write(listsForBranch);
                        break;
                    case "save_lists_for_contact":
                        string listSaveResult = SaveListAllocationForContact(json);
                        context.Response.Write(listSaveResult);
                        break;
                    case "save_lists_for_selection":
                        string multiListSaveResult = SaveSelectionToSelectedLists(json);
                        context.Response.Write(multiListSaveResult);
                        break;
                    case "export_list":
                        string exportListResult = ExportList(json);
                        context.Response.Write(exportListResult);
                        break;
                    case "retrieve_list_types":
                        string listTypes = RetrieveListTypes();
                        context.Response.Write(listTypes);
                        break;
                    case "create_list":
                        string createListResult = CreateNewListForBranch(json);
                        context.Response.Write(createListResult);
                        break;
                    case "delete_list":
                        string listDeleteResult = DeleteList(json);
                        context.Response.Write(listDeleteResult);
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

        private string DeleteList(string json)
        {
            var input = ProspectingCore.Deserialise<ContactList>(json);
            var result = ProspectingCore.DeleteList(input);
            return ProspectingCore.SerializeToJsonWithDefaults(result);
        }

        private string CreateNewListForBranch(string json)
        {
            var input = ProspectingCore.Deserialise<ContactList>(json);
            var createListResult = ProspectingCore.CreateNewListForBranch(input);
            return ProspectingCore.SerializeToJsonWithDefaults(createListResult);
        }

        private string RetrieveListTypes()
        {
            var types = ProspectingCore.RetrieveListTypes();
            return ProspectingCore.SerializeToJsonWithDefaults(types);
        }

        private string ExportList(string json)
        {
            var export = ProspectingCore.Deserialise<ListExportSelection>(json);
            var result = ProspectingCore.ExportList(export);
            return ProspectingCore.SerializeToJsonWithDefaults(result);
        }

        private string SaveSelectionToSelectedLists(string json)
        {
            var listSelection = ProspectingCore.Deserialise<MultiContactListSelection>(json);
            var result = ProspectingCore.SaveSelectionToSelectedLists(listSelection);
            return ProspectingCore.SerializeToJsonWithDefaults(result);
        }

        private string SaveListAllocationForContact(string json)
        {
            var listSelection = ProspectingCore.Deserialise<ContactListSelection>(json);
            var result = ProspectingCore.SaveListAllocationForContact(listSelection);
            return ProspectingCore.SerializeToJsonWithDefaults(result);
        }

        private string RetrieveListsForBranch(string json)
        {
            ProspectingContactPerson cp = ProspectingCore.Deserialise<ProspectingContactPerson>(json);
            var listsForBranch = ProspectingCore.RetrieveListsForBranch(cp);
            return ProspectingCore.SerializeToJsonWithDefaults(listsForBranch);
        }

        private string SendOptInRequest(string json)
        {
            var contact = ProspectingCore.Deserialise<ProspectingContactPerson>(json);
            var status = ProspectingCore.CreateOptInRequestForEmailComms(contact);
            return ProspectingCore.SerializeToJsonWithDefaults(status);
        }

        private string GeneratePseudoIdentifier()
        {
            string result = ProspectingCore.GeneratePseudoIdentifier();
            GeneratedIdentifier newIDContainer = new GeneratedIdentifier { GeneratedID = result };
            return Newtonsoft.Json.JsonConvert.SerializeObject(newIDContainer);
        }

        //private string SaveMandate(string json)
        //{
        //    var mandateInputs = ProspectingCore.Deserialise<NewMandateInputs>(json);
        //    var responseData = ProspectingCore.SaveMandate(mandateInputs);
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
        //}

        //private string LoadMandateLookupData()
        //{
        //    var mandateLookupDataPacket = ProspectingCore.LoadMandateLookupData();
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(mandateLookupDataPacket);
        //}

        //private string LoadCurrentMandateSet(string json)
        //{
        //    var lightstoneRecord = ProspectingCore.Deserialise<ProspectingPropertyId>(json);
        //    var responsePacket = ProspectingCore.LoadCurrentMandateSet(lightstoneRecord.LightstonePropertyId);
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(responsePacket);
        //}


        private string UpdateCompanyName(string json)
        {
            var companyWithNewName = ProspectingCore.Deserialise<ProspectingContactCompany>(json);
            companyWithNewName = ProspectingCore.UpdateCompanyName(companyWithNewName);
            return ProspectingCore.SerializeToJsonWithDefaults(companyWithNewName);
        }

        private string FilterActivitiesFollowupsForBusinessUnit(string json)
        {
            var activitiesFollowupsFilterInput = ProspectingCore.Deserialise<ActivitiesFollowupsFilterInputs>(json);
            var responsePacket = ProspectingCore.FilterActivitiesFollowupsForBusinessUnit(activitiesFollowupsFilterInput);
            return Newtonsoft.Json.JsonConvert.SerializeObject(responsePacket);
        }

        private string UpdateCompanyRegNo(string json)
        {
            var companyWithNewReg = ProspectingCore.Deserialise<ProspectingContactCompany>(json);
            companyWithNewReg = ProspectingCore.UpdateCompanyRegNo(companyWithNewReg);
            return ProspectingCore.SerializeToJsonWithDefaults(companyWithNewReg);
        }

        private string PerformTrustEnquiryGetTrustees(string json)
        {
            var trustHash = ProspectingCore.Deserialise<TrustHashcodeInput>(json);
            var responsePacket = ProspectingCore.GetTrustees(trustHash);
            return ProspectingCore.SerializeToJsonWithDefaults(responsePacket);
        }

        private string PerformTrustEnquiry(string json)
        {
            var enquiryPacket = ProspectingCore.Deserialise<CompanyEnquiryInputPacket>(json);
            var responsePacket = ProspectingCore.PerformTrustSearch(enquiryPacket);
            return ProspectingCore.SerializeToJsonWithDefaults(responsePacket);
        }

        private void UpdateDoNotContactStatusForContact(string json)
        {
            var inputPacket = ProspectingCore.Deserialise<ProspectingContactPerson>(json);
            ProspectingCore.UpdateDoNotContactStatusForContact(inputPacket);
        }

        private string GetReferralsHistoryForProperty(string json)
        {
            var inputPacket = ProspectingCore.Deserialise<ProspectingPropertyId>(json);
            ReferralsHistory referralsHistory = ProspectingCore.RetrieveReferralsHistoryForProperty(inputPacket);
            return ProspectingCore.SerializeToJsonWithDefaults(referralsHistory);
        }

        private string CreateReferral(string json)
        {
            var inputPacket = ProspectingCore.Deserialise<ReferralInputDetails>(json);
            ReferralResponseObject details = ProspectingCore.CreateReferral(inputPacket);
            return ProspectingCore.SerializeToJsonWithDefaults(details);
        }

        private string GenerateReferralDetails(string json)
        {
            var inputPacket = ProspectingCore.Deserialise<ReferralInputDetails>(json);
            ReferralResponseObject details = ProspectingCore.GenerateReferralDetails(inputPacket);
            return ProspectingCore.SerializeToJsonWithDefaults(details);
        }

        private void EnableFilteringForSuburb(string json)
        {
            var suburbContainer = ProspectingCore.Deserialise<UserSuburb>(json);
            ProspectingCore.UpdateStatisticsForSuburb(suburbContainer.SuburbId);
        }

        private string ValidatePersonIdNumber(string json)
        {
            string idNumber = ProspectingCore.Deserialise<ProspectingContactPerson>(json).IdNumber;
            var validationResult = ProspectingCore.HasValidSAIdentityNumber(idNumber);
            return ProspectingCore.SerializeToJsonWithDefaults(validationResult);
        }

        private string LoadCommunicationData(string json)
        {
            var filterPacket = ProspectingCore.Deserialise<CommReportFilters>(json);
            var results = ProspectingCore.LoadCommunicationData(filterPacket);
            return ProspectingCore.SerializeToJsonWithDefaults(results);
        }

        private string PerformCompanyEnquiry(string json)
        {
            var enquiryPacket = ProspectingCore.Deserialise<CompanyEnquiryInputPacket>(json);
            var responsePacket = ProspectingCore.PerformCompanyEnquiry(enquiryPacket);
            return ProspectingCore.SerializeToJsonWithDefaults(responsePacket);
        }

        private void DeleteValuation(string json)
        {
             var valuation = ProspectingCore.Deserialise<PropertyValuation>(json);
             ProspectingCore.DeleteValuation(valuation);
        }

        private string LoadValuations(string json)
        {
            var prospectingProperty = ProspectingCore.Deserialise<ProspectingPropertyId>(json);
            var valuations = ProspectingCore.LoadValuations(prospectingProperty.ProspectingPropertyID.Value);
            return ProspectingCore.SerializeToJsonWithDefaults(valuations);
        }

        private void CreateValuation(string json)
        {
            var valuation = ProspectingCore.Deserialise<PropertyValuation>(json);
            ProspectingCore.CreateValuation(valuation);
        }

        private string UpdatePropertyOwnership(string json)
        {
            var property = ProspectingCore.Deserialise<ProspectingPropertyId>(json);
            bool updated = ProspectingCore.UpdatePropertyOwnership(property);
            return ProspectingCore.SerializeToJsonWithDefaults(updated);
        }

        private string CalculateCostOfSmsBatch(string json)
        {
            var batch = ProspectingCore.Deserialise<SmsBatch>(json);
            var calculationResult = ProspectingCore.CalculateCostOfSmsBatch(batch);
            return ProspectingCore.SerializeToJsonWithDefaults(calculationResult);
        }

        private string CalculateCostOfEmailBatch(string json)
        {
            var batch = ProspectingCore.Deserialise<EmailBatch>(json);
            var calculationResult = ProspectingCore.CalculateCostOfEmailBatch(batch);
            return ProspectingCore.SerializeToJsonWithDefaults(calculationResult);
        }

        private string SubmitEmailBatch(string json)
        {
            var emailBatch = ProspectingCore.Deserialise<EmailBatch>(json);
            var status = ProspectingCore.SubmitEmailBatch(emailBatch);
            return ProspectingCore.SerializeToJsonWithDefaults(status);
        }

        private string GetSystemTemplates(string json)
        {
            var templateRequest = ProspectingCore.Deserialise<CommTemplateRequest>(json);
            var results = ProspectingCore.GetListOfSystemTemplates(templateRequest);
            return ProspectingCore.SerializeToJsonWithDefaults(results);
        }

        private string GetUserTemplates(string json)
        {
            var templateRequest = ProspectingCore.Deserialise<CommTemplateRequest>(json);
            var results = ProspectingCore.GetListOfUserTemplates(templateRequest);
            return ProspectingCore.SerializeToJsonWithDefaults(results);
        }

        private void DeleteTemplate(string json)
        {
            var templateRequest = ProspectingCore.Deserialise<CommTemplateRequest>(json);
            ProspectingCore.DeleteTemplate(templateRequest);
        }

        private void AddOrUpdateTemplate(string json)
        {
            var templateRequest = ProspectingCore.Deserialise<CommTemplateRequest>(json);
            ProspectingCore.AddOrUpdateTemplate(templateRequest);
        }

        private string GetTemplate(string json)
        {
            var templateRequest = ProspectingCore.Deserialise<CommTemplateRequest>(json);
            var result = ProspectingCore.GetTemplate(templateRequest);
            return ProspectingCore.SerializeToJsonWithDefaults(result);
        }

        private string RetrieveUserSignature()
        {
            var sig = ProspectingCore.RetrieveUserSignature();
            return ProspectingCore.SerializeToJsonWithDefaults(sig);
        }

        private string LoadProperties(string json)
        {
            var inputProperties = ProspectingCore.Deserialise<LightstonePropertyIDPacket>(json);
            var results = ProspectingCore.LoadProperties(inputProperties.LightstonePropertyIDs);
            return ProspectingCore.SerializeToJsonWithDefaults(new { Properties = results });
        }

        private string SubmitSMSBatch(string json)
        {
            var smsBatch = ProspectingCore.Deserialise<SmsBatch>(json);
            var status = ProspectingCore.SubmitSMSBatch(smsBatch);
            return ProspectingCore.SerializeToJsonWithDefaults(status);
        }

        private string FindAreaId(string json)
        {
            GeoLocation location = ProspectingCore.Deserialise<GeoLocation>(json);
            var result = ProspectingCore.FindAreaId(location);
            return ProspectingCore.SerializeToJsonWithDefaults(result);
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
            bool impersonate = HttpContext.Current.Session["target_guid"] != null;

            var guid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            var sessionKey = Guid.Parse((string)HttpContext.Current.Session["session_key"]);

            UserDataResponsePacket user = ProspectingCore.LoadUser(guid, sessionKey, impersonate);
            user.IsTrainingMode = IsTrainingMode();
            HttpContext.Current.Session["user"] = user;
            HttpContext.Current.Session["deleted_item_count"] = 0;
            return ProspectingCore.SerializeToJsonWithDefaults(user);
        }

        public static bool IsTrainingMode()
        {
            if (HttpContext.Current.Session["training_mode"] != null)
            {
                return (bool)HttpContext.Current.Session["training_mode"];
            }

            return false;
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

