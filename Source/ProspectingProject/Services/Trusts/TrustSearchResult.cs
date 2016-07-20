using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ProspectingProject
{
    public class TrustSearchResult
    {
        public TrustSearchResult(XElement trusteeDetail, int contactCompanyID, int prospectingPropertyID)
        {
            TrustName = trusteeDetail.Element("TrustName")?.Value;
            TrustNumber = trusteeDetail.Element("TrustNumber")?.Value;
            TrustHighCourt = trusteeDetail.Element("HighCourt")?.Value;
            HasPersonData = trusteeDetail.Element("PersonalData")?.Value;
            ProspectingPropertyID = prospectingPropertyID;
            ContactCompanyID = contactCompanyID;
        }

        public string TrustName { get; private set; }
        public string TrustNumber { get; private set; }
        public string TrustHighCourt { get; private set; }
        public string HasPersonData { get; private set; }

        public int ProspectingPropertyID { get; set; }
        public int ContactCompanyID { get; set; } // Trust ID in "prospecting company" table

        public int id
        {
            get
            {
                return TrustName.GetHashCode() + TrustNumber.GetHashCode() + TrustHighCourt.GetHashCode();
            }
        }
    }
}