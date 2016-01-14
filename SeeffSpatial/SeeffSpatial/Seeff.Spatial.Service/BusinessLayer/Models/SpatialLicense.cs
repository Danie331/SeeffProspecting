using Newtonsoft.Json;
using Seeff.Spatial.Service.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.BusinessLayer.Models
{
    public class SpatialLicense: SpatialModelBase
    {
        public int? LicenseID { get; set; }
        public string LicenseName { get; set; }
        public int? TerritoryID { get; set; }

        public override int? PolyID
        {
            get { return LicenseID; }
        }

        public IList<SpatialSuburb> Suburbs { get; set; }
    }
}