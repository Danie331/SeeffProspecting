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
    public class TrustEntityEnquiryService : ICompanyEnquiryService
    {
        private decimal _enquiryCostTrustInformationFull = 8.50M;
        private decimal _enquiryCostTrustNotFoundOrNoTrustees = 4.0M;

        private bool _mustReimburseIncompleteResult = false;
      
        private Guid _userGuid;

        //private Guid _tokenGuid;
        private SearchWorksAPIServiceClient _client;
        private string _userSessionToken;
        private prospecting_contact_company _company;
        private int _prospectingPropertyId;
        private CompanyEnquiryResponsePacket _results;
        private Exception _exception = null;
        public TrustEntityEnquiryService(prospecting_contact_company company, int prospectingPropertyId)
        {
            _userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            _results = new CompanyEnquiryResponsePacket();
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
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "The service provider could not find a match for this trust, or their records are incomplete. The corresponding fee has been deducted from your balance.";
                _mustReimburseIncompleteResult = true;
                return;
            }

            _results.EnquirySuccessful = true;
            _results.ErrorMsg = null;
            _results.Contacts = contacts;
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

                string companyRegNo = _company.CK_number;
                if (_company.CK_number.Length > 13)
                {
                    companyRegNo = companyRegNo.Substring(0, 13);
                }

                string exceptionMessage = _exception != null ? _exception.ToString() : null;

                decimal? actualCost = null;
                if (successful)
                {
                    actualCost = _enquiryCostTrustInformationFull;
                }
                else
                {
                    if (_mustReimburseIncompleteResult)
                    {
                        actualCost = _enquiryCostTrustNotFoundOrNoTrustees;
                    }
                }

                service_enquiry_log logEntry = new service_enquiry_log
                {
                    prospecting_property_id = _prospectingPropertyId,
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
            decimal amountToCredit = _mustReimburseIncompleteResult ? (_enquiryCostTrustInformationFull - _enquiryCostTrustNotFoundOrNoTrustees)
                : _enquiryCostTrustInformationFull;              

            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                return prospectingAuthService.CreditUserBalance(amountToCredit, _userGuid);
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
                                ContactDataPacket contactDataPacket = new ContactDataPacket { ContactCompanyId = _company.contact_company_id, ProspectingPropertyId = _prospectingPropertyId };
                                ProspectingContactPerson newContact = new ProspectingContactPerson
                                {
                                    IdNumber = idNumber,
                                    Title = null,
                                    Gender = gender,
                                    Firstname = firstname,
                                    Surname = surname,
                                    ContactCompanyId = _company.contact_company_id,
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
                        friendly_error_msg = "Please contact support - error occurred creating contacts from successful Trust enquiry: CompanyId:" + _company.CK_number,
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