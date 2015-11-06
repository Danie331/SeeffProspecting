using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SeeffTerritory
    {
        public int TerritoryID { get; set; }
        public IList<SeeffLicense> Licenses { get; set; }
        [JsonConverter(typeof(Utils.DbGeographyConverter))]
        public DbGeography Centroid { get; set; }
        [JsonConverter(typeof(Utils.DbGeographyConverter))]
        public DbGeography Polygon { get; set; }
    }
}