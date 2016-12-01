using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public static class EnquiryServiceFactory
    {
        public static IPersonEnquiryService GetPersonEnquiryService(ProspectingInputData inputData)
        {
            string lookupType = inputData.PersonLookupType;
            if (string.IsNullOrEmpty(lookupType))
                return null;

            switch (lookupType)
            {
                case ProspectingLookupData.DracorePhoneEnquiryRequest: 
                {
                    IDracoreService dracoreService = GetDracoreService();
                    return new DracorePhoneNumberLookup(dracoreService, inputData);
                }
                case ProspectingLookupData.DracoreEmailEnquiryRequest:
                {
                    IDracoreService dracoreService = GetDracoreService();
                    return new DracoreEmailAddressLookup(dracoreService, inputData);
                }
                case ProspectingLookupData.TracePSEnquiryRequest:
                {
                    ITracepsService tpsService = GetTracePsService();
                    return new TracepsPhoneNumberLookup(tpsService, inputData);
                }
                default: throw new Exception("lookupType not found for a personal enquiry.");
            }
        }

        private static ITracepsService GetTracePsService()
        {
            bool testingMode = IsTestEnvironment;
            return testingMode ? (ITracepsService)new TracepsTestService() : (ITracepsService)new TracepsLiveService();
        }

        private static IDracoreService GetDracoreService()
        {
            return IsTestEnvironment ? (IDracoreService)new DracoreTestService() : (IDracoreService)new DracoreLiveService();
        }

        private static bool IsTestEnvironment
        {
            get { return HttpContext.Current.IsDebuggingEnabled; }
        }
    }
}