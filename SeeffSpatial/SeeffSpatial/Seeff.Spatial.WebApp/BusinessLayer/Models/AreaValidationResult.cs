using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class AreaValidationResult
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public List<SpatialModelBase> ConflictingPolys { get; set; }
        public int? LicenseID { get; set; }
        public int? TerritoryID { get; set; }
    }
}