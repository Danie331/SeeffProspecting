using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class SaveLicenseResult
    {
        public bool Successful { get; set; }
        public string SaveMessage { get; set; }
        public SeeffLicense LicenseResult { get; set; }
    }
}