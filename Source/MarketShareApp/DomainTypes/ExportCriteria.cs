using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketShareApp.DomainTypes
{
    public class ExportCriteria : BaseDataRequestPacket
    {
        public List<SuburbInfo> Suburbs { get; set; }
        public bool FilterByRegDate { get; set; }
        public List<string> PropertyTypes { get; set; }
        public List<string> MarketshareTypes { get; set; }
        public List<string> Years { get; set; }
        public bool AgencyAssigned { get; set; }
        public bool NoAgencyAssigned { get; set; }
        public List<string> Months { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }
}