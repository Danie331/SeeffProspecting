using Seeff.Spatial.WebApp.BusinessLayer.Models;
using Seeff.Spatial.WebApp.BusinessLayer.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.ControllerActions
{
    public partial class ControllerActions
    {
        public static AreaValidationResult ValidateLicense(SeeffLicense license)
        {
            try
            {
                if (license.Polygon == null)
                {
                    return new AreaValidationResult { IsValid = false, ValidationMessage = "Not a valid polygon." };
                }

                LicenseValidator validator = new LicenseValidator(license);
                return validator.Validate();
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "ValidateLicense()", license);
                return new AreaValidationResult { IsValid = false, ValidationMessage = ex.Message };
            }
        }
    }
}