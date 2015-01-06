using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
 
 
public partial class MarketShare : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Determine the user permissions based on the incoming GUID
            // In debug (dev) mode always look at the incoming querystring for this value, for production this value is expected to be received via a POST

            string userGuid = (string)Session["user_guid"];

            using (var bossDb = new BossDataContext())
            {
                var user = (from ur in bossDb.user_registrations
                            where ur.user_guid.Equals(userGuid)
                            select ur).FirstOrDefault();
                if (user != null)
                {
                    Response.Write("<input type='hidden' id='user_guid' value='" + userGuid + "' />");
                    Response.Write("<input type='hidden' id='app_context' value='marketshare' />");
                    switch (GetUserType(user))
                    {
                        // Admin
                        case "admin":
                            Response.Write("<input type='hidden' id='user_designation' value='admin' />");
                            break;
                        // Normal user
                        default:
                            Response.Write("<input type='hidden' id='user_designation' value='licensee' />");

                            var availableSuburbs = LoadSuburbInfoForUser(user.ms_area_permissions);
                            Response.Write("<input type='hidden' id='user_suburbs' value='" + availableSuburbs + "' />");

                            var availableAgencies = LoadAllAvailableAgencies();
                            Response.Write("<input type='hidden' id='all_agencies' value='" + availableAgencies + "' />");
                            break;
                    }
                }
            }     
        }
    }

    /// <summary>
    /// Determine what "type" of user this guid belongs to.
    /// </summary>
    private string GetUserType(user_registration user)
    {
        string userFullname = user.user_name + " " + user.user_surname;
        switch (userFullname)
        {
            case "Michael Scott": return "admin";
            default: return "default";
        }
    }

    private string LoadSuburbInfoForUser(string suburbsWithPermissions)
    {
        Regex regexSplitter = new Regex(@"\[(.*?)\]");
        var matches = regexSplitter.Matches(suburbsWithPermissions);
        if (matches.Count > 0)
        {
            var suburbSets = from m in matches.Cast<Match>()
                             let suburbSet = m.Groups[0].Value.Split(new[] { ',' })
                             select new
                             {
                                 AreaId = int.Parse(suburbSet[0].Replace("[", "")),
                                 CanEdit = suburbSet[1].Replace("]", "") == "1"
                             };

            using (var seeffDb = new SeeffDataContext())
            {
                var seeffAreas = (from a in seeffDb.areas select new { a.areaId, a.areaName }).ToList();
                var suburbsInfo = from a in seeffAreas
                                  join ss in suburbSets on a.areaId equals ss.AreaId    
                                  orderby a.areaName
                                  select new
                                  {
                                      SuburbId = a.areaId,
                                      SuburbName = a.areaName,
                                      CanEdit = ss.CanEdit,
                                      Fated = AllListingsFated(a.areaId),// Only true when all listings in this suburb are fated

                                      FatedCount = GetTotalFatings(a.areaId, true),
                                      UnfatedCount = GetTotalFatings(a.areaId, false),
                                      SeeffCurrentListingCount = GetCountSeeffCurrentListings(a.areaId)
                                  };

                return new JavaScriptSerializer().Serialize(suburbsInfo);
            }
        }

        return string.Empty;
    }

    private int GetCountSeeffCurrentListings(int areaId)
    {
        using (var seeff = new SeeffDataContext())
        {
            int count = 0;
            foreach (var seeffListing in seeff.searches.Where(item => item.fkAreaId == areaId))
            {
                Decimal lat, lng;
                if (Domain.ConvertToLatLng(seeffListing.searchLatitude, seeffListing.searchLongitude, out lat, out lng))
                {
                    count += 1;
                }
            }

            return count;
        }
    }

    private int? GetTotalFatings(int areaId, bool getFated)
    {
        using (var ls_base = new LightStoneDataContext())
        {
            var areaFatingRecord = ls_base.area_fatings.Where(af => af.area_id == areaId).FirstOrDefault();
            if (areaFatingRecord != null)
            {
                return getFated ? areaFatingRecord.fated : areaFatingRecord.unfated;
            }
        }

        return null;
    }

    private bool AllListingsFated(int areaId)
    {            
        using (var lsBase = new LightStoneDataContext())
        {
            var results = lsBase.base_datas.Where(a => a.seeff_area_id == areaId).All(a => a.fated.HasValue);
            return results;
        }
    }

    private string LoadAllAvailableAgencies()
    {
        using (var lsbase = new LightStoneDataContext())
        {
            var allAgencies = from agncy in lsbase.agencies
                              orderby agncy.agency_name
                              select new
                                  {
                                       agency_id = agncy.agency_id,
                                       agency_name = agncy.agency_name
                                  };
            return new JavaScriptSerializer().Serialize(allAgencies);
        }
    }
}