using ProspectingProject.Services.PropertyListingService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using ProspectingProject.Services.Propdata;
using ProspectingProject.Services.Propdata.Models;
using ProspectingProject.Controllers;
using ProspectingProject.Controllers.Models;

namespace ProspectingProject.Services.PropertyListingService
{
    public class PropertyListingService
    {
        private PropdataApiWrapper _propdataApi;

        public PropertyListingService()
        {
            _propdataApi = new PropdataApiWrapper();
        }

        public async Task<List<Suburb>> GetP24SuburbsAsync(int seeffAreaId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var suburbs = prospecting.propdata_suburb_lookups.Where(sl => sl.seeff_suburb_id == seeffAreaId).ToList();
                var p24SuburbIds = suburbs.Where(d => d.p24_suburb_id.HasValue).Select(p => p.p24_suburb_id.Value).ToList();
                if (p24SuburbIds.Count == 0)
                {
                    throw new Exception("Unable to find a corresponding P24 suburb ID for this suburb - please contact Support.");
                }

                var locationResults = await _propdataApi.GetLocationsFromP24IDs(p24SuburbIds);

                return await Task.FromResult((from s in locationResults
                                              select new Suburb
                                              {
                                                  LocationId = s.id,
                                                  LocationArea = s.area,
                                                  LocationSuburb = s.suburb
                                              }).ToList());
            }
        }

