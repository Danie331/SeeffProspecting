using Microsoft.SqlServer.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using Seeff.Spatial.WebApp.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer
{
    public class Utils
    {
        public static void LogException(Exception ex, string context, object contextObject)
        {
            using (var database = new spatial_web_appEntities()) 
            {
                string contextJson = JsonConvert.SerializeObject(contextObject ?? "");
                Guid userGuid = new Guid();
                try
                {
                    userGuid = (Guid)HttpContext.Current.Session["user_guid"];
                }
                catch { }                
                exception_log error_record = new exception_log
                {
                    created = DateTime.Now,
                    context = context,
                    raw_exception = ex.ToString(),
                    state_object_json = contextJson,
                    user = userGuid
                };

                database.exception_log.Add(error_record);
                database.SaveChanges();
            }
        }

        public static List<int> ParseAreaListString(string areaStringList)
        {
            if (string.IsNullOrWhiteSpace(areaStringList))
            {
                return Enumerable.Empty<int>().ToList();
            }

            Regex regexSplitter = new Regex(@"\[(.*?)\]");
            var matches = regexSplitter.Matches(areaStringList);
            if (matches.Count > 0)
            {
                List<int> areasList = new List<int>();
                var regex = new Regex(@"\s+");
                foreach (Match match in matches)
                {
                    string areaStr = regex.Replace(match.Value, "").Replace("[","").Replace(",1]","").Replace(",0]","");
                    int areaID;
                    if (int.TryParse(areaStr, out areaID))
                    {
                        areasList.Add(areaID);
                    }
                    else
                    {
                        throw new Exception("Unable to parse area list string on Login()");
                    }
                }
                return areasList;
            }

            return Enumerable.Empty<int>().ToList();
        }

        public class CustomBodyModelValidator : System.Web.Http.Validation.DefaultBodyModelValidator
        {
            public override bool ShouldValidateType(Type type)
            {
                return type != typeof(DbGeography) && base.ShouldValidateType(type);
            }
        }
    }
}