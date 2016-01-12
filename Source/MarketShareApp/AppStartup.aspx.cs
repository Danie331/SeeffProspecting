using System;
using System.Web;

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
                using (var authService = new MarketShareApp.AuthService.SeeffProspectingAuthServiceClient())
                {
                    var userAuthPacket = authService.AuthenticateMSUser(Guid.Parse((string)Session["user_guid"]), Guid.Parse((string)Session["session_key"]));
                    Session["area_permissions_list"] = userAuthPacket.AreaPermissionsList;
                }
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

                    using (var authService = new MarketShareApp.AuthService.SeeffProspectingAuthServiceClient())
                    {
                        var userAuthPacket = authService.AuthenticateMSUser(Guid.Parse(userGuid), Guid.Parse(sessionKey));
                        if (!userAuthPacket.Authenticated)
                        {
                            throw new Exception(userAuthPacket.AuthMessage);
                        }
                        else
                        {
                            Session["area_permissions_list"] = userAuthPacket.AreaPermissionsList;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errMessage = ex.Message;
                    Response.Redirect("NotAuthorised.aspx?error=" + errMessage, true);
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