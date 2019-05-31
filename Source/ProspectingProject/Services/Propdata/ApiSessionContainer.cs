using ProspectingProject.Services.Propdata.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProspectingProject.Services.Propdata
{
    public class ApiSessionContainer
    {
        private Dictionary<string, HttpClient> _clientConnections;

        public ApiSessionContainer()
        {
            _clientConnections = new Dictionary<string, HttpClient>();
        }

        public HttpClient GetServiceClient(string baseAddress)
        {
            if (!_clientConnections.ContainsKey(baseAddress))
            {
                _clientConnections[baseAddress] = new HttpClient();
                _clientConnections[baseAddress].BaseAddress = new Uri(baseAddress);
                _clientConnections[baseAddress].DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return _clientConnections[baseAddress];
            }
            return _clientConnections[baseAddress];
        }

        public string Token { get; set; }

        public List<Agent> Agents { get; set; }
    }
}