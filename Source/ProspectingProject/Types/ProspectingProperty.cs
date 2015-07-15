using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ProspectingProperty : IEqualityComparer<ProspectingProperty>
    {
        //public List<LightstoneListing> LightstoneSales { get; set; }    // lightstoneListings   
        public ProspectingProperty()
        {
            //LightstoneSales = new List<LightstoneListing>();
        }

        public int? ProspectingPropertyId { get; set; } // If null, there is no record for this in the prospecting_row_header header table.               
        public GeoLocation LatLng { get; set; }
        public int? LightstonePropertyId { get; set; }
        public string PropertyAddress { get; set; }
        public string StreetOrUnitNo { get; set; }
        public int? SeeffAreaId { get; set; }

        public List<ProspectingContactPerson> Contacts { get; set; }
        public List<ProspectingContactCompany> ContactCompanies { get; set; }

        //public bool HasTracePSEnquiry { get; set; }
        public string LightstoneIDOrCKNo { get; set; }
        public string LightstoneRegDate { get; set; }
        public string Comments { get; set; }
        //public bool HasContactsWithDetails { get; set; }
        public decimal? LastPurchPrice { get; set; }

        public string SS_FH { get; set; }
        public string SSName { get; set; }
        public string SSNumber { get; set; }
        public string Unit { get; set; }
        public string SS_ID { get; set; }
        public string SSDoorNo { get; set; }
        public Boolean Prospected { get; set; }
        // Lightstone prop id for this unit?
        public string SS_UNIQUE_IDENTIFIER { get; set; }

        public int? ErfNo { get; set; }

        // Farms
        public string FarmName { get; set; }
        public string Portion { get; set; }
        public string LightstoneSuburb { get; set; }

        public string CreateError { get; set; } // Holds information on whether there was a problem creating this prospect 

        public ActivityBundle ActivityBundle { get; set; }

        public bool Equals(ProspectingProperty x, ProspectingProperty y)
        {
            if (x == null || y == null)
                return false;

            return x.ProspectingPropertyId == y.ProspectingPropertyId;
        }

        public int GetHashCode(ProspectingProperty obj)
        {
            return ProspectingPropertyId.HasValue ? ProspectingPropertyId.Value : -999999;
        }

        public bool? IsLockedByOtherUser { get; set; }

        public string LockedUsername { get; set; }

        public DateTime? LockedDateTime { get; set; }

        public string LatestRegDateForUpdate { get; set; }

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
    }
}