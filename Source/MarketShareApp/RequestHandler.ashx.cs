using MarketShareApp.DomainTypes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
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
                case "auto_fate_all":
                    bool operationSuccess = AutoFateTransactionsForLicense();
                    string jsonOut = new JavaScriptSerializer().Serialize(operationSuccess);
                    context.Response.Write(jsonOut);
                    break;
                case "retrieve_list_of_developers":
                    var developersList = RetrieveDevelopersList();
                    string developersListJSON = new JavaScriptSerializer().Serialize(developersList);
                    context.Response.Write(developersListJSON);
                    break;
                case "search_candidate_developers":
                    DeveloperSearchText searchObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DeveloperSearchText>(json);
                    List<SearchCandidateDeveloperResult> candidateDevelopers = SearchCandidateDevelopers(searchObject.SearchText);
                    string candidateDevelopersJSON = new JavaScriptSerializer().Serialize(candidateDevelopers);
                    context.Response.Write(candidateDevelopersJSON);
                    break;
                case "save_developers":
                    DevelopersToSaveList listObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DevelopersToSaveList>(json);
                    bool saveResult = SaveDevelopers(listObject.DevelopersToSave);
                    context.Response.Write(new JavaScriptSerializer().Serialize(saveResult));
                    break;
                case "save_new_agency":
                    Agency agencyName = Newtonsoft.Json.JsonConvert.DeserializeObject<Agency>(json);
                    var saveAgencyResult = AddNewAgency(agencyName);
                    context.Response.Write(new JavaScriptSerializer().Serialize(saveAgencyResult));
                    break;
                case "download_export":
                    var exportCriteria = Newtonsoft.Json.JsonConvert.DeserializeObject<ExportCriteria>(json);
                    var exportResult = GenerateExportPackage(exportCriteria);
                    context.Response.Write(new JavaScriptSerializer().Serialize(exportResult));
                    break;
                default: break;
            }
        }

        private ExportResult GenerateExportPackage(ExportCriteria criteria)
        {
            try
            {
                using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                {
                    var suburbIds = criteria.Suburbs.Select(s => s.SuburbId).ToList();
                    var baseQuery = from n in lsBase.base_datas.Where(b => b.seeff_area_id.HasValue && suburbIds.Contains(b.seeff_area_id.Value))
                                    join q in lsBase.agencies on n.agency_id equals q.agency_id into nq
                                    from q in nq.DefaultIfEmpty()
                                    select new LightstoneListing
                                    {
                                        PropertyId = n.property_id,
                                        RegDate = n.iregdate,
                                        PurchDate = n.ipurchdate,
                                        PurchPrice = n.purch_price,
                                        SeeffAreaId = n.seeff_area_id,
                                        SeeffAreaName = "",
                                        MunicipalityName = n.munic_name,
                                        Province = n.province,
                                        PropertyType = n.property_type,
                                        ErfOrUnitSize = n.erf_size,
                                        BuyerName = n.buyer_name,
                                        SellerName = n.seller_name,
                                        EstateName = n.estate_name,
                                        LightstoneSuburb = n.suburb,
                                        SeeffDeal = n.seeff_deal,
                                        Fated = n.fated,
                                        FatedDate = n.fated_date,
                                        MarketShareType = n.market_share_type,
                                        AgencyName = q != null ? q.agency_name : "",
                                        StreetOrUnitNo = n.street_or_unit_no,
                                        PropertyAddress = n.property_address,
                                        ErfNo = n.erf_no,
                                        PortionNo = n.portion_no
                                    };

                    var trans = new List<LightstoneListing>();
                    foreach (var item in baseQuery.ToList())
                    {
                        if (!item.SeeffAreaId.HasValue) continue;
                        var target = criteria.Suburbs.First(s => s.SuburbId == item.SeeffAreaId.Value);
                        item.SeeffAreaName = target.SuburbName;

                        switch (target.FilterType)
                        {
                            case "fated":
                                if (item.Fated == true && item.MarketShareType != null)
                                {
                                    trans.Add(item);
                                }
                                break;
                            case "unfated":
                                if (!item.Fated.HasValue || item.Fated == false)
                                {
                                    trans.Add(item);
                                }
                                break;
                            case "all":
                                trans.Add(item);
                                break;
                        }
                    }

                    // Apply filters
                    var filtered = trans.Where(t => criteria.PropertyTypes.Contains(t.PropertyType))
                                        .Where(t => t.MarketShareType == null || criteria.MarketshareTypes.Contains(t.MarketShareType));

                    var monthLookup = new Dictionary<string, string>();
                    monthLookup["jan"] = "01";
                    monthLookup["feb"] = "02";
                    monthLookup["mar"] = "03";
                    monthLookup["apr"] = "04";
                    monthLookup["may"] = "05";
                    monthLookup["jun"] = "06";
                    monthLookup["jul"] = "07";
                    monthLookup["aug"] = "08";
                    monthLookup["sep"] = "09";
                    monthLookup["oct"] = "10";
                    monthLookup["nov"] = "11";
                    monthLookup["dec"] = "12";
                    var months = criteria.Months.Select(s => monthLookup[s]).ToList();
                    if (criteria.FilterByRegDate)
                    {
                        filtered = filtered.Where(t => criteria.Years.Contains(t.RegDate.Substring(0,4)));
                        filtered = filtered.Where(t => months.Contains(t.RegDate.Substring(4, 2)));
                    }
                    else
                    {
                        if (criteria.Years.Count > 0)
                        {
                            filtered = filtered.Where(t =>
                            {
                                if (!t.PurchDate.HasValue) return false;
                                var result = DateTime.TryParseExact(t.PurchDate.Value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);
                                return result ? criteria.Years.Contains(dt.Year.ToString()) : false;
                            });

                            filtered = filtered.Where(t =>
                            {
                                if (!t.PurchDate.HasValue) return false;
                                var result = DateTime.TryParseExact(t.PurchDate.Value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);
                                return result ? months.Contains(dt.ToString("yyyyMMdd").Substring(4, 2)) : false;
                            });
                        }
                    }

                    if (criteria.AgencyAssigned && criteria.NoAgencyAssigned) { }
                    else
                    {
                        if (criteria.AgencyAssigned)
                        {
                            filtered = filtered.Where(t => !string.IsNullOrEmpty(t.AgencyName));
                        }

                        if (criteria.NoAgencyAssigned)
                        {
                            filtered = filtered.Where(t => string.IsNullOrEmpty(t.AgencyName));
                        }
                    }

                    if (criteria.PriceFrom.HasValue)
                    {
                        filtered = filtered.Where(t => t.PurchPrice.HasValue && t.PurchPrice.Value >= criteria.PriceFrom);
                    }

                    if (criteria.PriceTo.HasValue)
                    {
                        filtered = filtered.Where(t => t.PurchPrice.HasValue && t.PurchPrice.Value <= criteria.PriceTo);
                    }

                    filtered = filtered.OrderBy(b => b.SeeffAreaName).ThenByDescending(c => c.PropertyId).ToList();
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        string exportName = "MarketshareExport_" + Path.GetRandomFileName().Substring(0, 5) + "_" + DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(exportName);
                        //worksheet.Cells["A1"].LoadFromDataTable(new DataTable(), true);

                        worksheet.Row(1).Style.Font.Bold = true;
                        worksheet.Row(1).Height = 20;
                        worksheet.Row(2).Height = 18;

                        var cols = new[] { "Lightstone Property ID", "Reg. Date", "Purch. Date", "Purch. Price",
                                    "Seeff Suburb", "Munic. Name", "Province", "Property Type",
                                    "ERF Size", "Buyer Name", "Seller Name", "Estate Name",
                                    "Lightstone Suburb", "Seeff Deal", "Fated", "Fated Date", "Market Share Type",
                                    "Selling Agent", "Street/Unit No.", "Property Address", "ERF no.", "Portion no." };
                        for (int i = 1; i <= cols.Length; i++)
                        {
                            worksheet.Cells[1, i].Value = cols[i - 1];
                        }

                        int rowNumber = 2;
                        foreach (var record in filtered)
                        {
                            worksheet.Cells[rowNumber, 1].Value = record.PropertyId;
                            worksheet.Cells[rowNumber, 2].Value = FormatDateString(record.RegDate);
                            worksheet.Cells[rowNumber, 3].Value = record.PurchDate.HasValue ? FormatDateString(record.PurchDate.ToString()) : "";
                            worksheet.Cells[rowNumber, 4].Value = record.PurchPrice.HasValue ? record.PurchPrice.Value.ToString("N0", CultureInfo.GetCultureInfo("en-ZA")) : "";
                            worksheet.Cells[rowNumber, 5].Value = record.SeeffAreaName ?? "";
                            worksheet.Cells[rowNumber, 6].Value = record.MunicipalityName ?? "";
                            worksheet.Cells[rowNumber, 7].Value = record.Province ?? "";
                            worksheet.Cells[rowNumber, 8].Value = record.PropertyType ?? "";
                            worksheet.Cells[rowNumber, 9].Value = record.ErfOrUnitSize.HasValue ? record.ErfOrUnitSize.Value.ToString() : "";
                            worksheet.Cells[rowNumber, 10].Value = record.BuyerName ?? "";
                            worksheet.Cells[rowNumber, 11].Value = record.SellerName ?? "";
                            worksheet.Cells[rowNumber, 12].Value = record.EstateName ?? "";
                            worksheet.Cells[rowNumber, 13].Value = record.LightstoneSuburb ?? "";
                            worksheet.Cells[rowNumber, 14].Value = record.SeeffDeal.HasValue ? (record.SeeffDeal.Value ? "Yes" : "") : "";
                            worksheet.Cells[rowNumber, 15].Value = record.Fated.HasValue ? (record.Fated.Value ? "Yes" : "No") : "No";
                            worksheet.Cells[rowNumber, 16].Value = record.FatedDate.HasValue ? record.FatedDate.Value.ToShortDateString() : "";
                            worksheet.Cells[rowNumber, 17].Value = FormatMarketShareType(record.MarketShareType) ?? "";
                            worksheet.Cells[rowNumber, 18].Value = record.AgencyName ?? "";
                            worksheet.Cells[rowNumber, 19].Value = record.StreetOrUnitNo ?? "";
                            worksheet.Cells[rowNumber, 20].Value = record.PropertyAddress ?? "";
                            worksheet.Cells[rowNumber, 21].Value = record.ErfNo.HasValue ? record.ErfNo.Value.ToString() : "";
                            worksheet.Cells[rowNumber, 22].Value = record.PortionNo.HasValue ? record.PortionNo.Value.ToString() : "";

                            rowNumber++;
                        }

                        for (int i = 1; i <= cols.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        string fileName = exportName + ".xlsx";
                        return SaveFile(fileName, package);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ExportResult { Success = false, Error = ex.Message };
            }
        }

        private string FormatMarketShareType(string marketShareType)
        {
            if (string.IsNullOrEmpty(marketShareType)) return null;

            switch (marketShareType)
            {
                case "R":
                    return "Residential";
                case "C":
                    return "Commercial";
                case "A":
                    return "Agri";
                case "D":
                    return "Development";
                case "O":
                    return "Other";
                case "P":
                    return "Pending";
                default: return "anomolous";
            }
        }

        private ExportResult SaveFile(string fileName, ExcelPackage package)
        {
            string exportPath = "/ExcelExtracts/" + fileName;

            var path = HttpContext.Current.Server.MapPath("~" + exportPath);
            FileInfo fs = new FileInfo(path);
            fs.Directory.Create();
            package.SaveAs(fs);

            string appFolderPath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath;
            appFolderPath = Path.Combine(appFolderPath, exportPath);
            return new ExportResult { Success = true, Filepath = appFolderPath };
        }

        private string FormatDateString(string date)
        {
            if (date == null) return "";
            var isDate = DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result);

            return isDate ? result.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
        }

        private Agency AddNewAgency(Agency agencyNameHolder)
        {
            if (string.IsNullOrWhiteSpace( agencyNameHolder.agency_name))
            {
                return null;
            }
            try
            {
                using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                {
                    agency newAgency = new agency
                    {
                         agency_name = agencyNameHolder.agency_name
                    };
                    lsBase.agencies.InsertOnSubmit(newAgency);
                    lsBase.SubmitChanges();

                    return new Agency { agency_name = newAgency.agency_name, agency_id = newAgency.agency_id };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool SaveDevelopers(List<string> developersToSave)
        {
            try
            {
                using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                {
                    bool changesMade = false;
                    foreach (var developerName in developersToSave)
                    {
                        var existingDeveloper = lsBase.developers.FirstOrDefault(dev => dev.fk_seller_name.ToUpper() == developerName.ToUpper());
                        if (existingDeveloper == null)
                        {
                            developer newDeveloper = new developer
                            {
                                fk_seller_name = developerName,
                                fk_registration_id = -1
                            };
                            lsBase.developers.InsertOnSubmit(newDeveloper);
                            lsBase.SubmitChanges();
                            changesMade = true;
                        }
                    }

                    if (!changesMade)
                        return true;

                    //
                    string commandText = @"update bd
                                            set bd.fated = 1, bd.market_share_type = 'D'
                                            from ls_base.dbo.base_data bd
                                            where bd.seller_name in (select fk_seller_name from ls_base.dbo.developers)";

                    SqlConnection conn = lsBase.Connection as SqlConnection;
                    SqlCommand cmd = new SqlCommand(commandText, conn);
                    conn.Open();
                    cmd.CommandTimeout = 300;
                    cmd.ExecuteNonQuery();
                    //

                    int baseYear = DateTime.Now.Year - 4;
                    lsBase.update_area_fating(baseYear.ToString());

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private List<SearchCandidateDeveloperResult> SearchCandidateDevelopers(string searchText)
        {
            try
            {
                List<SearchCandidateDeveloperResult> results = new List<SearchCandidateDeveloperResult>();
                using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                {
                    string commandText = @"SELECT [seller_name], COUNT([seller_name])   
                                    FROM [ls_base].[dbo].[base_data] 
                                    WHERE [seller_name] LIKE '%" + searchText + @"%'
                                    AND seller_name NOT IN (SELECT [fk_seller_name] FROM [ls_base].[dbo].[developers]) 
                                    GROUP BY [seller_name] 
                                    HAVING COUNT([seller_name]) > 10 
                                    ORDER BY [seller_name]";

                    SqlConnection conn = lsBase.Connection as SqlConnection;
                    SqlCommand cmd = new SqlCommand(commandText, conn);
                    conn.Open();
                    cmd.CommandTimeout = 120;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string sellerName = reader.GetString(0);
                                int numReg = reader.GetInt32(1);

                                SearchCandidateDeveloperResult result = new SearchCandidateDeveloperResult();
                                result.CandidateDeveloperName = sellerName;
                                result.NumRegistrations = numReg;
                                results.Add(result);
                            }
                        }
                    }
                    return results;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private List<string> RetrieveDevelopersList()
        {
            try
            {
                using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                {
                    List<string> results = new List<string>();
                    results = lsBase.developers.Select(dev => dev.fk_seller_name).OrderBy(name => name).ToList();

                    return results;
                }
            }
            catch
            {
                return null;
            }
        }

        private bool AutoFateTransactionsForLicense()
        {
            try
            {
                List<SuburbInfo> userSuburbs = (List<SuburbInfo>)HttpContext.Current.Session["user_suburbs"];
                using (var lsBase = DataManager.DataContextRetriever.GetLSBaseDataContext())
                {
                    var suburbIDs = userSuburbs.Select(u => u.SuburbId.ToString()).ToList();
                    string areasList = suburbIDs.Aggregate((x,y) => x.ToString() + "," + y.ToString());

                    lsBase.auto_fate_transactions(areasList);
                }                 

                return true;
            }
            catch (Exception ex)
            {
                return false;
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

    internal class ExportResult
    {
        public bool Success { get; set; }
        public string Filepath { get; set; }
        public string Error { get; internal set; }
    }
}