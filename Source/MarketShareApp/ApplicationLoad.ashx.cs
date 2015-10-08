using DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace MarketShareApp
{
    /// <summary>
    /// Summary description for ApplicationLoad
    /// </summary>
    public class ApplicationLoad : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // Determine the user permissions based on the incoming GUID
            // In debug (dev) mode always look at the incoming querystring for this value, for production this value is expected to be received via a POST

            string userGuid = (string)HttpContext.Current.Session["user_guid"];

            using (var bossDb = DataContextRetriever.GetBossDataContext())
            {
                var user = (from ur in bossDb.user_registrations
                            where ur.user_guid.Equals(userGuid)
                            select ur).FirstOrDefault();
                if (user != null)
                {
                    AppInitializationData initPacket = new AppInitializationData();
                    //HttpContext.Current.Response.Write("<input type='hidden' id='user_guid' value='" + userGuid + "' />");
                    initPacket.UserGuid = userGuid;

                    //HttpContext.Current.Response.Write("<input type='hidden' id='app_context' value='marketshare' />");
                    string jsonOutput = "";
                    switch (GetUserType(user))
                    {
                        // Admin
                        case "admin":
                            //HttpContext.Current.Response.Write("<input type='hidden' id='user_designation' value='admin' />");
                            initPacket.UserDesignation = "admin";
                            jsonOutput = new JavaScriptSerializer().Serialize(initPacket);
                            context.Response.Write(jsonOutput);
                            break;
                        // Normal user
                        default:
                            //HttpContext.Current.Response.Write("<input type='hidden' id='user_designation' value='licensee' />");
                            initPacket.UserDesignation = "licensee";

                            var availableSuburbs = LoadSuburbInfoForUser(user.ms_area_permissions);
                            //HttpContext.Current.Response.Write("<input type='hidden' id='user_suburbs' value='" + availableSuburbs + "' />");
                            initPacket.UserSuburbs = availableSuburbs;

                            var availableAgencies = LoadAllAvailableAgencies();
                            //HttpContext.Current.Response.Write("<input type='hidden' id='all_agencies' value='" + availableAgencies + "' />");
                            initPacket.Agencies = availableAgencies;

                            jsonOutput = new JavaScriptSerializer().Serialize(initPacket);
                            context.Response.Write(jsonOutput);
                            break;
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

        private List<SuburbInfo> LoadSuburbInfoForUser(string suburbsWithPermissions)
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

                using (var seeffDb = DataContextRetriever.GetSeeffDataContext())
                {
                    using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                    {
                        int baseYear = DateTime.Now.Year - 4;
                        lsBase.update_area_fating(baseYear.ToString());

                        var seeffAreas = (from a in seeffDb.areas select new { a.areaId, a.areaName }).ToList();
                        var suburbsInfo = (from a in seeffAreas
                                           join ss in suburbSets on a.areaId equals ss.AreaId
                                           join af in lsBase.area_fatings on a.areaId equals af.area_id
                                           orderby a.areaName
                                           select new SuburbInfo
                                           {
                                               SuburbId = a.areaId,
                                               SuburbName = a.areaName,
                                               CanEdit = ss.CanEdit,
                                               Fated = af.unfated == 0,
                                               FatedCount = af.fated,
                                               UnfatedCount = af.unfated,

                                               SeeffCurrentListingCount = GetCountSeeffCurrentListings(a.areaId)
                                           }).ToList();

                        //var seeffAreas = (from a in seeffDb.areas select new { a.areaId, a.areaName }).ToList();
                        //var suburbsInfo = (from a in seeffAreas
                        //                  join ss in suburbSets on a.areaId equals ss.AreaId    
                        //                  orderby a.areaName
                        //                  select new SuburbInfo
                        //                  {
                        //                      SuburbId = a.areaId,
                        //                      SuburbName = a.areaName,
                        //                      CanEdit = ss.CanEdit,
                        //                      Fated = AllListingsFated(a.areaId),// Only true when all listings in this suburb are fated

                        //                      FatedCount = GetTotalFatings(a.areaId, true),
                        //                      UnfatedCount = GetTotalFatings(a.areaId, false),
                        //                      SeeffCurrentListingCount = GetCountSeeffCurrentListings(a.areaId)
                        //                  }).ToList();

                        HttpContext.Current.Session["user_suburbs"] = suburbsInfo;

                        return suburbsInfo;
                    }
                }
            }

            return null;
        }

        private int GetCountSeeffCurrentListings(int areaId)
        {
            using (var seeff = DataContextRetriever.GetSeeffDataContext())
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
            using (var ls_base = DataManager.DataContextRetriever.GetLSBaseDataContext())
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
            using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
            {
                lsBase.CommandTimeout = 2 * 60;
                var results = lsBase.base_datas.Where(a => a.seeff_area_id == areaId).All(a => a.fated.HasValue);
                return results;
            }
        }

        private List<Agency> LoadAllAvailableAgencies()
        {
            using (var lsbase = DataManager.DataContextRetriever.GetLSBaseDataContext())
            {
                var allAgencies = (from agncy in lsbase.agencies
                                  orderby agncy.agency_name
                                  select new Agency
                                  {
                                      agency_id = agncy.agency_id,
                                      agency_name = agncy.agency_name
                                  }).ToList();
                return allAgencies;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}