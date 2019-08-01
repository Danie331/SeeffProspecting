using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace ProspectingProject.Services.SeeffSpatial
{
    public class SpatialServiceReader
    {
        private string _serviceBase;
        public SpatialServiceReader()
        {
            _serviceBase = WebConfigurationManager.AppSettings["seeff_spatial_service"];
        }

        public async Task<SpatialSuburb> LoadSuburb(int suburbID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync<int>("api/Read/GetSuburbFromID", suburbID);
                response.EnsureSuccessStatusCode();
                SpatialSuburb result = await response.Content.ReadAsAsync<SpatialSuburb>();

                return result;
            }
        }

        public async Task<List<SpatialSuburb>> SuburbsListOnly()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/Read/GetSuburbsListOnly");
                response.EnsureSuccessStatusCode();
                List<SpatialSuburb> result = await response.Content.ReadAsAsync<List<SpatialSuburb>>();

                return result;
            }
        }

        public async Task<SpatialSuburb> GetSuburbFromID(decimal lat, decimal lng)
        {
            var spatialPoint = new SpatialPoint { Lat = Convert.ToDouble(lat), Lng = Convert.ToDouble(lng) };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync<SpatialPoint>("api/Read/GetSuburbFromPoint", spatialPoint);
                response.EnsureSuccessStatusCode();
                SpatialSuburb result = await response.Content.ReadAsAsync<SpatialSuburb>();

                return result;
            }
        }
    }
}