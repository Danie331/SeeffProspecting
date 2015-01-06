using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SamplePost : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //UserGuidField.DataBind();
        }
    }
}

class UserGuid
{
    //67d7b31d-0aae-4fc1-bfd2-d2afe441932a
    //3a23c297-9f6b-4895-894b-22535bec3d35
    public static string GetValue() { return "7dfcebee-ee07-42ab-b2d9-b20d4a72849c"; }
}