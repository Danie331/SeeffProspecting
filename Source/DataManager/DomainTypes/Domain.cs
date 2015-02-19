using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using DataManager;

public class Domain
{
    public Domain() { }

    public static List<GeoLocation> LoadPolyCoords(int suburbId, string resCommAgri = "R")
    {
        using (var seeffDb = DataContextRetriever.GetSeeffDataContext())
        {
            return (from entry in seeffDb.kml_areas
                    where entry.area_id == suburbId && entry.area_type == resCommAgri.ToCharArray()[0]
                    orderby entry.seq ascending
                    select new GeoLocation
                    {
                        Lat = entry.latitude,
                        Lng = entry.longitude
                    }).ToList();
        }
    }

    public static List<GeoLocation> LoadPolyCoords(SeeffDataContext dataContext, int suburbId, string resCommAgri = "R")
    {
        return (from entry in dataContext.kml_areas
                where entry.area_id == suburbId && entry.area_type == resCommAgri.ToCharArray()[0]
                orderby entry.seq ascending
                select new GeoLocation
                {
                    Lat = entry.latitude,
                    Lng = entry.longitude
                }).ToList();
    }

    public static List<GeoLocation> PolyCoordsFromString(string inputSet)
    {
        string[] coordSets = inputSet.Split(new[] { ',' });
        return (from c in coordSets
                let latLongSet = c.Split(new[] { ' ' })
                select new GeoLocation
                {
                    Lat = Decimal.Parse(latLongSet[0]),
                    Lng = Decimal.Parse(latLongSet[1])
                }).ToList();
    }

    public static string PolyCoordsToString(LightStoneDataContext ls_base, List<GeoLocation> polyCoords)
    {
        string result = string.Join(",", from p in polyCoords select p.Lat + " " + p.Lng);
        result = MakePolygonValid(ls_base, result);

        return result;
    }

    // Ensure that the coords are valid before being populated into the table, ie
    // the start and end points of the polygon must be the same -  if not then replace last point with first one
    private static string MakePolygonValid(LightStoneDataContext ls_base, string polyCoords)
    {
        string[] coordSets = polyCoords.Split(new[] { ',' });
        string result = polyCoords;
        bool sameStartAndEnd = coordSets[0].Trim() == coordSets[coordSets.Length - 1].Trim();

        // Step 1: Ensure that the polygon has the same start/end coordinates
        if (!sameStartAndEnd)
        {
            coordSets[coordSets.Length - 1] = coordSets[0];
            result = string.Join(",", coordSets);
        }

        // Step 2: Validate the polygon against SQL servers geography engine. If it's invalid, reverse the order of the coordinates.
        if (ls_base.validate_polygon_coords(result) == 0)
        {
            coordSets = result.Split(new[] { ',' });
            result = string.Join(",", coordSets.Reverse());
        }

        return result;
    }

    public static List<GeoLocation> MakePolygonValid(LightStoneDataContext ls_base, List<GeoLocation> polyCoords)
    {
        // Step 1: Ensure that start and end points the same
        if (!polyCoords[0].Equals(polyCoords[polyCoords.Count - 1]))
        {
            polyCoords.Add(new GeoLocation { Lat = polyCoords[0].Lat, Lng = polyCoords[0].Lng });
        }

        // Step 1: Validate the polygon against SQL server's geogrpahy engine. If invalid, reverse order of coords.
        var coords = string.Join(",", polyCoords.Select(p => p.ToString()));
        if (ls_base.validate_polygon_coords(coords) == 0)
        {
            polyCoords.Reverse();
        }

        return polyCoords;
    }

    public static string GetAreaName(int areaId)
    {
        using (var seeff = DataContextRetriever.GetSeeffDataContext())
        {
            return (from item in seeff.areas
                    where item.areaId == areaId
                    select item.areaName).FirstOrDefault();
        }
    }

