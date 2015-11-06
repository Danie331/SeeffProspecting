using Newtonsoft.Json;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;

namespace Seeff.Spatial.WebApp.BusinessLayer
{
    public class ServiceManager
    {
        private readonly string spatialServiceBaseAddress;
        public ServiceManager()
        {
            spatialServiceBaseAddress = WebConfigurationManager.AppSettings["seeff_spatial_service"];
        }        

        private T DeserializeResultFromService<T>(string controllerTarget)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(spatialServiceBaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(controllerTarget).Result;
                response.EnsureSuccessStatusCode();
                T result = response.Content.ReadAsAsync<T>().Result;

                return result;
            }
        }

        public List<SeeffSuburb> RetrieveAllSuburbs()
        {
            try
            {
                List<SeeffSuburb> suburbs = DeserializeResultFromService<List<SeeffSuburb>>("api/Read/GetAllSuburbs");
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
                List<SeeffLicense> seeffLicenses = DeserializeResultFromService<List<SeeffLicense>>("api/Read/GetAllLicenses");
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
                List<SeeffTerritory> territories = DeserializeResultFromService<List<SeeffTerritory>>("api/Read/GetAllTerritories");
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