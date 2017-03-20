using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using DataManager;
using DataManager.SeeffSpatial;

public class Domain
{
    public Domain() { }

    public static List<GeoLocation> LoadPolyCoords(string polyWKT)
    {
        List<GeoLocation> polygon = new List<GeoLocation>();

        polyWKT = polyWKT.Replace("POLYGON ((", "").Replace("))", "");
        var coordPairs = polyWKT.Split(new[] { ',' });
        foreach (var item in coordPairs)
        {
            string[] coordPair = item.Trim().Split(new[] { ' ' });
            string lat = coordPair[1];
            string lng = coordPair[0];
            GeoLocation loc = new GeoLocation
            {
                Lat = Decimal.Parse(lat),
                Lng = Decimal.Parse(lng)
            };
            polygon.Add(loc);
        }

        return polygon;
    }

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
                                                                     return year > 2013;
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
        using (var ls_base = DataContextRetriever.GetLSBaseDataContext())
        {
            foreach (var seeffListing in ls_base.seeff_searches.Where(item => item.fkAreaId == areaId))
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
                        CurrentSeeffPropCategoryName = GetSearchCategoryName(ls_base, seeffListing.fkCategoryId),
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

    private static string GetSearchCategoryName(LightStoneDataContext ls_base, int categoryId)
    {
        return ls_base.seeff_categories.First(c => c.categoryId == categoryId).categoryName;
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
}