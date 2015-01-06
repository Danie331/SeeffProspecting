using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IListing
{
    GeoLocation LatLong { get; set; }
    int? PropertyId { get; set; }
    string UniqueId { get; set; }
    string RegDate { get; set; }
    decimal? PurchPrice { get; set; }
    string ErfKey { get; set; }
    string Suburb { get; set; }
    string PropertyAddress { get; set; }
    string StreetOrUnitNo { get; set; }
    string SellerName { get; set; }
    string BuyerName { get; set; }
    string SS_FH { get; set; }
    string MarketShareType { get; set; }
    int? Agency { get; set; }
    bool? SeeffDeal { get; set; }
    bool? Fated { get; set; }
    bool CanEdit { get; set; }
    string TitleDeedNo { get; set; }
    decimal? ErfOrUnitSize { get; set; }
    int? ErfNo { get; set; }
    int? PortionNo { get; set; }
    bool? SaleIncludesOtherProperties { get; set; }
    int? ParentPropertyId { get; set; }
    string PropertyType { get; set; }
    string EstateName { get; set; }
    bool? IsCurrentSeeffListing { get; set; }
    int? SeeffAreaId { get; set; }
}
