using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class OrphanedProperty
    {
        public SpatialPoint LatLng { get; set; }
        public int? LightstonePropertyID { get; set; }
    }
}