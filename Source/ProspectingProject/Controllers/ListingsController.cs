
using ProspectingProject.Controllers.Models;
using ProspectingProject.Services.PropertyListingService;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.SessionState;

namespace ProspectingProject.Controllers
{
    // TODO:
    // 1. enable email sending for go-live
    // 2. update endpoints to prod for go-live
    // 3. activity polling for updates to listings
    // 4. approach required to keep lookup tables in sync
    // 5. purge db of staging data for go-live
    // 6. multi-user login???
    public class ListingsController : ApiController
    {
        private PropertyListingService _propertyListingService;
        public ListingsController()
        {
            _propertyListingService = new PropertyListingService();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetLookupData([FromUri]int seeffAreaId)
        {
            try
            {
                var locations = await _propertyListingService.GetP24SuburbsAsync(seeffAreaId);
                var agents = await _propertyListingService.GetAgentsAsync();

                return Ok(new { Locations = locations, Agents = agents });
            }
            catch (Exception ex)
            {
                LogException(ex, "GetLookupData(...)");
                return new ExceptionResult(ex, this);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAgentBranches([FromUri]int agentId)
        {
            try
            {
                var agentBranches = await _propertyListingService.GetBranchesAsync(agentId);
                return Ok(new { Branches = agentBranches });
            }
            catch(Exception ex)
            {
                LogException(ex, "GetAgentBranches(...)");
                return new ExceptionResult(ex, this);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateResidentialListing(NewResidentialListingModel listingModel)
        {
            try
            {
                var result = await _propertyListingService.CreateResidentialListingAsync(listingModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogException(ex, "CreateResidentialListing(...)");
                return new ExceptionResult(ex, this);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateCommercialListing(NewCommercialListingModel listingModel)
        {
            try
            {
                var result = await _propertyListingService.CreateCommercialListingAsync(listingModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogException(ex, "CreateCommercialListing(...)");
                return new ExceptionResult(ex, this);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateDevelopmentsListing(NewDevelopmentsListingModel listingModel)
        {
            try
            {
                var result = await _propertyListingService.CreateDevelopmentsListingAsync(listingModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogException(ex, "CreateDevelopmentsListing(...)");
                return new ExceptionResult(ex, this);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateHolidayListing(NewHolidayListingModel listingModel)
        {
            try
            {
                var result = await _propertyListingService.CreateHolidayListingAsync(listingModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogException(ex, "CreateHolidayListing(...)");
                return new ExceptionResult(ex, this);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetListing([FromUri]int listingId)
        {
            try
            {
                var result = await _propertyListingService.GetListingAsync(listingId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogException(ex, "GetListing(...)");
                return new ExceptionResult(ex, this);
            }
        }

        private void LogException(Exception ex, string methodName)
        {
            try
            {
                using (var prospectingDb = new ProspectingDataContext())
                {
                    var friendlyErrorMsg = $"Exception occurred in ListingsController > {methodName}: {ex.Message}";
                    var errorRec = new exception_log
                    {
                        friendly_error_msg = friendlyErrorMsg,
                        exception_string = ex.ToString(),
                        user = RequestHandler.GetUserSessionObject().UserGuid,
                        date_time = DateTime.Now
                    };
                    prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                    prospectingDb.SubmitChanges();
                }
            }
            catch { }
        }
    }
}
