using Newtonsoft.Json;
using Seeff.Spatial.Service.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.BusinessLayer.Models
{
    public class SpatialTerritory: SpatialModelBase
    {
        public int? TerritoryID { get; set; }
        public string TerritoryName { get; set; }
    }
}