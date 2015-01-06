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
            string userGuid = HttpContext.Current.IsDebuggingEnabled ? Request.QueryString["user_guid"] : Request.Form["UserGuidField"];
            Session["user_guid"] = userGuid;

            DisplayLoadingGif();        
        }
    }

    private void DisplayLoadingGif()
    {
        Response.Write("<div id=\"loading\" style=\"position:absolute; width:100%; text-align:center; top:300px;\"><img id='loadgif' src=\"Assets/loading.gif\" border=0></div>");
    }


}