using Seeff.Spatial.WebApp.BusinessLayer;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.ServiceInterface
{
    public class SpatialDataReader : ServiceBase
    {
        public List<SeeffSuburb> RetrieveAllSuburbs()
        {
            try
            {
                List<SeeffSuburb> suburbs = RetrieveFromService<List<SeeffSuburb>>("api/Read/GetAllSuburbs");
                return suburbs;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "RetrieveAllSuburbs()", null);
                throw;
            }
        }

        public List<SeeffLicense> RetrieveAllLicenses()
        {
            try
            {
                List<SeeffLicense> seeffLicenses = RetrieveFromService<List<SeeffLicense>>("api/Read/GetAllLicenses");
                return seeffLicenses;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "RetrieveAllLicenses()", null);
                throw;
            }
        }

        public List<SeeffTerritory> RetrieveAllTerritories()
        {
            try
            {
                List<SeeffTerritory> territories = RetrieveFromService<List<SeeffTerritory>>("api/Read/GetAllTerritories");
                return territories;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "RetrieveAllTerritories()", null);
                throw;
            }
        }

        public SeeffSuburb GetSuburbUnderMaintenance()
        {
            try
            {
                SeeffSuburb result = RetrieveFromService<SeeffSuburb>("api/Read/GetSuburbUnderMaintenance");
                return result;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetSuburbUnderMaintenance()", null);
                throw;
            }
        }

        public List<SeeffSuburb> GetSuburbsUnderMaintenance()
        {
            try
            {
                List<SeeffSuburb> results = RetrieveFromService<List<SeeffSuburb>>("api/Read/GetSuburbsUnderMaintenance");
                return results;
            }
            catch(Exception ex)
            {
                Utils.LogException(ex, "GetSuburbsUnderMaintenance()", null);
                throw;
            }
        }

        public List<SeeffSuburb> GetSpatialSuburbsListOnly()
        {
            try
            {
                List<SeeffSuburb> results = RetrieveFromService<List<SeeffSuburb>>("api/Read/GetSuburbsListOnly");
                return results;
            }
            catch(Exception ex)
            {
                Utils.LogException(ex, "GetSpatialSuburbsListOnly() in Spatial web app", null);
                throw;
            }
        }
    }
}