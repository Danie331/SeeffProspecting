using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject.Services
{
    public class EnquiryServiceManager
    {
        private IPersonEnquiryService _personEnquiryService;
        public EnquiryServiceManager(ProspectingInputData inputData)
        {
            _personEnquiryService = EnquiryServiceFactory.GetPersonEnquiryService(inputData);
        }

        public bool IsPersonLookup()
        {
            return _personEnquiryService != null;
        }

        /// <summary>
        /// Performs the enquiry transaction and returns its output
        /// </summary>
        public PersonEnquiryResponsePacket PerformPersonEnquiry()
        {
            PersonEnquiryResponsePacket results = new PersonEnquiryResponsePacket();
            if (_personEnquiryService == null)
            {
                results.ErrorMsg = "The lookupType is not supported for this operation.";
                return results;
            }

            decimal? walletBalance = null;
            // Variables to keep track of the state of the transaction
            bool enquirySuccessful = false, deductionMade = false, deductionReimbursed = false;
            try
            {
                _personEnquiryService.InitResponsePacket(results);
                walletBalance = _personEnquiryService.DeductEnquiryCost(); // NB: Will return a value < 0 if insufficient funds
                if (walletBalance >= decimal.Zero)
                {
                    deductionMade = true;
                    _personEnquiryService.DoEnquiry();
                    if (PersonEnquiryResultsValid(results))
                    {
                        enquirySuccessful = true;
                    }
                    else
                    {
                        walletBalance = _personEnquiryService.ReverseEnquiryCost();
                        deductionReimbursed = true;
                    }
                    results.WalletBalance = walletBalance;
                }
                else
                {
                    results.ErrorMsg = "Insufficient funds to perform enquiry.";
                }
            }
            catch (Exception ex)
            {
                results.ErrorMsg = "Exception occurred performing enquiry: " + ex.Message;
                _personEnquiryService.SetError(ex);
            }
            finally
            {
                // Double check here that if enquiry failed we do not accidently bill the user.
                if (!enquirySuccessful && deductionMade && !deductionReimbursed)
                {
                    results.WalletBalance = _personEnquiryService.ReverseEnquiryCost();
                }
                _personEnquiryService.LogEnquiry();
            }

            return results;
        }

        /// <summary>
        /// This method validates the response from a personal enquiry.
        /// An enquiry is only considered valid under these conditions, if invalid the transaction charge must be reversed
        /// 1. The enquiry itself doesn't return with an error message or "no record found"
        /// 2. We have at least one of the following: a cell number, work number, home number, or email address - no contact info = invalid
        /// </summary>
        private bool PersonEnquiryResultsValid(PersonEnquiryResponsePacket results)
        {
            if (!results.EnquirySuccessful)
                return false;
            if (!string.IsNullOrEmpty(results.ErrorMsg)) // Test the error message just in case
                return false;
            if (results.ContactRows.Count == 0)
                return false;

            return true;
        }
    }
}