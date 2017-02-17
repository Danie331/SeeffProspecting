using ProspectingProject.SearchWorksAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace ProspectingProject
{
    public class TrustEnquiryServiceGetTrustees : ICompanyEnquiryService
    {
        private decimal _enquiryCostTrustInformationFull = 8.50M;

        private Guid _userGuid;

        //private Guid _tokenGuid;
        private SearchWorksAPIServiceClient _client;
        private string _userSessionToken;
        private CompanyEnquiryResponsePacket _results;
        private Exception _exception = null;
        private TrustSearchResult _targetTrust;
        public TrustEnquiryServiceGetTrustees(TrustSearchResult target)
        {
            _userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            _results = new CompanyEnquiryResponsePacket();
            _targetTrust = target;
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

        public decimal DeductEnquiryCost()
        {
            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                return prospectingAuthService.DebitUserBalance(_enquiryCostTrustInformationFull, _userGuid);
            }
        }

        public void DoEnquiry()
        {
            if (!string.IsNullOrEmpty(_results.ErrorMsg))
                return;

            string trustName = _targetTrust.TrustName;
            string trustNumber = _targetTrust.TrustNumber;
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
                xmlResponse = _client.CSITrustInformationSearch(_userSessionToken, Guid.NewGuid().ToString(), ResponseFormat.XML, "", trustName, trustNumber);
                result = XDocument.Parse((string)xmlResponse);
            }
            catch (Exception ex)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "Error occurred performing enquiry. You have not been billed. Details of the error: " + ex.Message;
                return;
            }

            List<ProspectingContactPerson> contacts = new List<ProspectingContactPerson>();

            var trustElement = result.Root.Element("Trust");
            if (trustElement != null)
            {
                var trusteesElement = trustElement.Element("Trustees");
                if (trusteesElement != null)
                {
                    contacts = CreateTrustees(trusteesElement);
                }
            }

            if (contacts.Count == 0)
            {
                string message = BuildReportsMessageForNoTrustees(result);
                ProspectingCore.SendEmail("danie.vdm@seeff.com", "Prospecting Support", "reports@seeff.com",null, "reports@seeff.com", "Prospecting system notification", message, false);
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "There was an issue retrieving trustees for this trust, Prospecting support has been notified about the issue.";
                return;
            }

            _results.EnquirySuccessful = true;
            _results.ErrorMsg = null;
            _results.Contacts = contacts;
        }

        private string BuildReportsMessageForNoTrustees(XDocument serviceOutput)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Trust enquiry service: No contacts could be retrieved for the following Trust enquiry, although it is flagged as having valid data. This means that either the person's firstname or surname or ID number is not present. Please see details below:");
            sb.AppendLine("");
            sb.AppendLine("User: " + _userGuid);
            sb.AppendLine("");
            sb.AppendLine("Prospecting property ID (database record ID): " + _targetTrust.ProspectingPropertyID);
            sb.AppendLine("");
            sb.AppendLine("Prospecting Trust ID (database record ID): " + _targetTrust.ContactCompanyID);
            sb.AppendLine("");
            sb.AppendLine("Trust Name: " + _targetTrust.TrustName);
            sb.AppendLine("");
            sb.AppendLine("Trust Number: " + _targetTrust.TrustNumber);
            sb.AppendLine("");
            sb.AppendLine("Output from service provider: ");
            sb.AppendLine("");
            sb.Append(serviceOutput.ToString(SaveOptions.DisableFormatting));

            return sb.ToString();
        }

        public void InitResponsePacket(CompanyEnquiryResponsePacket results)
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

                string companyRegNo = _targetTrust.TrustNumber;
                if (companyRegNo.Length > 13)
                {
                    companyRegNo = companyRegNo.Substring(0, 13);
                }

                string exceptionMessage = _exception != null ? _exception.ToString() : null;

                decimal? actualCost = null;
                if (successful)
                {
                    actualCost = _enquiryCostTrustInformationFull;
                }

                service_enquiry_log logEntry = new service_enquiry_log
                {
                    prospecting_property_id = _targetTrust.ProspectingPropertyID,
                    user = _userGuid,
                    date_of_enquiry = DateTime.Now,
                    successful = successful,
                    id_number = companyRegNo,
                    HWCE_indicator = "",
                    //
                    service_type_name = "TRUST ENQUIRY",
                    enquiry_cost = actualCost,
                    status_message = statusMessage,
                    exception = exceptionMessage,
                };

                prospectingDB.service_enquiry_logs.InsertOnSubmit(logEntry);
                prospectingDB.SubmitChanges();
            }
        }

        public decimal ReverseEnquiryCost()
        {
            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                return prospectingAuthService.CreditUserBalance(_enquiryCostTrustInformationFull, _userGuid);
            }
        }

        public void SetError(Exception ex)
        {
            _exception = ex;
        }

        private List<ProspectingContactPerson> CreateTrustees(XElement trusteesElement)
        {
            List<ProspectingContactPerson> prospectingContactPersons = new List<ProspectingContactPerson>();
            try
            {
                if (trusteesElement.HasElements)
                {
                    int? relationshipToTrust = ProspectingLookupData.PersonCompanyRelationshipTypes.First(kvp => kvp.Value == "Trustee").Key;
                    foreach (var trusteeDetail in trusteesElement.Elements("TrusteeDetail"))
                    {
                        string firstname = trusteeDetail.Element("FirstName")?.Value;
                        string surname = trusteeDetail.Element("Surname")?.Value; ;
                        string idNumber = trusteeDetail.Element("InferredIDNumber")?.Value;

                        if (!string.IsNullOrEmpty(firstname) && firstname != "-" &&
                            !string.IsNullOrEmpty(surname) && surname != "-" &&
                            !string.IsNullOrEmpty(idNumber) && idNumber != "-")
                        {
                            string gender = ProspectingCore.DetermineOwnerGender(idNumber);
                            if (!string.IsNullOrEmpty(gender))
                            {
                                ContactDataPacket contactDataPacket = new ContactDataPacket { ContactCompanyId = _targetTrust.ContactCompanyID, ProspectingPropertyId = _targetTrust.ProspectingPropertyID };
                                ProspectingContactPerson newContact = new ProspectingContactPerson
                                {
                                    IdNumber = idNumber,
                                    Title = null,
                                    Gender = gender,
                                    Firstname = firstname,
                                    Surname = surname,
                                    ContactCompanyId = _targetTrust.ContactCompanyID,
                                    PersonCompanyRelationshipType = relationshipToTrust
                                };
                                contactDataPacket.ContactPerson = newContact;
                                var prospectingContactPerson = ProspectingCore.SaveContactPerson(contactDataPacket);
                                prospectingContactPersons.Add(prospectingContactPerson);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (var prospectingDb = new ProspectingDataContext())
                {
                    var errorRec = new exception_log
                    {
                        friendly_error_msg = "Please contact support - error occurred creating contacts from successful Trust enquiry: CompanyId:" + _targetTrust.ContactCompanyID,
                        exception_string = ex.Message + " - " + ex.ToString(),
                        user = RequestHandler.GetUserSessionObject().UserGuid,
                        date_time = DateTime.Now
                    };
                    prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                    prospectingDb.SubmitChanges();
                }
            }
            return prospectingContactPersons;
        }
    }
}