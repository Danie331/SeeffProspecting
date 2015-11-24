using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class UnmappedSuburbs
    {
        public List<SeeffSuburb> Suburbs { get; set; }
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }
    }
}