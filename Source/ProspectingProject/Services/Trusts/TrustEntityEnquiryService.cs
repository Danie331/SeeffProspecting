using ProspectingProject.SearchWorksAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProspectingProject
{
    public class TrustEntityEnquiryService 
    {      
        private Guid _userGuid;

        //private Guid _tokenGuid;
        private SearchWorksAPIServiceClient _client;
        private string _userSessionToken;
        private prospecting_contact_company _company;
        private int _prospectingPropertyId;
        private TrustEnquiryResultPacket _results;
        private Exception _exception = null;
        public TrustEntityEnquiryService(prospecting_contact_company company, int prospectingPropertyId)
        {
            _userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            _results = new TrustEnquiryResultPacket();
            _company = company;
            _prospectingPropertyId = prospectingPropertyId;
            try
            {
                _client = new SearchWorksAPIServiceClient();
                _userSessionToken = _client.Login("apilive@seeff.com", "S33fL1v3@p1");
            }
            catch (Exception e)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "Service provider is currently unavailable - please try again later";
                using (var prospectingDb = new ProspectingDataContext())
                {
                    var errorRec = new exception_log
                    {
                        friendly_error_msg = "Error occurred while trying to access the Trust enquiries service: " + e.Message,
                        exception_string = e.ToString(),
                        user = RequestHandler.GetUserSessionObject().UserGuid,
                        date_time = DateTime.Now
                    };
                    prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                    prospectingDb.SubmitChanges();
                }
                throw new Exception(_results.ErrorMsg, e);
            }
        }

        public void DoEnquiry()
        {
            if (!string.IsNullOrEmpty(_results.ErrorMsg))
                return;

            string trustName = _company.company_name;
            string trustNumber = _company.CK_number;
            if (string.IsNullOrEmpty(trustNumber) || trustNumber.Contains("UNKNOWN_CK"))
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "This trust does not have a valid trust number. You have not been billed.";
                return;
            }

            trustName = trustName.Trim();
            trustNumber = trustNumber.Trim();

            object xmlResponse = null;
            XDocument result = null;
            try
            {
                xmlResponse = _client.CSITrustInformationSearch(_userSessionToken, Guid.NewGuid().ToString(), ResponseFormat.XML, "", "", trustNumber);
                result = XDocument.Parse((string)xmlResponse);
            }
            catch (Exception ex)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "Error occurred performing enquiry. You have not been billed. Details of the error: " + ex.Message;
                return;
            }

            List<TrustSearchResult> trustSearchResults = new List<TrustSearchResult>();

            var trustsElement = result.Root.Elements("Trust");
            if (trustsElement != null)
            {
                foreach (var trust in trustsElement)
                {
                    List<TrustSearchResult> trustResults = GetTrustResults(trust);
                    trustSearchResults.AddRange(trustResults);
                }
                trustSearchResults = trustSearchResults.Where(tr => tr.HasPersonData == "AVAILABLE").ToList();
            }

            if (trustSearchResults.Count == 0)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "The service provider could not find any matches for this trust, or their records are incomplete.";
                return;
            }

            _results.EnquirySuccessful = true;
            _results.ErrorMsg = null;
            _results.Results = trustSearchResults;
        }

        private List<TrustSearchResult> GetTrustResults(XElement trust)
        {
            List<TrustSearchResult> results = new List<TrustSearchResult>();
            var trusteesElement = trust.Element("Trustees");
            if (trusteesElement != null && trusteesElement.HasElements)
            {
                foreach (var trusteeDetail in trusteesElement.Elements("TrusteeDetail"))
                {
                    TrustSearchResult result = new TrustSearchResult(trusteeDetail, _company.contact_company_id, _prospectingPropertyId);
                    results.Add(result);
                }
            }

            return results;
        }

        public void InitResponsePacket(TrustEnquiryResultPacket results)
        {
            _results = results;
        }

        public void LogEnquiry()
        {
            if (_results == null)
                return;   // Check to ensure that an enquiry has actually been done with this instance

            using (var prospectingDB = new ProspectingDataContext())
            {
                bool successful = _results.EnquirySuccessful;
                string statusMessage = successful ? "OK" : _results.ErrorMsg;
                if (statusMessage.Length > 255)
                {
                    statusMessage = statusMessage.Substring(0, 255);
                }

                string companyRegNo = _company.CK_number;
                if (_company.CK_number.Length > 13)
                {
                    companyRegNo = companyRegNo.Substring(0, 13);
                }

                string exceptionMessage = _exception != null ? _exception.ToString() : null;

                service_enquiry_log logEntry = new service_enquiry_log
                {
                    prospecting_property_id = _prospectingPropertyId,
                    user = _userGuid,
                    date_of_enquiry = DateTime.Now,
                    successful = successful,
                    id_number = companyRegNo,
                    HWCE_indicator = "",
                    //
                    service_type_name = "TRUST SEARCH",
                    enquiry_cost = null,
                    status_message = statusMessage,
                    exception = exceptionMessage,
                };

                prospectingDB.service_enquiry_logs.InsertOnSubmit(logEntry);
                prospectingDB.SubmitChanges();
            }
        }

        public void SetError(Exception ex)
        {
            _exception = ex;
        }
    }
}