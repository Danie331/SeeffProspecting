using Newtonsoft.Json;
using ProspectingProject.Controllers.Models;
using ProspectingProject.Services.Propdata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProspectingProject.Services.Propdata
{
    public class PropdataApiWrapper
    {
        private static SemaphoreSlim _sessionSemaphore = null, _clientSemaphore = null;
        private static Dictionary<string, ApiSessionContainer> _clientSessions;

        private const string _agentsBaseAddress = "https://eos-agents-staging.herokuapp.com/";
        private const string _listingsBaseAddress = "https://eos-listings-staging.herokuapp.com/";
        private const string _locationsBaseAddress = "https://eos-locations-staging.herokuapp.com/";
        private const string _branchesService = "https://eos-branches-staging.herokuapp.com/";

        private const string _residentialPortalURL = "https://staging.manage.propdata.net/secure/residential/{id}/edit";
        private const string _commercialPortalURL = "https://staging.manage.propdata.net/secure/commercial/{id}/edit";
        private const string _developmentsPortalURL = "https://staging.manage.propdata.net/secure/development/{id}/edit";
        private const string _holidayPortalURL = "https://staging.manage.propdata.net/secure/holiday/{id}/edit";

        static PropdataApiWrapper()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            _sessionSemaphore = new SemaphoreSlim(1, 1);
            _clientSemaphore = new SemaphoreSlim(1, 1);
            _clientSessions = new Dictionary<string, ApiSessionContainer>();
        }

        private async Task<HttpClient> CreateSession(string userId, string baseAddress)
        {
            await _sessionSemaphore.WaitAsync();
            try
            {
                HttpClient httpClient = null;
                if (!_clientSessions.ContainsKey(userId))
                {
                    var newSession = new ApiSessionContainer();
                    newSession.Token = await GetLoginTokenAsync("adam.roberts@learnit.co.za", "B0bbaF3tt", newSession); // retrieve these values from session obj
                    httpClient = newSession.GetServiceClient(baseAddress);
                    _clientSessions[userId] = newSession;
                }
                else
                {
                    var session = _clientSessions[userId];
                    httpClient = session.GetServiceClient(baseAddress);
                    session.Token = await GetRefreshTokenAsync(session);
                }

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientSessions[userId].Token);
                return httpClient;
            }
            finally
            {
                _sessionSemaphore.Release();
            }
        }

        private ApiSessionContainer GetSession(string sessionId)
        {
            if (_clientSessions.ContainsKey(sessionId))
            {
                return _clientSessions[sessionId];
            }

            throw new Exception($"Unable to find an existing session for {sessionId}");
        }

        private async Task<string> GetRefreshTokenAsync(ApiSessionContainer session)
        {
            try
            {
                var agentsService = session.GetServiceClient(_agentsBaseAddress);
                agentsService.DefaultRequestHeaders.Clear();
                agentsService.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
                using (var response = await agentsService.GetAsync("/api/v1/renew-token/"))
                {
                    response.EnsureSuccessStatusCode();
                    return response.Headers.GetValues("Token").FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message}", ex);
            }
        }

        private async Task<string> GetLoginTokenAsync(string username, string password, ApiSessionContainer session)
        {
            try
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(username + ":" + password);
                var encodedCredential = Convert.ToBase64String(plainTextBytes);
                var agentsService = session.GetServiceClient(_agentsBaseAddress);
                agentsService.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredential);
                using (var response = await agentsService.GetAsync("/public-api/login/"))
                {
                    response.EnsureSuccessStatusCode();
                    var payload = await response.Content.ReadAsStringAsync();
                    var loginResult = JsonConvert.DeserializeObject<LoginResult>(payload);
                    var token = loginResult.clients.FirstOrDefault(s => s.site.domain == "seeff.com")?.token;
                    return token;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message}", ex);
            }
        }

        private string GetSessionIdentifier()
        {
            return RequestHandler.GetUserSessionObject().EmailAddress;
        }

        public async Task<List<Agent>> GetAgentsAsync(List<int> branchIds)
        {
            string requestString = "";
            try
            {
                var sessionId = GetSessionIdentifier();
                var agentsService = await CreateSession(sessionId, _agentsBaseAddress);
                requestString = BuildHttpRequestInfo(agentsService, $"/api/v1/agents/?branches__contains={string.Join(",", branchIds)}&get_all=1", "GET", null);
                var response = await agentsService.GetStringAsync($"/api/v1/agents/?branches__contains={string.Join(",", branchIds)}&get_all=1");               
                var agents = JsonConvert.DeserializeObject<List<Agent>>(response);
                GetSession(sessionId).Agents = agents;

                return agents;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message} - Request: {requestString}", ex);
            }
        }

        public string GetAgentEmail(int propdataAgentId)
        {
            var userSession = GetSession(GetSessionIdentifier());
            var agents = userSession.Agents;
            var targetAgent = agents.FirstOrDefault(a => a.id == propdataAgentId);
            return targetAgent?.email;
        }

        public Agent GetAgent(int agentId)
        {
            var userSession = GetSession(GetSessionIdentifier());
            var targetAgent = userSession.Agents?.FirstOrDefault(a => a.id == agentId);

            return targetAgent;
        }

        public async Task<ListingResult> CreateResidentialListing(NewResidentialListingModel listingModel)
        {
            string requestString = "";
            try
            {
                var sessionId = GetSessionIdentifier();
                var listingService = await CreateSession(sessionId, _listingsBaseAddress);
                var response = await listingService.PostAsJsonAsync("/api/v1/residential/", listingModel);
                if (!response.IsSuccessStatusCode)
                {
                    requestString = BuildHttpRequestInfo(listingService, "/api/v1/residential/", "Post", JsonConvert.SerializeObject(listingModel));
                }
                var result = await response.Content.ReadAsStringAsync();
                var listingObject = JsonConvert.DeserializeObject<ListingResult>(result);
                listingObject.URL = _residentialPortalURL.Replace("{id}", listingObject.id.ToString());
                listingObject.JsonPayload = result;

                return listingObject;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message} - Request: {requestString}", ex);
            }
        }

        public async Task<ListingResult> CreateCommercialListing(NewCommercialListingModel listingModel)
        {
            string requestString = "";
            try
            {
                var sessionId = GetSessionIdentifier();
                var listingService = await CreateSession(sessionId, _listingsBaseAddress);
                var response = await listingService.PostAsJsonAsync("/api/v1/commercial/", listingModel);
                if (!response.IsSuccessStatusCode)
                {
                    requestString = BuildHttpRequestInfo(listingService, "/api/v1/commercial/", "Post", JsonConvert.SerializeObject(listingModel));
                }
                var result = await response.Content.ReadAsStringAsync();
                var listingObject = JsonConvert.DeserializeObject<ListingResult>(result);
                listingObject.URL = _commercialPortalURL.Replace("{id}", listingObject.id.ToString());
                listingObject.JsonPayload = result;

                return listingObject;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message} - Request: {requestString}", ex);
            }
        }

        public async Task<ListingResult> CreateDevelopmentsListing(NewDevelopmentsListingModel listingModel)
        {
            string requestString = "";
            try
            {
                var sessionId = GetSessionIdentifier();
                var listingService = await CreateSession(sessionId, _listingsBaseAddress);
                var response = await listingService.PostAsJsonAsync("/api/v1/developments/", listingModel);
                if (!response.IsSuccessStatusCode)
                {
                    requestString = BuildHttpRequestInfo(listingService, "/api/v1/developments/", "Post", JsonConvert.SerializeObject(listingModel));
                }
                var result = await response.Content.ReadAsStringAsync();
                var listingObject = JsonConvert.DeserializeObject<ListingResult>(result);
                listingObject.URL = _developmentsPortalURL.Replace("{id}", listingObject.id.ToString());
                listingObject.JsonPayload = result;

                return listingObject;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message} - Request: {requestString}", ex);
            }
        }

        public async Task<ListingResult> CreateHolidayListing(NewHolidayListingModel listingModel)
        {
            string requestString = "";
            try
            {
                var sessionId = GetSessionIdentifier();
                var listingService = await CreateSession(sessionId, _listingsBaseAddress);
                var response = await listingService.PostAsJsonAsync("/api/v1/holiday/", listingModel);
                if (!response.IsSuccessStatusCode)
                {
                    requestString = BuildHttpRequestInfo(listingService, "/api/v1/holiday/", "Post", JsonConvert.SerializeObject(listingModel));
                }
                var result = await response.Content.ReadAsStringAsync();
                var listingObject = JsonConvert.DeserializeObject<ListingResult>(result);
                listingObject.URL = _holidayPortalURL.Replace("{id}", listingObject.id.ToString());
                listingObject.JsonPayload = result;

                return listingObject;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message} - Request: {requestString}", ex);
            }
        }

        public async Task<List<LocationResult>> GetLocationsFromP24IDs(List<int> p24SuburbIDs)
        {
            string requestString = "";
            try
            {
                var sessionId = GetSessionIdentifier();
                var locationsService = await CreateSession(sessionId, _locationsBaseAddress);
                var results = new List<LocationResult>();
                foreach (var item in p24SuburbIDs)
                {
                    try
                    {
                        var response = await locationsService.GetStringAsync($"/api/v1/locations/?property24_id__exact={item}");
                        var resultHolder = JsonConvert.DeserializeObject<LocationResultContainer>(response);
                        results.Add(resultHolder.results.First());
                    }
                    catch (Exception ex)
                    {
                        requestString = BuildHttpRequestInfo(locationsService, $"/api/v1/locations/?property24_id__exact={item}", "GET", null);
                        throw;
                    }
                }
               
                return results;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error from service provider: {ex.Message} - Request: {requestString}", ex);
            }
        }

        private string BuildHttpRequestInfo(HttpClient client, string uri, string method, string body)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("URL: " + client.BaseAddress + uri);
            sb.AppendLine("METHOD: " + method);
            if (body != null)
            {
                sb.AppendLine("BODY: " + body);
            }

            sb.AppendLine("TOKEN: " + client.DefaultRequestHeaders.First(s => s.Key == "Authorization").Value.First());

            return sb.ToString();
        }
    }
}
