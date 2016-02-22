using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class DracoreEmailAddressLookup : IPersonEnquiryService
    {
        private decimal _enquiryCost = 0.4m;

        private Guid _userGuid;
        private IDracoreService _dracoreService;
        private ProspectingInputData _inputData;
        private Exception _exception = null;
        private PersonEnquiryResponsePacket _results = null;
        public DracoreEmailAddressLookup(IDracoreService dracoreService, ProspectingInputData inputData)
        {
            _dracoreService = dracoreService;
            _inputData = inputData;
            _userGuid = Guid.Parse((string)HttpContext.Current.Session["user_guid"]);
        }

        public decimal DeductEnquiryCost()
        {
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

            Consumer002 dracoreResult = null;
            try
            {
                dracoreResult = _dracoreService.ByIdTYPE002(long.Parse(_results.IdNumber, CultureInfo.InvariantCulture));
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

            // Now process the results
            _results.OwnerGender = ProspectingCore.DetermineOwnerGender(_results.IdNumber);

            // Add the contact rows
            string email = dracoreResult.EMAIL_1;

            // Business rule: we do not pay if the record does not contain any contact information, we must therefore fail the enquiry
            if (string.IsNullOrWhiteSpace(email))
            {
                _results.EnquirySuccessful = false;
                _results.ErrorMsg = "No Dracore email address found for this record.";
                return;
            }

            _results.ContactRows.Add(new ContactRow { EmailAddress = email, Type = "email", Date = DateTime.Now.ToString() });
            _results.HWCE_indicator = "0001";
        }

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
                string exceptionMessage = _exception != null ? _exception.ToString() : null;

                string idNumber = _results.IdNumber;
                if (idNumber.Length > 13 )
                {
                    idNumber = idNumber.Substring(0, 13);
                }
                if (statusMessage != null && statusMessage.Length > 255)
                {
                    statusMessage = statusMessage.Substring(0, 255);
                }

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
            _results.LookupType = ProspectingLookupData.DracoreEmailEnquiryRequest;
        }
    }
}