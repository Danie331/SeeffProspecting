using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Validators
{
    public class LicenseValidator
    {
         private SeeffLicense _license;
        public LicenseValidator(SeeffLicense license)
        {
            _license = license;
        }

        public AreaValidationResult Validate()
        {
            AreaValidationResult result = new AreaValidationResult { IsValid = true, ConflictingPolys = new List<SpatialModelBase>() };

            // #1: This license' boundaries may not intersect with another license boundaries
            // TODO
            //List<SeeffLicense> intersectingLicenses = _license.GetIntersectingPolysIgnoreTouching(GlobalAreaCache.Instance.SeeffLicenses);
            //if (intersectingLicenses.Count > 0)
            //{
            //    result.IsValid = false;
            //    result.ValidationMessage = "This license's polygon intersects with one or more licenses.";
            //    return result;
            //}

            // #2: License must be fully contained by a territory
            var territories = _license.GetContainingPolys(GlobalAreaCache.Instance.SeeffTerritories);
            if (territories.Count == 0)
            {
                result.IsValid = false;
                result.ValidationMessage = "This license is not fully contained by a territory.";
                return result;
            }
            if (territories.Count == 1)
            {
                result.TerritoryID = territories[0].TerritoryID;
            }

            // All other cases
            return result;
        }
    }
}