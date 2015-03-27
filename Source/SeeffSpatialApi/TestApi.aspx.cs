using Newtonsoft.Json;
using SeeffSpatialApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SeeffSpatialApi
{
    public partial class TestApi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // GET
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:59658//"); // This is the web application's root server address
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync("api/SeeffSpatialLookup/GetAreaId?key=a2c48f98-14fb-425e-bbd2-312cfb89980c&lat=-33.0&lng=18.0").Result;
                int? areaId = response.Content.ReadAsAsync<int?>().Result;                

                // POST
                client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:59658//"); // This is the web application's root server address
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                SpatialLatLng modelInput = new SpatialLatLng { Lat = -33.0, Lng = 18.0 };
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                HttpContent content = new ObjectContent<SpatialLatLng>(modelInput, jsonFormatter);
                var resp = client.PostAsync("api/SeeffSpatialLookup/GetLicenseId", content).Result;
                int? licenseId = resp.Content.ReadAsAsync<int?>().Result;
            }
        }
    }
}