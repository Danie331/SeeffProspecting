using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SeeffLicense: ISeeffAreaCollection
    {
        public int LicenseID { get; set; }
        public int TerritoryID { get; set; }
        public IList<SeeffSuburb> Suburbs { get; set; }
        [JsonConverter(typeof(Utils.DbGeographyConverter))]
        public DbGeography Centroid { get; set; }
        [JsonConverter(typeof(Utils.DbGeographyConverter))]
        public DbGeography Polygon { get; set; }
    }
}