using Seeff.Spatial.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.BusinessLayer
{
    public class Utils
    {
        public static List<GeoLocation> ParseKMLString(string input)
        {
            if (string.IsNullOrEmpty(input)) 
            {
                return new List<GeoLocation>();
            }
            try
            {
                string[] pairs = input.Split(new[] { ' ' });
                List<GeoLocation> coords = (from pair in pairs
                                            let pairSet = pair.Split(new[] { ',' })
                                            select new GeoLocation
                                            {
                                                Lat = decimal.Parse(pairSet[1]),
                                                Lng = decimal.Parse(pairSet[0])
                                            }).ToList();

                return coords;
            }
            catch (Exception ex)
            {
                LogException(ex, "ParseKMLString", input);
                throw;
            }
        }

        public static void LogException(Exception ex, string context, object contextObject)
        {
            // TODO.
        }
    }
}