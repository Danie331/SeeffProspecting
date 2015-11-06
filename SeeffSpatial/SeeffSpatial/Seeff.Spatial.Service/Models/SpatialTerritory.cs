using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.Models
{
    public class SpatialTerritory
    {
        public int? TerritoryID { get; set; }
        public string TerritoryName { get; set; }

        public DbGeography Centroid { get; set; }

        public DbGeography Polygon { get; set; }
    }
}