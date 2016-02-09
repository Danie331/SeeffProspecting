using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public SpatialSuburb LoadSuburb(int suburbID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PostAsJsonAsync<int>("api/Read/GetSuburbFromID", suburbID).Result;
                response.EnsureSuccessStatusCode();
                SpatialSuburb result = response.Content.ReadAsAsync<SpatialSuburb>().Result;

                return result;
            }
        }

        public List<SpatialSuburb> SuburbsListOnly()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/Read/GetSuburbsListOnly").Result;
                response.EnsureSuccessStatusCode();
                List<SpatialSuburb> result = response.Content.ReadAsAsync<List<SpatialSuburb>>().Result;

                return result;
            }
        }

        public SpatialSuburb GetSuburbFromID(decimal lat, decimal lng)
        {
            var spatialPoint = new SpatialPoint { Lat = Convert.ToDouble(lat), Lng = Convert.ToDouble(lng) };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_serviceBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PostAsJsonAsync<SpatialPoint>("api/Read/GetSuburbFromPoint", spatialPoint).Result;
                response.EnsureSuccessStatusCode();
                SpatialSuburb result = response.Content.ReadAsAsync<SpatialSuburb>().Result;

                return result;
            }
        }
    }
}