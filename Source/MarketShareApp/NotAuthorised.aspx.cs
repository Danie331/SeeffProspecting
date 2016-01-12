using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MarketShareApp
{
    public partial class NotAuthorised : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string message = Request.QueryString["error"];
                if (!string.IsNullOrEmpty(message))
                {
                    Response.Write("<h3 style='font-color:red'>" + message + "</h3>");
                }
            }
        }
    }
}