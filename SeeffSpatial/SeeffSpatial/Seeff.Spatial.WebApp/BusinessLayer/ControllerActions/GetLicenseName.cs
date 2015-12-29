using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.ControllerActions
{
    public partial class ControllerActions
    {
        public static LicenseNameResult GetLicenseName(SeeffLicense license)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://bossservices.seeff.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // GetLicenseFromID
                HttpResponseMessage response = client.PostAsJsonAsync<int>("api/BOSS/GetLicenseName", license.LicenseID).Result;
                response.EnsureSuccessStatusCode();
                LicenseNameResult result = response.Content.ReadAsAsync<LicenseNameResult>().Result;

                return result;
            }
        }
    }
}