using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class OrphanedPropertiesResult
    {
        public string Message { get; internal set; }
        public bool Successful { get; internal set; }

        public IList<OrphanedProperty> Orphans { get; set; }
    }
}