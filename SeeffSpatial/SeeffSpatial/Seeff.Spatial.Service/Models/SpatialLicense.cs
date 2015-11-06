using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.Models
{
    public class SpatialLicense
    {
        public int? LicenseID { get; set; }
        public int? TerritoryID { get; set; }

        public DbGeography Centroid { get; set; }

        public DbGeography Polygon { get; set; }
    }
}