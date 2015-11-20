using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SeeffSuburb : SpatialModelBase
    {
        public int? SeeffAreaID { get; set; }

        public string AreaName { get; set; }

        public int? LicenseID { get; set; }

        public int? TerritoryID { get; set; }

        public override int? PolyID
        {
            get { return SeeffAreaID; }
        }
    }
}