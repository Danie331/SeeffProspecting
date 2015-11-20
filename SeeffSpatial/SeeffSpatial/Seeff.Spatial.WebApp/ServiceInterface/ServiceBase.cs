using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;

namespace Seeff.Spatial.WebApp.ServiceInterface
{
    public abstract class ServiceBase
    {
        private readonly string spatialServiceBaseAddress;
        public ServiceBase()
        {
            spatialServiceBaseAddress = WebConfigurationManager.AppSettings["seeff_spatial_service"];
        }        

        protected T RetrieveFromService<T>(string controllerTarget)
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

        protected T PostToService<T>(string controllerTarget, T input)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(spatialServiceBaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PostAsJsonAsync<T>(controllerTarget, input).Result;
                response.EnsureSuccessStatusCode();
                T result = response.Content.ReadAsAsync<T>().Result;

                return result;
            }
        }
    }
}