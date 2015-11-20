using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SeeffTerritory: SpatialModelBase
    {
        public int TerritoryID { get; set; }

        public IList<SeeffLicense> Licenses { get; set; }

        public override int? PolyID
        {
            get { return TerritoryID; }
        }
    }
}