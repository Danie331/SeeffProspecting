using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class ExportLicenseModelResult
    {
        public SeeffLicense SeeffLicense { get; set; }
        public string Message { get; internal set; }
        public bool Successful { get; internal set; }
    }
}