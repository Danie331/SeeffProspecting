using Newtonsoft.Json;
using Seeff.Spatial.Service.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.BusinessLayer.Models
{
    public class SpatialSuburb: SpatialModelBase
    {
        public int? SeeffAreaID { get; set; }
        public int? LicenseID { get; set; }
        public int? TerritoryID { get; set; }
        public string AreaName { get; set; }       
    }
}