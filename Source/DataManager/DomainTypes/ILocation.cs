using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ILocation
/// </summary>
public interface ILocation
{
    int? ParentLocationId { get; set; }
    int? LocationID { get; set; }
    string LocationName { get; set; }

    List<GeoLocation> PolyCoords { get; set; }

    List<IListing> Listings { get; set; }

    List<int> RelatedAreas { get; set; }
    List<int?> NeighboringAreas { get; set; }

    int? AreaTypeId { get; set; }

    string ResCommAgri { get; set; }

    bool UnderMaintenance { get; set; }
}