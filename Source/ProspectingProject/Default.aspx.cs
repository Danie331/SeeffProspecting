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
            string userGuid = HttpContext.Current.IsDebuggingEnabled ? Request.QueryString["user_guid"] : Request.Form["UserGuidField"];
            Session["user_guid"] = userGuid;
        }
    }
}