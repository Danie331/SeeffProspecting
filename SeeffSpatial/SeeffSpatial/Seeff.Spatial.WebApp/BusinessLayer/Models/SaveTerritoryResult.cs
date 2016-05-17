using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SaveTerritoryResult
    {
        public string SaveMessage { get; internal set; }
        public bool Successful { get; internal set; }
        public SeeffTerritory TerritoryResult { get; internal set; }
    }
}