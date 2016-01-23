using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    public class BOSSWebService
    {
        private string _wsBaseAddress = "http://bossservices.seeff.com/";

        public List<int> GetListUnregisteredProperties()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_wsBaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/BOSS/GetUnRegisteredProperties").Result;
                response.EnsureSuccessStatusCode();
                List<int> unregisteredProeprties = response.Content.ReadAsAsync<List<int>>().Result;

                return unregisteredProeprties;
            }
        }

        public TransactionResult IsSeeffRegistered(int propertyID, string regDate)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://bossservices.seeff.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tran = new TransactionRequest
                {
                    PropertyId = propertyID,
                    RegistrationDate = regDate
                };
                HttpResponseMessage response = client.PostAsJsonAsync<TransactionRequest>("api/BOSS/BOSSRegistrationLookup", tran).Result;
                response.EnsureSuccessStatusCode();
                TransactionResult result = response.Content.ReadAsAsync<TransactionResult>().Result;

                return result;
            }
        }
    }
}