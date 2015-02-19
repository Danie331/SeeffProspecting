<%@ WebHandler Language="C#" Class="AgenciesDataManager" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Linq;
using System.Collections.Generic;

public class AgenciesDataManager : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        var json = context.Request.Form[0];
        DataRequestPacket requestPacket = (DataRequestPacket)new JavaScriptSerializer().Deserialize(json, typeof(DataRequestPacket));

        switch (requestPacket.instruction)
        {
            case "load":
                var selectedAgencies = LoadAgenciesAvailableToSuburb(requestPacket);    
                var serialiser = new JavaScriptSerializer();
                context.Response.Write(serialiser.Serialize(selectedAgencies));            
                break;
            case "save":
                SaveSelectedAgenciesForSuburb(requestPacket);                
                break;
            default: break;
        }        
    }

    private int[] LoadAgenciesAvailableToSuburb(DataRequestPacket inputPacket)
    {
        // TODO: currently looking at a table in the boss database. The final table schema and location to be determined
        using (var lsbase = DataManager.DataContextRetriever.GetLSBaseDataContext())
        {
            return (from agncySub in lsbase.agencies_user_suburbs
                   where agncySub.suburb_id == inputPacket.suburbID && agncySub.agency_id != null
                   select agncySub.agency_id).Cast<int>().ToArray();
        }
    }

    private void SaveSelectedAgenciesForSuburb(DataRequestPacket inputPacket)
    {
        using (var lsbase = DataManager.DataContextRetriever.GetLSBaseDataContext())
        {
            var allRecordsForExistingSuburb = from a in lsbase.agencies_user_suburbs
                                              where a.suburb_id == inputPacket.suburbID
                                              select a;

            if (allRecordsForExistingSuburb.Count() == 0)
            {
                // Insert records for this suburb because it doesn't yet exist
                foreach (int id in inputPacket.selectedAgencies)
                {
                    var newRecord = new agencies_user_suburb { suburb_id = inputPacket.suburbID, agency_id = id, updated_by = inputPacket.userGuid };
                    lsbase.agencies_user_suburbs.InsertOnSubmit(newRecord);
                }

                lsbase.SubmitChanges();
                return;
            }

            if (inputPacket.selectedAgencies == null || inputPacket.selectedAgencies.Length == 0)
            {
                foreach (var record in allRecordsForExistingSuburb)
                {
                    // Firstly invalidate all records with a matching suburb id. We do this as opposed to deleting the records, for record keeping.
                    record.agency_id = null;
                    record.updated_by = inputPacket.userGuid;
                }
                lsbase.SubmitChanges();
                return;
            }
            // These records represent data already saved that must be invalidated because they no longer exist in the incoming data
            var recordsThatMustBeInvalidated = from r in allRecordsForExistingSuburb
                                               where r.agency_id != null && !inputPacket.selectedAgencies.Contains((int)r.agency_id)
                                               select r;
            foreach (var record in recordsThatMustBeInvalidated)
            {
                record.agency_id = null;
                record.updated_by = inputPacket.userGuid;
            }
            lsbase.SubmitChanges();

            // Insert new records for an existing suburb, this will be an insert or update depending on whether agency_id is null
            var newAgencyIdsForSuburb = inputPacket.selectedAgencies.Except((from a in allRecordsForExistingSuburb
                                                                            where a.agency_id != null
                                                                            select a.agency_id).Cast<int>());
            foreach (var newId in newAgencyIdsForSuburb)
            {
                var newRecord = new agencies_user_suburb { suburb_id = inputPacket.suburbID, agency_id = newId, updated_by = inputPacket.userGuid };
                lsbase.agencies_user_suburbs.InsertOnSubmit(newRecord);
            }
            lsbase.SubmitChanges();
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