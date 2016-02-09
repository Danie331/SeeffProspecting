using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ProspectingSuburb : ILocation
    {
        public int? ParentLocationId { get; set; }   

        public int? LocationID { get; set; }   

        public string LocationName { get; set; }   

        public List<GeoLocation> PolyCoords { get; set; }   

        public List<IListing> Listings { get; set; }   

        public List<int> RelatedAreas { get; set; }         

        public List<int?> NeighboringAreas { get; set; }   

        public int? AreaTypeId{ get; set; }   

        public string ResCommAgri { get; set; }

        public List<ProspectingProperty> ProspectingProperties { get; set; }

        public bool UnderMaintenance { get; set; }
        public bool IsDeleted { get; set; }
    }
}