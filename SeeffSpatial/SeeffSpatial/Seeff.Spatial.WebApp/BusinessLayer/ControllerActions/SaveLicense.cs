using Seeff.Spatial.WebApp.BusinessLayer.Models;
using Seeff.Spatial.WebApp.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.ControllerActions
{
    public partial class ControllerActions
    {
        public static SaveLicenseResult SaveLicense(SeeffLicense license)
        {
            var validationResult = ControllerActions.ValidateLicense(license);
            if (!validationResult.IsValid)
            {
                return new SaveLicenseResult
                {
                    Successful = false,
                    SaveMessage = validationResult.ValidationMessage
                };
            }

            SpatialDataWriter spatialWriter = new SpatialDataWriter();
            SeeffLicense result = spatialWriter.SaveLicense(license);

            result = GlobalAreaCache.Instance.UpdateCacheItem(result);

            return new SaveLicenseResult
            {
                Successful = true,
                LicenseResult = result
            };
        }
    }
}