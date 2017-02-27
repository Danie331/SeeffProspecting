using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class AppInitializationData
{
    public string UserGuid { get; set; }
    public string AppContext { get { return "marketshare"; } }
    public string UserDesignation { get; set; }
    public List<SuburbInfo> UserSuburbs { get; set; }
    public List<Agency> Agencies { get; set; }

    public bool Authenticated { get; set; }
    public string AuthMessage { get; set; }
    public string AdminUserList { get; internal set; }
}