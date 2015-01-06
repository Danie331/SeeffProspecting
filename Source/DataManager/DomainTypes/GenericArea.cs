using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

/// <summary>
/// An instance of this type represents potentially any Seeff area/suburb/region/city town/province.
/// </summary>
public class GenericArea : ILocation
{
    public int? ParentLocationId
    {
        get;
        set;
    }

    [JsonProperty(PropertyName="id")]
    public int? LocationID
    {
        get;
        set;
    }

     [JsonProperty(PropertyName = "Name")]
    public string LocationName
    {
        get;
        set;
    }

    public List<GeoLocation> PolyCoords
    {
        get;
        set;
    }

    public List<IListing> Listings
    {
        get;
        set;
    }

    public List<int> RelatedAreas
    {
        get;
        set;
    }

    public List<int?> NeighboringAreas
    {
        get;
        set;
    }


    public int? AreaTypeId
    {
        get;
        set;
    }

     [JsonProperty(PropertyName = "type")]
    public string ResCommAgri
    {
        get;
        set;
    }
}