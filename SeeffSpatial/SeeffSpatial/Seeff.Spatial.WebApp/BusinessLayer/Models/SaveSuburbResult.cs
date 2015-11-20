using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SaveSuburbResult
    {
        public bool Successful { get; set; }
        public string SaveMessage { get; set; }
        public SeeffSuburb SuburbResult { get; set; }
    }
}