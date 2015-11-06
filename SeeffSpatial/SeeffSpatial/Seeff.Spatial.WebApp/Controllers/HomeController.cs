using Seeff.Spatial.WebApp.BusinessLayer;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace Seeff.Spatial.WebApp.Controllers
{
    public class HomeController : ApiController, IRequiresSessionState
    {
        [HttpPost]
        public SpatialUser Login()
        {
            Guid userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            Guid sessionKey = Guid.Parse((string)HttpContext.Current.Session["session_key"]);
            try
            {
                //using (var authService = new SpatialAuthService.SeeffProspectingAuthServiceClient())
                //{
                    //var authResult = authService.AuthenticateAndLoadSpatialUser(userGuid, sessionKey);
                    if (/*authResult.Authenticated*/true)
                    {
                        //var msAreas = Utils.ParseAreaListString(authResult.MarketShareSuburbsList);
                        //var prospectingAreas = Utils.ParseAreaListString(authResult.ProspectingSuburbsList);
                        //var targetAreas = msAreas.Union(prospectingAreas);

                        SpatialUser user = new SpatialUser();
                        user.SeeffAreaCollection = GlobalAreaManager.Instance.AllSuburbs; // TODO (user areas only)
                        user.LoginSuccess = true;

                        return user;
                    }
                    else
                    {
                        return new SpatialUser { LoginSuccess = false, /*LoginMessage = authResult.AuthMessage*/ };
                    }
                //}
            }
            catch (Exception ex)
            {
                var userCredentails = new
                {
                    UserGuid = userGuid,
                    SessionKey = sessionKey
                };
                Utils.LogException(ex, "Login", userCredentails); // TODO: add exception handling all over the show.
                return new SpatialUser { LoginSuccess = false, LoginMessage = ex.Message };
            }
        }
    }
}
