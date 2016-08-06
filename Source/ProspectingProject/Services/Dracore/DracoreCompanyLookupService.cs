using ProspectingProject.DracoreETLConsumerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    // Service enquiry log entry -- look at traceps/dreacore
    // Also test with an existing contact already added/affiliated to company/property
    public class DracoreCompanyLookupService : ICompanyEnquiryService
    {
        private decimal _enquiryCost = 14.40M;
        private Guid _userGuid;

        //private Guid _tokenGuid;
        private DracoreDataServiceClient _client;
        private DracoreETLConsumerService.Token _token;
        private prospecting_contact_company _company;
        private int _prospectingPropertyId;
        private CompanyEnquiryResponsePacket _results;
        private Exception _exception = null;
        public DracoreCompanyLookupService(prospecting_contact_company company, int prospectingPropertyId)
        {
            _userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
            _results = new CompanyEnquiryResponsePacket();
            _company = company;
            _prospectingPropertyId = prospectingPropertyId;
            try
            {
                _client = new DracoreDataServiceClient();                
            }
            catch (Exception e)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "Service provider is currently unavailable - please try again later";
                using (var prospectingDb = new ProspectingDataContext())
                {
                    var errorRec = new exception_log
                    {
                        friendly_error_msg = "Error occurred while trying to Login() to Dracore companies service: " + e.Message,
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

        private DracoreETLConsumerService.Token GetToken()
        {
            var token = new DracoreETLConsumerService.Token();// DracoreETLConsumerService.Token();
            token.Username = "seefflive";
            string reqTime = DateTime.Now.ToString();//.ToString("yyyyMMddHHmmssffff");
            token.RequestTime = reqTime;
            string tokenHash = token.Username + reqTime + "S33f!@)(LIVE" + "154.70.214.213";
            var crypto = System.Security.Cryptography.SHA1CryptoServiceProvider.Create();
            var cryptoBytes = crypto.ComputeHash(System.Text.Encoding.UTF8.GetBytes(tokenHash));
            token.TokenVerify = Convert.ToBase64String(cryptoBytes);

            return token;
        }

        public decimal DeductEnquiryCost()
        {
            using (var prospectingAuthService = new ProspectingUserAuthService.SeeffProspectingAuthServiceClient())
            {
                return prospectingAuthService.DebitUserBalance(_enquiryCost, _userGuid);
            }
        }

        // EXCEPTION HANDLING + WALLET
        public void DoEnquiry()
        {
            if (!string.IsNullOrEmpty(_results.ErrorMsg))
                return;
            //throw new NotImplementedException();
            var token = GetToken();
            var companySearch = _client.SearchCompanies(token, "", "", _company.CK_number);//.BusinessEnquiry(_tokenGuid, _company.CK_number, "", "");
            if (companySearch != null)
            {
                if (companySearch.ResultType == SearchResultType.MatchFound)
                {
                    _results.EnquirySuccessful = true;
                    _results.ErrorMsg = null;
                    var searchedCompany = companySearch.Results.First();
                    var contacts = CreateCompanyContactPersons(searchedCompany);
                    _results.Contacts = contacts;
                    // Test this in an SS with multiple units owned by same company
                }
                else
                {
                    string failureReason = string.Empty;
                    switch (companySearch.ResultType)
                    {
                        case SearchResultType.Error:
                            failureReason = companySearch.ErrorMessage;
                            break;
                        case SearchResultType.MultipleMatchesFound:
                            failureReason = "Multiple matching companies found for this reg. no.";
                            break;
                        case SearchResultType.NoMatchesFound:
                            failureReason = "No matching result found.";
                            break;
                    }
                    _results.ErrorMsg = "Enquiry unsuccessful - Reason: " + failureReason;
                    if (companySearch.ResultType == SearchResultType.MultipleMatchesFound)
                    {
                        using (var prospectingDb = new ProspectingDataContext())
                        {
                            var errorRec = new exception_log
                            {
                                friendly_error_msg = "Dracore companies enquiry returned more than one result for reg. no. " + _company.CK_number,
                                exception_string = "",
                                user = RequestHandler.GetUserSessionObject().UserGuid,
                                date_time = DateTime.Now
                            };
                            prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                            prospectingDb.SubmitChanges();
                        }
                    }
                }
            }
            else
            {
                _results.ErrorMsg = "Service provider could not find a company with matching criteria.";
            }
        }
        /// <summary>
        /// //
        /// </summary>
        /// <param name="targetCompany"></param>
        /// <returns></returns>
        private List<ProspectingContactPerson> CreateCompanyContactPersons(DracoreETLConsumerService.Company targetCompany)
        {
            List<ProspectingContactPerson> prospectingContactPersons = new List<ProspectingContactPerson>();
            try
            {
                bool atLeastOneDirectorSaved = false;
                if (targetCompany.Directors != null && targetCompany.Directors.Count() > 0)
                {
                    int? relationshipToCompany = ProspectingLookupData.PersonCompanyRelationshipTypes.First(kvp => kvp.Value == "Company Director").Key;
                    foreach (var director in targetCompany.Directors)
                    {
                        var directorPerson = director;

                        string firstname = directorPerson.FirstNames;
                        if (string.IsNullOrWhiteSpace(firstname))
                        {
                            firstname = "(unknown firstname)";
                        }
                        string surname = directorPerson.Surname;
                        if (string.IsNullOrWhiteSpace(surname))
                        {
                            surname = "(unknown surname)";
                        }
                        if (!string.IsNullOrEmpty(directorPerson.IdentityNumber) && directorPerson.IdentityNumber.Length == 13)
                        {
                            string gender;
                            switch (directorPerson.Gender)
                            {
                                case GenderEnum.Male: gender = "M"; break;
                                case GenderEnum.Female: gender = "F"; break;
                                default: gender = ProspectingCore.DetermineOwnerGender(directorPerson.IdentityNumber); break;
                            }

                            var titleKvp = /*!string.IsNullOrEmpty(directorPerson.) ? ProspectingLookupData.ContactPersonTitle.FirstOrDefault(cpt => cpt.Value.ToLower() == directorPerson.Salutation.ToLower()) :*/ default(KeyValuePair<int, string>);
                            ContactDataPacket contactDataPacket = new ContactDataPacket { ContactCompanyId = _company.contact_company_id, ProspectingPropertyId = _prospectingPropertyId };
                            ProspectingContactPerson newContact = new ProspectingContactPerson
                            {
                                IdNumber = directorPerson.IdentityNumber,
                                Title = !Equals(titleKvp, default(KeyValuePair<int, string>)) ? (int?)titleKvp.Key : null,
                                Gender = gender,
                                Firstname = firstname,
                                Surname = surname,
                                ContactCompanyId = _company.contact_company_id,
                                Directorship = "Yes",
                                PersonCompanyRelationshipType = relationshipToCompany
                                // TODO: Rem to add phone no's + email addresses if they are available???
                            };
                            contactDataPacket.ContactPerson = newContact;
                            var prospectingContactPerson = ProspectingCore.SaveContactPerson(contactDataPacket);
                            prospectingContactPersons.Add(prospectingContactPerson);
                            atLeastOneDirectorSaved = true;
                        }
                    }
                }
                else
                {
                    // Ensure enquiry not billed, that it is not successful, and front-end reflects accordingly.
                    _results.EnquirySuccessful = false;
                    _results.ErrorMsg = "No Directorship information was found for this company - the transaction will not be billed.";
                }
                if (!atLeastOneDirectorSaved)
                {
                    _results.EnquirySuccessful = false;
                    _results.ErrorMsg = "A Dracore match was found but has missing or incomplete information - the transaction will not be billed.";
                }
            }
            catch(Exception ex)
            {
                using (var prospectingDb = new ProspectingDataContext())
                {
                    var errorRec = new exception_log
                    {
                        friendly_error_msg = "URGENT ATTENTION REQUIRED - Error occurred creating contacts from successful Dracore company enquiry result: CompanyId:" + _company.CK_number,
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

        // warn about enquiry cost.
      
        // AND TEST WHERE CHANGING OWNERSHIP  to new company and immediately doing a lookup.

        public decimal ReverseEnquiryCost()
        {
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
                    service_type_name = "DRACORE CPY ENQUIRY",
                    enquiry_cost = successful ? _enquiryCost : (decimal?)null,
                    status_message = statusMessage,
                    exception = exceptionMessage,
                };

                prospectingDB.service_enquiry_logs.InsertOnSubmit(logEntry);
                prospectingDB.SubmitChanges();
            }
        }

        public void InitResponsePacket(CompanyEnquiryResponsePacket results)
        {
            _results = results;
        }
    }
}