        public async Task<List<PropdataBranch>> GetBranchesAsync(int agentId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var currentUser = RequestHandler.GetUserSessionObject();
                var targetAgent = _propdataApi.GetAgent(agentId);
                if (targetAgent == null)
                {
                    throw new Exception($"Unable to retrieve agent with id {agentId} from session - please contact Support.");
                }

                var agentBranches = new List<PropdataBranch>();
                var branchRecords = prospecting.propdata_branch_lookups.Where(pb => targetAgent.branches.Contains(pb.propdata_branch_id)).ToList();
                var propdataBranches = branchRecords.Select(p => p.propdata_branch_id).Distinct();
                if (!targetAgent.branches.All(i => propdataBranches.Contains(i)))
                {
                    throw new Exception($"Un-mapped branches found in Propdata/branch correlation table: {string.Join(",", targetAgent.branches)} - please contact Seeff Support");
                }

                foreach (var item in branchRecords)
                {
                    agentBranches.Add(new PropdataBranch { Id = item.propdata_branch_id, BranchName = item.propdata_branch_name });
                }

                return await Task.FromResult(agentBranches.OrderBy(br => br.BranchName).ToList());
            }
        }

        public async Task<Listing> GetListingAsync(int listingId)
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var targetRecord = prospecting.prospecting_property_listings.Where(li => li.prospecting_property_listing_id == listingId).First();
                return await Task.FromResult(new Listing
                {
                    Status = targetRecord.listing_status,
                    Url = targetRecord.propdata_listing_url
                });
            }
        }

        public async Task<ListingResult> CreateHolidayListingAsync(NewHolidayListingModel listingModel)
        {
            var result = await _propdataApi.CreateHolidayListing(listingModel);
            var targetAgentEmail = _propdataApi.GetAgentEmail(result.agent);
            result.ActiveListingId = SaveListingDetails("Holiday", targetAgentEmail, listingModel, result);
            NotifyAgent(targetAgentEmail, result.URL);
            return result;
        }

        public async Task<ListingResult> CreateResidentialListingAsync(NewResidentialListingModel listingModel)
        {
            var result = await _propdataApi.CreateResidentialListing(listingModel);
            var targetAgentEmail = _propdataApi.GetAgentEmail(result.agent);
            result.ActiveListingId = SaveListingDetails("Residential", targetAgentEmail, listingModel, result);
            NotifyAgent(targetAgentEmail, result.URL);
            return result;
        }

        public async Task<ListingResult> CreateDevelopmentsListingAsync(NewDevelopmentsListingModel listingModel)
        {
            var result = await _propdataApi.CreateDevelopmentsListing(listingModel);
            var targetAgentEmail = _propdataApi.GetAgentEmail(result.agent);
            result.ActiveListingId = SaveListingDetails("Developments", targetAgentEmail, listingModel, result);
            NotifyAgent(targetAgentEmail, result.URL);
            return result;
        }

        public async Task<ListingResult> CreateCommercialListingAsync(NewCommercialListingModel listingModel)
        {
            var result = await _propdataApi.CreateCommercialListing(listingModel);
            var targetAgentEmail = _propdataApi.GetAgentEmail(result.agent);
            result.ActiveListingId = SaveListingDetails("Commercial", targetAgentEmail, listingModel, result);
            NotifyAgent(targetAgentEmail, result.URL);
            return result;
        }

        public async Task<List<Agent>> GetAgentsAsync()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                var currentUser = RequestHandler.GetUserSessionObject();
                var branchRecords = prospecting.propdata_branch_lookups.Where(p => p.seeff_branch_id == currentUser.BranchID)
                                                                       .ToList();

                var agents = await _propdataApi.GetAgentsAsync(branchRecords.Select(p => p.propdata_branch_id).Distinct().ToList());
                if (agents.Count == 0)
                {
                    throw new Exception($"Unable to retrieve agents for Seeff branch(es) {string.Join(",", branchRecords.Select(p => p.seeff_branch_id).Distinct())} from API - please contact Support.");
                }

                return agents.OrderBy(a => a.first_name).ThenBy(a => a.last_name).ToList();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ////
        //// PRIVATE METHODS
        ////
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void NotifyAgent(string emailAddress, string listingUrl)
        {
            return;

            var msgBody = "A new listing has been assigned to you from Prospecting";
            msgBody += "<p>";
            msgBody += $"Please access the listing here: <a href='{listingUrl}' target='_blank'>{listingUrl}</a>";
            ProspectingCore.SendEmail(emailAddress, "Seeff Prospecting", "reports@seeff.com", null, null, "New Listing Created", msgBody);
        }

        private int SaveListingDetails(string listedAs, string targetAgentEmail, IListingBaseModel requestObject, ListingResult resultObject)
        {
            try
            {
                var activityType = ProspectingLookupData.SystemActivityTypes.First(act => act.Value == "New Listing Created").Key;
                Guid targetAgentGuid = Guid.Empty;
                using (var authService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
                {
                    targetAgentGuid = authService.GetUserGuidByEmail(targetAgentEmail);
                }

                var newListingActivity = new ProspectingActivity
                {
                    IsForInsert = true,
                    LightstonePropertyId = requestObject.LightstoneId,
                    FollowUpDate = DateTime.Now,
                    AllocatedTo = targetAgentGuid,
                    ActivityTypeId = activityType,
                    Comment = $"A new listing has been created for {ProspectingCore.GetFormattedAddress(requestObject.LightstoneId)} and assigned to {targetAgentEmail}.<br />" +
                              $"Listing URL: <a href='{resultObject.URL}' target='_blank'>{resultObject.URL}</a><br />",
                };
                var activityLogId = ProspectingCore.UpdateInsertActivity(newListingActivity);
                using (var db = new ProspectingDataContext())
                {
                    var newListing = new prospecting_property_listing
                    {
                        request_payload = requestObject.ToJsonString(),
                        lightstone_property_id = requestObject.LightstoneId,
                        listed_as = listedAs,
                        listing_status = resultObject.status ?? (resultObject.active ? "Active" : "active: False"),
                        propdata_listing_id = resultObject.id,
                        propdata_listing_url = resultObject.URL,
                        propdata_response_payload = resultObject.JsonPayload,
                        last_activity_id = activityLogId,
                        created_by_user = RequestHandler.GetUserSessionObject().UserGuid,
                        created_date = DateTime.Now,
                        updated_date = DateTime.Now
                    };
                    db.prospecting_property_listings.InsertOnSubmit(newListing);
                    db.SubmitChanges();

                    var listingId = newListing.prospecting_property_listing_id;
                    var targetProperty = db.prospecting_properties.Where(pp => pp.lightstone_property_id == requestObject.LightstoneId).First();
                    targetProperty.active_listing_id = listingId;
                    db.SubmitChanges();

                    return listingId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while saving listing details to the database. Please access your listing from this URL: {resultObject.URL}, and report this error to Seeff Support.", ex);
            }
        }
    }
}