    /// <summary>
    /// This method always loads its data from lightstone (ls_base) regardless of the type parameter T.
    /// The purpose of the type parameter T is only to ensure that we return a list of the correct type the consumer is expecting, not to create a fully populated instance of the type.
    /// This method should ideally be replaced with a factory method returning a list of the fully-populated target type. 
    /// </summary>
    public static List<LightstoneListing> LoadLightstoneListingsForSuburb(int suburbId)
    {
        var allListings = new List<LightstoneListing>();
        using (var lsBase = DataContextRetriever.GetLSBaseDataContext())
        {
            var queryToRetrieveSuburbData = lsBase.base_datas.Where(i => i.seeff_area_id == suburbId).ToList();
            queryToRetrieveSuburbData = queryToRetrieveSuburbData.Where(b =>
                                                             {
                                                                 if (b.iregdate != null)
                                                                 {
                                                                     int year = DateTime.ParseExact(b.iregdate, "yyyyMMdd", null).Year;
                                                                     return year > 2011;
                                                                 }
                                                                 return false;
                                                             }).ToList();

            foreach (var listing in queryToRetrieveSuburbData)
            {
                LightstoneListing item = new LightstoneListing();
                item.PropertyId = listing.property_id;
                item.UniqueId = listing.unique_id;
                item.LatLong = new GeoLocation { Lat = listing.y.Value, Lng = listing.x.Value };
                item.SS_FH = listing.ss_fh;
                item.ErfNo = listing.erf_no;
                item.ErfKey = listing.erf_key;
                item.Suburb = listing.seeff_suburb;
                item.TitleDeedNo = listing.title_deed_no;
                item.ErfOrUnitSize = listing.erf_size;
                //SectSchemeName = listing.SECT_SCHEME_NAME,
                item.PropertyAddress = listing.property_address;
                item.StreetOrUnitNo = listing.street_or_unit_no;
                item.RegDate = listing.iregdate;
                item.PurchPrice = listing.purch_price;
                item.MarketShareType = listing.market_share_type;
                item.Fated = listing.fated;
                item.SeeffDeal = listing.seeff_deal;
                item.Agency = listing.agency_id;
                item.SellerName = listing.seller_name;
                item.BuyerName = listing.buyer_name;
                item.IsCurrentSeeffListing = false; // Indicates this is not a current seeff listing.
                item.SaleIncludesOtherProperties = listing.sale_includes_others_flag;
                item.ParentPropertyId = listing.parent_property_id;
                item.PropertyType = listing.property_type;
                item.EstateName = listing.estate_name;
                item.SeeffAreaId = listing.seeff_area_id;
                allListings.Add(item);
            }
        }  

        return allListings;
    }

    public static List<SeeffListing> LoadCurrentSeeffListings(int areaId)
    {
        var seeffListings = new List<SeeffListing>();
        using (var seeff = DataContextRetriever.GetSeeffDataContext())
        {
            foreach (var seeffListing in seeff.searches.Where(item => item.fkAreaId == areaId))
            {
                Decimal lat, lng;
                if (Domain.ConvertToLatLng(seeffListing.searchLatitude, seeffListing.searchLongitude, out lat, out lng))
                {
                    seeffListings.Add(new SeeffListing
                    {
                        IsCurrentSeeffListing = true,
                        LatLong = new GeoLocation { Lat = lat, Lng = lng },
                        CurrentSeeffAgentId = seeffListing.fkAgentId,
                        CurrentSeeffAreaId = seeffListing.fkAreaId,
                        CurrentSeeffAreaName = seeffListing.searchAreaName,
                        CurrentSeeffAreaParentName = seeffListing.searchAreaParentName,
                        CurrentSeeffPropCategoryName = GetSearchCategoryName(seeff, seeffListing.fkCategoryId),
                        CurrentSeeffPropertyId = seeffListing.fkPropertyId,
                        CurrentSeeffPropTypeName = seeffListing.searchPropertyTypeName,
                        CurrentSeeffRentalTerm = seeffListing.searchRentalTerm,
                        CurrentSeeffSearchImage = seeffListing.searchImage,
                        CurrentSeeffSearchPrice = seeffListing.searchPrice,
                        CurrentSeeffSearchReference = seeffListing.searchReference,
                        CurrentSeeffSearchRentalTerm = seeffListing.searchRentalTerm,
                        CurrentSeeffSaleOrRent = seeffListing.fkActionId,
                        CurrentSeeffPropIsSS = false
                    });
                }
            }
        }

        // Determine whether any props are part of an SS by matching x/y coords and set the appropriate property value.
        var sameLatLong = seeffListings.GroupBy(listing => listing.LatLong, new GeoLocationComparer())
                                       .Where(g => g.Count() > 1);
        foreach (var grouping in sameLatLong)
        {
            // This checks to make sure that there are both rentals and sales at this SS.
            // TODO: what if there are only sales or only rentals? Different icon?
            //if (grouping.Any(listing => listing.CurrentSeeffSaleOrRent == 2) &&
            //    grouping.Any(listing => listing.CurrentSeeffSaleOrRent == 3))
            //{
                foreach (var listing in grouping)
                {
                    listing.CurrentSeeffPropIsSS = true;
                }
            //}
        }

        return seeffListings;
    }

