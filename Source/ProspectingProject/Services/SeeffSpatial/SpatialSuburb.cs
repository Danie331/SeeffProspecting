using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject.Services.SeeffSpatial
{
    public class SpatialSuburb
    {
        public string PolyWKT { get; set; }
        public int? SeeffAreaID { get; set; }
        public int? LicenseID { get; set; }
        public int? TerritoryID { get; set; }
        public string AreaName { get; set; }
    }
}