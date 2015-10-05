using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ProspectingProject
{

    public class ProspectingInputData : BaseDataRequestPacket
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
        public string SS_UNIQUE_IDENTIFIER { get; set; }

        // This caters for the different enquiry/lookup types : should be equal to one of the types in ProspectingStaticData.
        public string PersonLookupType { get; set; }

        // Hedonic data
        public int? ErfSize { get; set; }
        public int? DwellingSize { get; set; }
        public string Condition { get; set; }
        public int? Beds { get; set; }
        public int? Baths { get; set; }
        public int? Receptions { get; set; }
        public int? Studies { get; set; }
        public int? Garages { get; set; }
        public int? ParkingBays { get; set; }
        public bool? Pool { get; set; }
        public bool? StaffAccomodation { get; set; }

        public bool? IsShortTermRental { get; set; }
        public bool? IsLongTermRental { get; set; }
        public bool? IsCommercial { get; set; }
        public bool? IsAgricultural { get; set; }
        public bool? IsInvestment { get; set; }

        public bool? TitleCaseSS { get; set; }
    }
}