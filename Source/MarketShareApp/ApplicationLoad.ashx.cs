using DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Configuration;

namespace MarketShareApp
{
    /// <summary>
    /// Summary description for ApplicationLoad
    /// </summary>
    public class ApplicationLoad : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            AppInitializationData initPacket = new AppInitializationData() { Authenticated = true };
            try
            {
                string jsonOutput = "";
                if (HttpContext.Current.Session["user_guid"] == null || HttpContext.Current.Session["session_key"] == null)
                {
                    throw new Exception("You are not authorised to access this resource");
                }
                string userGuid = (string)HttpContext.Current.Session["user_guid"];
                string sessionKey = (string)HttpContext.Current.Session["session_key"];

                // Success
                initPacket.AdminUserList = System.Configuration.ConfigurationManager.AppSettings["add_agency_permission"]; ;

                initPacket.UserGuid = userGuid;
                initPacket.UserDesignation = "licensee";

                string areaPermissionsList = (string)HttpContext.Current.Session["area_permissions_list"];
                var availableSuburbs = LoadSuburbInfoForUser(areaPermissionsList);
                initPacket.UserSuburbs = availableSuburbs;

                var availableAgencies = LoadAllAvailableAgencies();
                initPacket.Agencies = availableAgencies;
                jsonOutput = new JavaScriptSerializer().Serialize(initPacket);
                context.Response.Write(jsonOutput);
            }
            catch (Exception ex)
            {
                initPacket.Authenticated = false;
                initPacket.AuthMessage = ex.Message;
                context.Response.Write(new JavaScriptSerializer().Serialize(initPacket));
            }
        }

        private List<SuburbInfo> LoadSuburbInfoForUser(string suburbsWithPermissions)
        {
            Regex regexSplitter = new Regex(@"\[(.*?)\]");
            var matches = regexSplitter.Matches(suburbsWithPermissions);
            if (matches.Count > 0)
            {
                var suburbSets = (from m in matches.Cast<Match>()
                                 let suburbSet = m.Groups[0].Value.Split(new[] { ',' })
                                 select new
                                 {
                                     AreaId = int.Parse(suburbSet[0].Replace("[", "")),
                                     CanEdit = suburbSet[1].Replace("]", "") == "1"
                                 }).GroupBy(s => s.AreaId).Select(g => g.First());


                    using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                    {
                        int baseYear = DateTime.Now.Year - 4;
                        lsBase.update_area_fating(baseYear.ToString());

                    var spatialReader = new DataManager.SeeffSpatial.SpatialServiceReader();
                    var spatialSuburbsList = spatialReader.SuburbsListOnly();
                    var suburbsInfo = (from a in spatialSuburbsList
                                           join ss in suburbSets on a.SeeffAreaID equals ss.AreaId
                                           join af in lsBase.area_fatings on a.SeeffAreaID equals af.area_id
                                           orderby a.AreaName
                                           let suburbID = a.SeeffAreaID.HasValue ? a.SeeffAreaID.Value : -1
                                       where !a.IsDeleted
                                       select new SuburbInfo
                                           {
                                               SuburbId = suburbID,
                                               SuburbName = a.AreaName,
                                               CanEdit = ss.CanEdit,
                                               Fated = af.unfated == 0,
                                               FatedCount = af.fated,
                                               UnfatedCount = af.unfated,
                                               SeeffCurrentListingCount = GetCountSeeffCurrentListings(suburbID)
                                           }).ToList();

                        HttpContext.Current.Session["user_suburbs"] = suburbsInfo;

                        return suburbsInfo;
                    }
                }

            return null;
        }

        private int GetCountSeeffCurrentListings(int areaId)
        {
            if (areaId == -1)
                return 0;

            using (var ls_base = DataContextRetriever.GetLSBaseDataContext())
            {
                int count = 0;
                foreach (var seeffListing in ls_base.seeff_searches.Where(item => item.fkAreaId == areaId))
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