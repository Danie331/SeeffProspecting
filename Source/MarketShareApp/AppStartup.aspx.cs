using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AppStartup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (HttpContext.Current.IsDebuggingEnabled)
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
                    //Session["session_key"] = sessionKey;

                    using (var boss = DataManager.DataContextRetriever.GetBossDataContext())
                    {
                        if (boss.user_auth(userGuid, Guid.Parse(sessionKey), "MARKET SHARE") != 1)
                        {
                            throw new Exception("Not authenticated.");
                        }
                    }
                }
                catch
                {
                    Response.Redirect("NotAuthorised.aspx", true);
                }
            }

            DisplayLoadingGif();        
        }
    }

    private void DisplayLoadingGif()
    {
        Response.Write("<div id=\"loading\" style=\"position:absolute; width:100%; text-align:center; top:300px;\"><img id='loadgif' src=\"Assets/loading.gif\" border=0></div>");
    }


}