using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public class ExportLicenseModel
    {
        public int LicenseID { get; set; }

        public bool IncludeSuburbs { get; set; }

        public bool IncludeOrphanedRecords { get; set; }
    }
}