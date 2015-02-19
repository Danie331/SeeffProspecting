using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace MarketShareApp
{
    public class RequestHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string json = context.Request.Form[0];
            BaseDataRequestPacket request = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseDataRequestPacket>(json);
            switch (request.Instruction)
            {
                case "perform_search":
                    SearchInputData searchData = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchInputData>(json);
                    string searchResults = Search(searchData);
                    context.Response.Write(searchResults);
                    break;
                default: break;
            }
        }

        private string Search(SearchInputData searchData)
        {
            List<base_data> userBaseData = HttpContext.Current.Session["user_base_data"] as List<base_data>;
            if (userBaseData == null)
            {
                userBaseData = LoadUserBaseDataIntoSession();
            }

            // Inclusive search for now until told otherwise
            var deedTownResults = !string.IsNullOrEmpty(searchData.DeedTown) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.munic_name))
                {
                    return searchData.DeedTown.ToLower().Contains(b.munic_name.ToLower());
                }
                return false;
            }) : Enumerable.Empty<base_data>();

            var suburbResults = !string.IsNullOrEmpty(searchData.Suburb) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.suburb))
                {
                    return searchData.Suburb.ToLower().Contains(b.suburb.ToLower());
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var streetNameResults = !string.IsNullOrEmpty(searchData.StreetName) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.property_address))
                {
                    return b.property_address.ToLower().Contains(searchData.StreetName.ToLower());
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var streetNoResults = !string.IsNullOrEmpty(searchData.StreetNumber) ? streetNameResults.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.street_or_unit_no))
                {
                    return searchData.StreetNumber == b.street_or_unit_no;
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var complexNameResults = !string.IsNullOrEmpty(searchData.ComplexName) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.property_address))
                {
                    return b.property_address.ToLower().Contains(searchData.ComplexName.ToLower());
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var estateNameResults = !string.IsNullOrEmpty(searchData.EstateName) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.property_address))
                {
                    return b.property_address.ToLower().Contains(searchData.EstateName.ToLower());
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var erfNoResults = !string.IsNullOrEmpty(searchData.ErfNo) ? userBaseData.Where(b =>
            {
                if (b.erf_no != null)
                {
                    return b.erf_no.ToString() == searchData.ErfNo;
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var portionNoResults = !string.IsNullOrEmpty(searchData.PortionNo) ? erfNoResults.Where(b =>
            {
                if (b.erf_no != null && b.portion_no != null)
                {
                    return b.erf_no.ToString() == searchData.ErfNo && b.portion_no.ToString() == searchData.PortionNo;
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var propertyIdResults = !string.IsNullOrEmpty(searchData.PropertyId) ? userBaseData.Where(b =>
            {
                if (b.property_id != null)
                {
                    return b.property_id.ToString() == searchData.PropertyId;
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var titleDeedresults = !string.IsNullOrEmpty(searchData.TitleDeed) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.title_deed_no))
                {
                    return b.title_deed_no == searchData.TitleDeed;
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var buyerNameResults = !string.IsNullOrEmpty(searchData.BuyerName) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.buyer_name))
                {
                    return b.buyer_name.ToLower().Contains(searchData.BuyerName.ToLower());
                }
                return false;
            }) : Enumerable.Empty<base_data>();
            var sellerNameResults = !string.IsNullOrEmpty(searchData.SellerName) ? userBaseData.Where(b =>
            {
                if (!string.IsNullOrEmpty(b.seller_name))
                {
                    return b.seller_name.ToLower().Contains(searchData.SellerName.ToLower());
                }
                return false;
            }) : Enumerable.Empty<base_data>();

            IEnumerable<base_data> combinedResults = Enumerable.Empty<base_data>();
            if (!string.IsNullOrEmpty(searchData.StreetName) && !string.IsNullOrEmpty(searchData.StreetNumber))
            {
                combinedResults = streetNoResults;
            }
            else if (!string.IsNullOrEmpty(searchData.ErfNo) && !string.IsNullOrEmpty(searchData.PortionNo))
            {
                combinedResults = portionNoResults;
            }
            else
            {
                combinedResults = deedTownResults.Union(suburbResults)
                                                .Union(streetNameResults)
                                                .Union(streetNoResults)
                                                .Union(complexNameResults)
                                                .Union(estateNameResults)
                                                .Union(erfNoResults)
                                                .Union(portionNoResults)
                                                .Union(propertyIdResults)
                                                .Union(titleDeedresults)
                                                .Union(buyerNameResults)
                                                .Union(sellerNameResults).Distinct(); // Distinct records only
            }

            // Distinct by property id too to give us actual properties
            var uniquePropertyId = combinedResults.GroupBy(p => p.property_id);
            List<base_data> distinctResultsByPropId = uniquePropertyId.Select(gr => gr.First()).ToList();

            distinctResultsByPropId = distinctResultsByPropId.OrderByDescending(s => s.ss_fh)
                                                             .ThenBy(s =>
                                                             {
                                                                 int streetOrUnitNo;
                                                                 if (int.TryParse(s.street_or_unit_no, out streetOrUnitNo))
                                                                 {
                                                                     return streetOrUnitNo;
                                                                 }
                                                                 return -1;
                                                             }).ToList();

            SearchResultsData results = new SearchResultsData();
            results.Data = distinctResultsByPropId;
            results.Count = distinctResultsByPropId.Count;

            return Newtonsoft.Json.JsonConvert.SerializeObject(results);
        }

        private List<base_data> LoadUserBaseDataIntoSession()
        {
            using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
            {
                List<SuburbInfo> userSuburbs = HttpContext.Current.Session["user_suburbs"] as List<SuburbInfo>;
                var suburbAreaIds = userSuburbs.Select(s => s.SuburbId).ToList();

                List<base_data> baseData = lsBase.base_datas
                                                .Where(b => b.seeff_area_id != null)
                                                .Where(b => suburbAreaIds.Contains(b.seeff_area_id.Value))
                                                .OrderBy(o => o.seeff_area_id).ToList();

                HttpContext.Current.Session["user_base_data"] = baseData;
                return baseData;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}