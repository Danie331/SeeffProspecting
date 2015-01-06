using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class SeeffListing : IListing
    {
        /// <summary>
        /// These properties represent a current Seeff listing on the market. 
        /// </summary>
        public int CurrentSeeffAreaId { get; set; }
        public decimal CurrentSeeffPropertyId { get; set; }
        public int? CurrentSeeffAgentId { get; set; }
        public string CurrentSeeffAreaName { get; set; }
        public string CurrentSeeffAreaParentName { get; set; }
        public string CurrentSeeffPropCategoryName { get; set; }
        public string CurrentSeeffPropTypeName { get; set; }
        public decimal? CurrentSeeffSearchPrice { get; set; }
        public string CurrentSeeffRentalTerm { get; set; } // in case this is a rental, searchPrice (rental term)
        public string CurrentSeeffSearchReference { get; set; }
        public string CurrentSeeffSearchImage { get; set; }
        public string CurrentSeeffSearchRentalTerm { get; set; }
        public int CurrentSeeffSaleOrRent { get; set; }
        public bool CurrentSeeffPropIsSS { get; set; }

        // Fields below inherted from IListing: Please note these are not used at the moment for current seeff listings in MarketShare,
        // the interface is only implemented to conform with the consumer's expectation, which combines Lighstone listings and current listings in the same List<IListing>
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
    }

