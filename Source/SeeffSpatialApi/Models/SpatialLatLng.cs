using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeeffSpatialApi.Models
{
    public class SpatialLatLng
    {
        public Guid Key { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}