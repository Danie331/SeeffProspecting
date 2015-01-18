using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class LightstonePropertyMatch : IEqualityComparer<LightstonePropertyMatch>
    {
        public LightstonePropertyMatch()
        {
            Owners = new List<IProspectingContactEntity>();
        }

        /// <summary>
        /// Fields relating to the physical property itself
        /// </summary>
        public string DeedTown { get; set; }
        public string City { get; set; }
        public string Suburb { get; set; }
        public string StreetName { get; set; }
        public string StreetOrUnitNo { get; set; }
        public GeoLocation LatLng { get; set; }
        public int? LightstonePropId { get; set; }
        public string RegDate { get; set; }
        public string PurchPrice { get; set; }
        public string ErfNo { get; set; }
        
        public string SSName { get; set; }
        public string SSNumber { get; set; }
        public string Unit { get; set; }
        public string SS_UnitNoFrom { get; set; }
        public string SS_UnitTo { get; set; }
        public string SS_FH { get; set; }
        public string SS_ID { get; set; }

        // Farms
        public string FarmName { get; set; }
        public string Portion { get; set; }
        public string LightstoneSuburb { get; set; }

        public bool LightstoneIdExists { get; set; }

        [JsonIgnore]
        public List<IProspectingContactEntity> Owners { get; set; }

        public bool Equals(LightstonePropertyMatch x, LightstonePropertyMatch y)
        {
            return x.LightstonePropId == y.LightstonePropId;
        }

        public int GetHashCode(LightstonePropertyMatch obj)
        {
            return obj.City.GetHashCode() * obj.Suburb.GetHashCode() + obj.StreetName.GetHashCode() + obj.StreetOrUnitNo.GetHashCode();
        }
    }
}