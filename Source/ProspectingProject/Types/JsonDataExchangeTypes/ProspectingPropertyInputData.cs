using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ProspectingProject
{

    public class ProspectingPropertyInputData : BaseDataRequestPacket
    {
        // Basic
        public int? ProspectingPropertyId { get; set; }
        // Properties to handle the prospecting lookup 
        public string StreetOrUnitNo { get; set; }
        public string StreetName { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string StreetType { get; set; }
        public string LightstoneIDOrCKNo { get; set; }
        public int? SeeffAreaId { get; set; }
        // These handle new records for insert
        public int? LightstoneId { get; set; }
        public string PropertyAddress { get; set; }
        public GeoLocation LatLng { get; set; }
        public string LightstoneRegDate { get; set; }
        public decimal? LastPurchPrice { get; set; }
        public int? ErfNo { get; set; }

        public List<ProspectingContactPerson> ContactPersons { get; set; }
        public List<ProspectingContactCompany> ContactCompanies { get; set; }

        public string SS_FH { get; set; }
        public string SSName { get; set; }
        public string SSNumber { get; set; }
        public string Unit { get; set; }
        public string SS_ID { get; set; }
        public string SSDoorNo { get; set; }
        public Boolean Prospected { get; set; }
    }
}