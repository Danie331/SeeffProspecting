using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Seeff.Spatial.WebApp.UserInterface
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool debugFlag = Request.QueryString["debug"] != null;
                if (HttpContext.Current.IsDebuggingEnabled || debugFlag)
                {
                    Session["user_guid"] = Request.QueryString["user_guid"];
                    Session["session_key"] = Guid.NewGuid().ToString();
                }
                else
                {
                    try
                    {
                        string userGuidSessionKey = Request.Form["UserGuidField"]; // Expected form: "user guid:session key"

                        string userGuid = userGuidSessionKey.Split(new[] { ':' })[0];
                        string sessionKey = userGuidSessionKey.Split(new[] { ':' })[1];

                        Session["user_guid"] = userGuid;
                        Session["session_key"] = sessionKey;
                    }
                    catch
                    {
                        Response.Redirect("NotAuthorised.aspx", true);
                    }
                }
            }
        }
    }
}