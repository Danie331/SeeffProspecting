using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Listing
/// </summary>
public class LightstoneListing : IListing
{    
    /// <summary>
    ///  The following properties map to columns from base_data. Each record represents an actual sale of a property.
    ///  Inherit from IListing
    /// </summary>
    public GeoLocation LatLong { get; set; }
    public int? PropertyId { get; set; }
    public string UniqueId { get; set; }
    public string RegDate { get; set; }
    public decimal? PurchPrice { get; set; }
    public string ErfKey { get; set; }
    public string Suburb { get; set; }
    public string PropertyAddress { get; set; }
    public string StreetOrUnitNo { get; set; }
    public string SellerName { get; set; }
    public string BuyerName { get; set; }
    public string SS_FH { get; set; }
    public string MarketShareType { get; set; }
    public int? Agency { get; set; }
    public bool? SeeffDeal { get; set; }
    public bool? Fated { get; set; }
    public bool CanEdit { get; set; }
    public string TitleDeedNo { get; set; }
    public decimal? ErfOrUnitSize { get; set; }
    public int? ErfNo { get; set; }
    public int? PortionNo { get; set; }
    public bool? SaleIncludesOtherProperties { get; set; }
    public int? ParentPropertyId { get; set; }
    public string PropertyType { get; set; }
    public string EstateName { get; set; }
    public bool? IsCurrentSeeffListing { get; set; }
    public int? SeeffAreaId { get; set; }
    public decimal? PurchDate { get; set; }
}