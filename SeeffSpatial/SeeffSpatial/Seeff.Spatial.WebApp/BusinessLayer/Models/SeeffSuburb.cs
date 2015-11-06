using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SeeffSuburb
    {
        public int SeeffAreaID { get; set; }
        public string AreaName { get; set; }

        [JsonConverter(typeof(Utils.DbGeographyConverter))]
        public DbGeography Centroid { get; set; }

        [JsonConverter(typeof(Utils.DbGeographyConverter))]
        public DbGeography Polygon { get; set; }

        public int? LicenseID { get; set; }
        public int? TerritoryID { get; set; }
    }
}