using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class DracorePhoneNumberLookup : IPersonEnquiryService
    {
        private decimal _enquiryCost = 0.6m;

        private Guid _userGuid;
        private IDracoreService _dracoreService;
        private ProspectingInputData _inputData;
        private Exception _exception = null;
        private PersonEnquiryResponsePacket _results = null;
        public DracorePhoneNumberLookup(IDracoreService dracoreService, ProspectingInputData inputData)
        {
            _dracoreService = dracoreService;
            _inputData = inputData;
            _userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
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

            Consumer001 dracoreResult = null;
            try
            {
                if (RequestHandler.IsTrainingMode())
                {
                    dracoreResult = GenerateTestReponseData();
                }
                else
                {
                    dracoreResult = _dracoreService.ByIdTYPE001(long.Parse(_results.IdNumber, CultureInfo.InvariantCulture));
                }
            }
            catch (Exception ex)
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = ex.Message;
                return;
            }

            // Check #2: If a service error occurred or no record was found, the enquiry was not successful.
            if (!string.IsNullOrEmpty(dracoreResult.SERVICE_ERROR) && dracoreResult.SERVICE_ERROR != "none")
            {
                _results.EnquirySuccessful = false;
                if (dracoreResult.SERVICE_ERROR.ToLower().Contains("record not found"))
                {
                    _results.ErrorMsg = "No Dracore record found for this ID number.";
                }
                else
                {
                    _results.ErrorMsg = "Dracore service not available. Enquiry failed with message: " + dracoreResult.SERVICE_ERROR;
                }
                return;
            }

            Func<string, string> purgeField = item => string.IsNullOrWhiteSpace(item) ? null : item;

            // Now process the results
            string title = purgeField(dracoreResult.TITLE);
            if (!string.IsNullOrWhiteSpace(title))
            {
                var kvp = ProspectingLookupData.ContactPersonTitle.FirstOrDefault(l => l.Value.ToUpper() == title.ToUpper());
                title = kvp.Key > 0 ? kvp.Key.ToString() : null;
            }
            _results.Title = title;
            _results.OwnerName = purgeField(dracoreResult.FIRST_NAME);
            _results.OwnerSurname = purgeField(dracoreResult.SURNAME);
            _results.DeceasedStatus = purgeField(dracoreResult.DECEASED_STATUS);
            _results.Citizenship = purgeField(dracoreResult.CITIZENSHIP);
            _results.OwnerGender = ProspectingCore.DetermineOwnerGender(_results.IdNumber);
            _results.AgeGroup = purgeField(dracoreResult.AGE_GROUP);
            _results.Location = purgeField(dracoreResult.LOCATION);
            _results.MaritalStatus = purgeField(dracoreResult.MARITAL_STATUS1);
            _results.HomeOwnership = purgeField(dracoreResult.HOMEOWNERSHIP);
            _results.Directorship = purgeField(dracoreResult.DIRECTORSHIP1);
            _results.PhysicalAddress = purgeField(dracoreResult.PHYSICALADDR1);

            // Add the contact rows
            string cellPhone = ProspectingCore.GetContactNumber(dracoreResult.CELL_1);
            string homePhone = ProspectingCore.GetContactNumber(dracoreResult.HOME_1);
            string workPhone = ProspectingCore.GetContactNumber(dracoreResult.WORK_1);

            // Business rule: we do not pay if the record does not contain any contact information, we must therefore fail the enquiry
            if (new[] { cellPhone, homePhone, workPhone }.All(item => string.IsNullOrWhiteSpace(item)))
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "No Dracore contact phone numbers found for this record.";
                return;
            }

            char[] HWCE_indicators = "0000".ToCharArray(); // NB order in which we add flags NB.

            if (!string.IsNullOrWhiteSpace(homePhone))
            {
                _results.ContactRows.Add(new ContactRow { Phone = homePhone, Type = "home", Date = DateTime.Now.ToString() });
                HWCE_indicators[0] = '1';
            }
            if (!string.IsNullOrWhiteSpace(workPhone))
            {
                _results.ContactRows.Add(new ContactRow { Phone = workPhone, Type = "work", Date = DateTime.Now.ToString() });
                HWCE_indicators[1] = '1';
            }
            if (!string.IsNullOrWhiteSpace(cellPhone))
            {
                _results.ContactRows.Add(new ContactRow { Phone = cellPhone, Type = "cell", Date = DateTime.Now.ToString() });
                HWCE_indicators[2] = '1';
            }
            HWCE_indicators[3] = '0';

            _results.HWCE_indicator = new String(HWCE_indicators);
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


        public void InitResponsePacket(PersonEnquiryResponsePacket results)
        {
            _results = results;
            _results.IdNumber = _inputData.LightstoneIDOrCKNo;
            _results.LookupType = ProspectingLookupData.DracorePhoneEnquiryRequest;
        }

        private Consumer001 GenerateTestReponseData()
        {
            return new Consumer001
            {
                SERVICE_ERROR = null,
                TITLE = "Mr",
                FIRST_NAME = "Danie",
                SURNAME = "van der Merwe",
                DECEASED_STATUS = "Alive",
                CITIZENSHIP = "South Africa",
                AGE_GROUP = "thirties",
                MARITAL_STATUS1 = "Unmarried",
                LOCATION = "South Africa",
                HOMEOWNERSHIP = "none",
                DIRECTORSHIP1 = "none",
                PHYSICALADDR1 = "n/a",
                CELL_1 = "0724707471",
                HOME_1 = "0218528166",
                WORK_1 = "0211231231"
            };
        }
    }
}