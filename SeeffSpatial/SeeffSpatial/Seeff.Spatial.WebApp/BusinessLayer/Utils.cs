using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
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
            // TODO.
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

        public class DbGeographyConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType.Equals(typeof(DbGeography));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return default(DbGeography);

                var jObject = JObject.Load(reader);

                if (!jObject.HasValues || (jObject["Geography"]["WellKnownText"] == null))
                    return default(DbGeography);

                string wkt = jObject["Geography"]["WellKnownText"].Value<string>();

                return DbGeography.FromText(wkt, DbGeography.DefaultCoordinateSystemId);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var dbGeography = value as DbGeography;

                if (dbGeography.WellKnownValue.WellKnownText.Contains("POINT"))
                {
                    string[] coordPair = dbGeography.WellKnownValue.WellKnownText.Replace("POINT (", "").Replace(")", "").Split(new[]{' '});
                    serializer.Serialize(writer, dbGeography == null || dbGeography.IsEmpty ? null : new { lat = double.Parse(coordPair[1]), lng = double.Parse(coordPair[0]) });
                }
                else if (dbGeography.WellKnownValue.WellKnownText.Contains("POLYGON"))
                {
                    string[] coordPairs = dbGeography.WellKnownValue.WellKnownText.Replace("POLYGON ((", "").Replace("))", "").Split(new[]{','});
                    List<object> set = new List<object>();
                    foreach (var item in coordPairs)
                    {
                        string[] coordPair = item.Trim().Split(new[] { ' ' });
                        double lat = double.Parse(coordPair[1]);
                        double lng = double.Parse(coordPair[0]);

                        set.Add(new { lat = lat, lng = lng });
                    }
                    // Serialise an array of lat/lng objects
                    serializer.Serialize(writer, dbGeography == null || dbGeography.IsEmpty ? null : set.ToArray());
                }               
            }
        }
    }
}