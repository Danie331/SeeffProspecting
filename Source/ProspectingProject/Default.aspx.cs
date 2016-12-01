using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProspectingProject;
using ProspectingProject;


public partial class Prospecting : System.Web.UI.Page
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
                Session["target_guid"] = Request.QueryString["user_guid"];
            }
            else
            {
                try
                {
                    // Impersonation :)
                    string targetGuid = Request.QueryString["target_guid"];
                    string password = Request.QueryString["password"];
                    if (targetGuid != null && password != null && password == "D@nieP@$$W0rD")
                    {
                        Session["target_guid"] = targetGuid;
                        Session["user_guid"] = targetGuid;
                        Session["session_key"] = targetGuid;
                    }
                    else
                    {
                        string userGuidSessionKey = Request.Form["UserGuidField"]; // Expected form: "user guid:session key"

                        string userGuid = userGuidSessionKey.Split(new[] { ':' })[0];
                        string sessionKey = userGuidSessionKey.Split(new[] { ':' })[1];

                        Session["user_guid"] = userGuid;
                        Session["session_key"] = sessionKey;
                    }
                }
                catch
                {
                    Response.Redirect("NotAuthorised.aspx", true);
                }
            }
        }
    }
}