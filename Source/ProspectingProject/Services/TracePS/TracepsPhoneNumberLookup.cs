using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ProspectingProject
{
    public class TracepsPhoneNumberLookup : IPersonEnquiryService
    {
        private decimal _enquiryCost = 0.6m;

        private Guid _userGuid;
        private ITracepsService _tpsService;
        private ProspectingInputData _inputData;
        private Exception _exception = null;
        private PersonEnquiryResponsePacket _results = null;
        public TracepsPhoneNumberLookup(ITracepsService tpsService, ProspectingInputData inputData)
        {
            _tpsService = tpsService;
            _inputData = inputData;
            _userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
        }

        public void InitResponsePacket(PersonEnquiryResponsePacket results)
        {
            _results = results;
            _results.IdNumber = _inputData.LightstoneIDOrCKNo;
            _results.OwnerGender = ProspectingCore.DetermineOwnerGender(_inputData.LightstoneIDOrCKNo);
            _results.LookupType = ProspectingLookupData.TracePSEnquiryRequest;
        }

        public decimal DeductEnquiryCost()
        {
            if (RequestHandler.IsTrainingMode())
            {
                return 0.0M;
            }

            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                return prospectingAuthService.DebitUserBalance(_enquiryCost, _userGuid);
            }
        }

        public void DoEnquiry()
        {
            _results.EnquirySuccessful = true;
            _results.IdNumber = _inputData.LightstoneIDOrCKNo;
            // Check #1: Check if the ID number is valid, ie 13 digits and convertible to a long.
            if (!ProspectingCore.IsValidIDNumber(_results.IdNumber))
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "Invalid ID number specified.";
                return;
            }

            XDocument resultsDoc = null;
            try
            {
                if (RequestHandler.IsTrainingMode())
                {
                    resultsDoc = GenerateTestResponseData();
                }
                else
                {
                    resultsDoc = _tpsService.GetResponseXML(_inputData.LightstoneIDOrCKNo);
                }
            }
            catch (Exception ex)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = ex.Message;
                return;
            }

            // Process the response
            foreach (var element in resultsDoc.Descendants("category"))
            {
                if (element.Attribute("name").Value == "contact")
                {
                    _results.ContactRows = (from e in element.Descendants("row")
                                           select new ContactRow
                                           {
                                               Phone = e.Descendants("field").Where(f => f.Attribute("name").Value == "phone").FirstOrDefault().Value,
                                               Type = e.Descendants("field").Where(f => f.Attribute("name").Value == "type").FirstOrDefault().Value,
                                               Date = e.Descendants("field").Where(f => f.Attribute("name").Value == "date").FirstOrDefault().Value
                                           }).OrderByDescending(d =>
                                           {
                                               if (!string.IsNullOrEmpty(d.Date))
                                               {
                                                   return Convert.ToDateTime(d.Date);
                                               }
                                               return DateTime.Now;
                                           }).ToList();
                }

                if (element.Attribute("name").Value == "person")
                {
                    string surname = element.Descendants().First(s => s.Attribute("name").Value == "surname").Value;
                    string name1 = element.Descendants().First(s => s.Attribute("name").Value == "name1").Value;
                    string name2 = element.Descendants().First(s => s.Attribute("name").Value == "name2").Value;
                    _results.OwnerName = string.Concat(name1, " ", name2);
                    _results.OwnerSurname = surname;
                }
            }

            // Check for error
            string outputXmlString = resultsDoc.ToString();
            if (outputXmlString.Contains("ERR04") && outputXmlString.Contains("No result found"))
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "No TracePS results could be found for this ID number.";
                return;
            }
            if (string.IsNullOrWhiteSpace(_results.OwnerName) || _results.ContactRows.Count == 0)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "No TracePS contact information could be found for this ID number, although a record was found.";
                return;
            }

           // Set the HWCE indicator
            char[] HWCE_indicators = "0000".ToCharArray(); // NB order in which we add flags NB.
            if (_results.ContactRows.Any(c => c.Type == "home"))
            {
                HWCE_indicators[0] = '1';
            }
            if (_results.ContactRows.Any(c => c.Type == "work"))
            {
                HWCE_indicators[1] = '1';
            }
            if (_results.ContactRows.Any(c => c.Type == "cell"))
            {
                HWCE_indicators[2] = '1';
            }
            _results.HWCE_indicator = new string(HWCE_indicators);
        }

        public decimal ReverseEnquiryCost()
        {
            if (RequestHandler.IsTrainingMode())
            {
                return 0.0M;
            }

            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                return prospectingAuthService.CreditUserBalance(_enquiryCost, _userGuid);
            }
        }

        public void SetError(Exception ex)
        {
            _exception = ex;
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
                string idNumber = "";
                if (!string.IsNullOrWhiteSpace(_results.IdNumber))
                {
                    if (_results.IdNumber.Length > 13)
                    {
                        idNumber = _results.IdNumber.Substring(0, 13);
                    }
                    else
                    {
                        idNumber = _results.IdNumber;
                    }
                }

                string exceptionMessage = _exception != null ? _exception.ToString() : null;
                service_enquiry_log logEntry = new service_enquiry_log
                {
                    prospecting_property_id = _inputData.ProspectingPropertyId.HasValue ? _inputData.ProspectingPropertyId.Value : -1,
                    user = _userGuid,
                    date_of_enquiry = DateTime.Now,
                    successful = successful,
                    id_number = idNumber,
                    HWCE_indicator = _results.HWCE_indicator,
                    //
                    service_type_name = _results.LookupType,
                    enquiry_cost = successful ? _enquiryCost : (decimal?)null,
                    status_message = statusMessage,
                    exception = exceptionMessage,
                };

                prospectingDB.service_enquiry_logs.InsertOnSubmit(logEntry);
                prospectingDB.SubmitChanges();
            }
        }

        private XDocument GenerateTestResponseData()
        {
            return new TracepsTestService().GetResponseXML(null);
        }
    }
}