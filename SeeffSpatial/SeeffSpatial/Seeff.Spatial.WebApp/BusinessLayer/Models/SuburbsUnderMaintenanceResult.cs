using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SuburbsUnderMaintenanceResult
    {
        public string Message { get; internal set; }
        public List<SeeffSuburb> Suburbs { get; set; }
        public bool Successful { get; internal set; }
    }
}