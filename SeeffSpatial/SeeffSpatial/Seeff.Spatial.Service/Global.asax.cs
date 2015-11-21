using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Seeff.Spatial.Service
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(System.Web.Http.Validation.IBodyModelValidator), new Seeff.Spatial.Service.BusinessLayer.Utils.CustomBodyModelValidator());
            GlobalConfiguration.Configure(WebApiConfig.Register);
            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
        }
    }
}
