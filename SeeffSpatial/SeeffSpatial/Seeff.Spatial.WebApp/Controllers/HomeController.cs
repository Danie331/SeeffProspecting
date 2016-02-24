using Newtonsoft.Json.Serialization;
using Seeff.Spatial.WebApp.BusinessLayer;
using Seeff.Spatial.WebApp.BusinessLayer.ControllerActions;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace Seeff.Spatial.WebApp.Controllers
{
    public class HomeController : ApiController, IRequiresSessionState
    {
        [HttpPost]
        public UserModel Login()
        {
            try
            {
                Guid userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
                Guid sessionKey = Guid.Parse((string)HttpContext.Current.Session["session_key"]);
                using (var authService = new SpatialAuthService.SeeffProspectingAuthServiceClient())
                {
                    var authResult = authService.AuthenticateAndLoadSpatialUser(userGuid, sessionKey);
                    if (authResult.Authenticated)
                    {
                        //var msAreas = Utils.ParseAreaListString(authResult.MarketShareSuburbsList);
                        //var prospectingAreas = Utils.ParseAreaListString(authResult.ProspectingSuburbsList);
                        //var targetAreas = msAreas.Union(prospectingAreas);

                        UserModel user = new UserModel();
                        user.SeeffAreaCollection = new List<SeeffSuburb>();// GlobalAreaCache.Instance.AllSuburbs; // TODO (user areas only)
                        user.SeeffLicenses = GlobalAreaCache.Instance.SeeffLicenses;
                        user.SeeffTerritories = GlobalAreaCache.Instance.SeeffTerritories;
                        user.LoginSuccess = true;

                        return user;
                    }
                    else
                    {
                        return new UserModel { LoginSuccess = false, LoginMessage = authResult.AuthMessage };
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "Login", ""); // TODO: add exception handling all over the show.
                return new UserModel { LoginSuccess = false, LoginMessage = ex.ToString() };
            }
        }

        [HttpGet]
        public IList<SeeffSuburb> GetSuburbs()
        {
            return GlobalAreaCache.Instance.AllSuburbs;
        }

        [HttpPost]
        public AreaValidationResult ValidateSuburb([FromBody]SeeffSuburb suburbFromFrontEnd)
        {
            // NB: CHECK RULES!!!!!!
            try
            {
                var validationResult = ControllerActions.ValidateSuburb(suburbFromFrontEnd);
                return validationResult;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "ValidateSuburb()", suburbFromFrontEnd);
                return new AreaValidationResult { IsValid = false, ValidationMessage = ex.Message };
            }
        }

        [HttpPost]
        public SaveSuburbResult SaveSuburb([FromBody]SeeffSuburb suburbFromFrontEnd)
        {
            try
            {
                var saveResult = ControllerActions.SaveSuburb(suburbFromFrontEnd);
                return saveResult;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "SaveSuburb()", suburbFromFrontEnd);
                return new SaveSuburbResult { Successful = false, SaveMessage = ex.Message };
            }
        }

        [HttpGet]
        public UnmappedSuburbs RetrieveUnmappedSuburbs()
        {
            try
            {
                var unmappedSubs = ControllerActions.RetrieveUnmappedSuburbs();
                return unmappedSubs;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "RetrieveUnmappedSuburbs()", null);
                return new UnmappedSuburbs { Successful = false, ErrorMessage = ex.Message };
            }
        }

        [HttpPost]
        public AreaValidationResult ValidateLicense([FromBody]SeeffLicense licenseFromFrontEnd)
        {
            try
            {
                var validationResult = ControllerActions.ValidateLicense(licenseFromFrontEnd);
                return validationResult;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "ValidateLicense()", licenseFromFrontEnd);
                return new AreaValidationResult { IsValid = false, ValidationMessage = ex.Message };
            }
        }

        [HttpPost]
        public SaveLicenseResult SaveLicense([FromBody]SeeffLicense licenseFromFrontEnd)
        {
            try
            {
                var saveResult = ControllerActions.SaveLicense(licenseFromFrontEnd);
                return saveResult;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "SaveLicense()", licenseFromFrontEnd);
                return new SaveLicenseResult { Successful = false, SaveMessage = ex.Message };
            }
        }

        [HttpPost]
        public DeleteSuburbResult DeleteSuburb([FromBody]SeeffSuburb suburb)
        {
            try
            {
                var result = ControllerActions.DeleteSuburb(suburb);
                return result;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "DeleteSuburb() from Home Controller", suburb);
                return new DeleteSuburbResult { Successful = false, Message = ex.Message };
            }
        }

        [HttpPost]
        public LicenseNameResult GetLicenseName([FromBody]SeeffLicense license)
        {
            try
            {
                var result = ControllerActions.GetLicenseName(license);
                return result;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetLicenseName() from Home Controller", license);
                return new LicenseNameResult { Successful = false, Message = ex.Message };
            }
        }

        [HttpPost]
        public OrphanedPropertiesResult GetOrphanedProperties([FromBody]SeeffLicense license)
        {
            try
            {
                var result = ControllerActions.GetOrphanedProperties(license);
                return result;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetOrphanedProperties() from HomeController", license);
                return new OrphanedPropertiesResult { Successful = false, Message = ex.Message };
            }
        }

        [HttpPost]
        public ExportLicenseModelResult ExportLicenseToKML([FromBody] ExportLicenseModel exportModel)
        {
            try
            {
                var result = ControllerActions.GenerateKMLExport(exportModel);
                return result;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "ExportLicenseToKML() from HomeController", exportModel);
                return new ExportLicenseModelResult { Successful = false, Message = ex.Message + " " + WindowsIdentity.GetCurrent().Name };
            }
        }

        [HttpGet]
        public SuburbsUnderMaintenanceResult GetSuburbsUnderMaintenance()
        {
            try
            {
                var result = ControllerActions.GetSuburbsUnderMaintenance();
                return result;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetSuburbsUnderMaintenance() from HomeController", null);
                return new SuburbsUnderMaintenanceResult { Successful = false, Message = ex.Message };
            }
        }
    }
}
