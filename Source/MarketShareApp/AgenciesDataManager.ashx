<%@ WebHandler Language="C#" Class="AgenciesDataManager" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Linq;
using System.Collections.Generic;

public class AgenciesDataManager : IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    public void ProcessRequest (HttpContext context)
    {
        var json = context.Request.Form[0];
        DataRequestPacket requestPacket = (DataRequestPacket)new JavaScriptSerializer().Deserialize(json, typeof(DataRequestPacket));

        switch (requestPacket.instruction)
        {
            case "load":
                var selectedAgencies = LoadAgenciesAvailableToUserSuburbs(requestPacket);
                var serialiser = new JavaScriptSerializer();
                context.Response.Write(serialiser.Serialize(selectedAgencies));
                break;
            case "save":
                string results = SaveSelectedAgencies(requestPacket);
                context.Response.Write(results);
                break;
            default: break;
        }
    }

    private int[] LoadAgenciesAvailableToUserSuburbs(DataRequestPacket inputPacket)
    {
        // TODO: currently looking at a table in the boss database. The final table schema and location to be determined
        using (var lsbase = DataManager.DataContextRetriever.GetLSBaseDataContext())
        {
            List<SuburbInfo> userSuburbs = HttpContext.Current.Session["user_suburbs"] as List<SuburbInfo>;
            var suburbIds = userSuburbs.Select(s => s.SuburbId);
            var resultSet1 = (from b in lsbase.agencies_user_suburbs
                             where suburbIds.Contains(b.suburb_id) && b.agency_id != null
                             select b.agency_id.Value).Distinct();

            var resultSet2 = (from b in lsbase.base_datas
                             where b.seeff_area_id != null && suburbIds.Contains(b.seeff_area_id.Value) && b.agency_id != null
                             select b.agency_id.Value).Distinct();

            return resultSet1.Union(resultSet2).ToArray();
        }
    }

    private string SaveSelectedAgencies(DataRequestPacket inputPacket)
    {
        MarketShareApp.SaveSelectedAgenciesResponsePacket results = new MarketShareApp.SaveSelectedAgenciesResponsePacket();
        List<SuburbInfo> userSuburbs = HttpContext.Current.Session["user_suburbs"] as List<SuburbInfo>;
        using (var lsbase = DataManager.DataContextRetriever.GetLSBaseDataContext())
        {
            // Step 1: find agency IDs allocated to transactions within user suburbs
            var userSuburbIDs = userSuburbs.Select(i => i.SuburbId).ToList();
            var allocatedAgencies = (from b in lsbase.base_datas
                                     where b.seeff_area_id != null && userSuburbIDs.Contains(b.seeff_area_id.Value) && b.agency_id != null
                                     select b.agency_id).Distinct().ToList();

            inputPacket.selectedAgencies = inputPacket.selectedAgencies.Distinct().ToArray();
            var agenciesToExclude = lsbase.agencies.Where(a => new[] { "Seeff", "Private sale", "Auction" }.Contains(a.agency_name)).Select(a => a.agency_id);
            bool alreadyMapped = false;
            foreach (var item in allocatedAgencies)
            {
                if (agenciesToExclude.Contains(item.Value)) continue;
                if (!inputPacket.selectedAgencies.Contains(item.Value))
                {
                    alreadyMapped = true;
                    break;
                }
            }
            if (alreadyMapped)
            {

                // First find all distinct agencies allocated to transactions for this user's licence            
                //List < int > agencyIds = new List<int>();
                //foreach (var suburb in userSuburbs)
                //{
                //    var agencyIdsForSuburb = from b in lsbase.base_datas
                //                             where b.seeff_area_id == suburb.SuburbId && b.agency_id != null
                //                             select b.agency_id.Value;
                //    agencyIds.AddRange(agencyIdsForSuburb);
                //}
                //var agenciesToExclude = lsbase.agencies.Where(a => new[] { "Seeff", "Private sale", "Auction" }.Contains(a.agency_name)).Select(a => a.agency_id);
                //agencyIds = agencyIds.Except(agenciesToExclude).Distinct().ToList();

                //int mismatchCount = agencyIds.Except(inputPacket.selectedAgencies).Count();
                //if (mismatchCount != 0)
                //{
                // If there are transactions in the database assigned an agency not in the input, then we cannot saved until the user has removed the dependency from the listing.
                results.Saved = false;
                return new JavaScriptSerializer().Serialize(results);
            }
            else
            {
                var allRecordsForUserSuburbs = from a in lsbase.agencies_user_suburbs
                                               where userSuburbs.Select(s => s.SuburbId).Contains(a.suburb_id)
                                               select a;

                if (allRecordsForUserSuburbs.Count() == 0)
                {
                    // Insert records for this suburb because it doesn't yet exist
                    foreach (int id in inputPacket.selectedAgencies)
                    {
                        foreach (var suburb in userSuburbs)
                        {
                            var newRecord = new agencies_user_suburb { suburb_id = suburb.SuburbId, agency_id = id, updated_by = inputPacket.userGuid };
                            lsbase.agencies_user_suburbs.InsertOnSubmit(newRecord);
                        }
                    }

                    lsbase.SubmitChanges();
                    results.Saved = true;
                    return new JavaScriptSerializer().Serialize(results);
                }

                if (inputPacket.selectedAgencies == null || inputPacket.selectedAgencies.Length == 0)
                {
                    foreach (var record in allRecordsForUserSuburbs)
                    {
                        // Firstly invalidate all records with a matching suburb id. We do this as opposed to deleting the records, for record keeping.
                        record.agency_id = null;
                        record.updated_by = inputPacket.userGuid;
                    }
                    lsbase.SubmitChanges();
                    results.Saved = true;
                    return new JavaScriptSerializer().Serialize(results);
                }
                // These records represent data already saved that must be invalidated because they no longer exist in the incoming data
                var recordsThatMustBeInvalidated = from r in allRecordsForUserSuburbs
                                                   where r.agency_id != null && !inputPacket.selectedAgencies.Contains((int)r.agency_id)
                                                   select r;
                foreach (var record in recordsThatMustBeInvalidated)
                {
                    record.agency_id = null;
                    record.updated_by = inputPacket.userGuid;
                }
                lsbase.SubmitChanges();

                // Insert new records
                var newAgencyIdsForSuburb = inputPacket.selectedAgencies.Except((from a in allRecordsForUserSuburbs
                                                                                 where a.agency_id != null
                                                                                 select a.agency_id).Cast<int>());
                foreach (var newId in newAgencyIdsForSuburb)
                {
                    foreach (var suburb in userSuburbs)
                    {
                        var newRecord = new agencies_user_suburb { suburb_id = suburb.SuburbId, agency_id = newId, updated_by = inputPacket.userGuid };
                        lsbase.agencies_user_suburbs.InsertOnSubmit(newRecord);
                    }
                }
                lsbase.SubmitChanges();

                results.Saved = true;
                return new JavaScriptSerializer().Serialize(results);
            }
        }
    }

    /// <summary>
    /// Stores the dersialized version of the data POSTed to this page
    /// </summary>
    private struct DataRequestPacket
    {
        public string instruction;
        public int suburbID;
        public string userGuid;
        public int[] selectedAgencies;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}