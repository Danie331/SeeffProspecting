using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.SessionState;

namespace ProspectingProject
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public override void Init()
        {
            this.PostAuthenticateRequest += (o, e) =>
            {
                System.Web.HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            };
            base.Init();
        }
    }
}