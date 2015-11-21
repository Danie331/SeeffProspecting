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
    }
}