    private static string GetSearchCategoryName(SeeffDataContext seeff, int categoryId)
    {
        return seeff.categories.First(c => c.categoryId == categoryId).categoryName;
    }

    public static List<int> LoadRelatedAreas(int areaId)
    {
        using (var seeff = DataContextRetriever.GetSeeffDataContext())
        {
            return (from a in seeff.related_areas
                      where a.fkAreaId == areaId
                      select a.fkRelatedAreaId).ToList();
        }
    }

    public static List<int?> LoadNeighboringAreas(int areaId)
    {
        using (var seeff = DataContextRetriever.GetSeeffDataContext())
        {
            return  (from a in seeff.neighbouring_areas
                      where a.fkAreaId == areaId
                      select a.fkNeighbouringAreaId).ToList();
        }
    }

    public static int? GetParentAreaId(int areaId)
    {
        using (var seeff = DataContextRetriever.GetSeeffDataContext())
        {
            return seeff.areas.Where(a => a.areaId == areaId).Select(a => a.areaParentId).FirstOrDefault();
        }
    }

    public static string LoadAllSavedPolygonTypes(int areaId)
    {
        using (var ls_base = DataContextRetriever.GetLSBaseDataContext())
        {
            var areaTypes = from al in ls_base.area_layers where al.area_id == areaId select al.area_type;
            return string.Join(",", areaTypes);
        }
    }

    public static int? GetAreaTypeId(int areaId)
    {
        using (var seeff = DataContextRetriever.GetSeeffDataContext())
        {
            return seeff.areas.Where(a => a.areaId == areaId).Select(ai => ai.fkAreaTypeId).FirstOrDefault();
        }
    }

    public static JsonSerializerSettings CreateDefaultJsonSettings()
    {
        return new JsonSerializerSettings { ContractResolver = new JsonNetPropertyNameResolverForSerialization() };
    }

    public static bool ConvertToLatLng(string latInput, string lngInput, out Decimal latOutput, out Decimal lngOutput)
    {
        latOutput = 0;
        lngOutput = 0;
        try
        {
            latOutput = Convert.ToDecimal(latInput, CultureInfo.InvariantCulture);
            lngOutput = Convert.ToDecimal(lngInput, CultureInfo.InvariantCulture);

            return true;
        }
        catch{}

        return false;
    }

    public static string SerializeToJsonWithDefaults(object obj)
    {
        return JsonConvert.SerializeObject(obj, CreateDefaultJsonSettings());
    }

    public static T Deserialise<T>(string json)
    {
        return (T)JsonConvert.DeserializeObject<T>(json);
    }

    public static int? FindSeeffAreaId(GeoLocation point)
    {
        using (var ls_base = DataContextRetriever.GetLSBaseDataContext())
        {
            int? result = null;
            foreach (var provId in new[] { 2, 11, 12, 13, 14, 15, 16, 17, 18 })
            {
                result = ls_base.find_area_id(point.Lat, point.Lng, "R", provId);
                if (result > 0)
                {
                    return result;
                }
            }
        }
        return null;
    }

    public static int? FindSeeffAreaId(int? lightstonePropertyId)
    {
        using (var ls_base = DataContextRetriever.GetLSBaseDataContext())
        {
            return (from prop in ls_base.base_datas
                    where prop.property_id == lightstonePropertyId
                    select prop.seeff_area_id).FirstOrDefault();
        }
    }
}