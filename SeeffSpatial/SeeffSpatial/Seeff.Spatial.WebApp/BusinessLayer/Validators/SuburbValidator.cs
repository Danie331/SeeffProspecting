using Seeff.Spatial.WebApp.BusinessLayer.Models;
using Seeff.Spatial.WebApp.BusinessLayer.Validators;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer
{
    public class SuburbValidator 
    {
        private SeeffSuburb _suburb;
        public SuburbValidator(SeeffSuburb suburb)
        {
            _suburb = suburb;
        }

        public AreaValidationResult Validate()
        {
            AreaValidationResult result = new AreaValidationResult { IsValid = true, ConflictingPolys = new List<SpatialModelBase>() };
          
            // Step 1: Find intersecting suburbs
            List<SeeffSuburb> intersectingSuburbs = _suburb.GetIntersectingPolys(GlobalAreaCache.Instance.AllSuburbs);             
            foreach (var suburb in intersectingSuburbs) // IMPLEMENT RULES.
            {
                result.ConflictingPolys.Add(suburb);
            }
            if (result.ConflictingPolys.Count > 0)
            {
                result.IsValid = false;
                result.ValidationMessage = "This suburbs polygon overlaps one or more polygon(s).";
                return result;
            }

            // Step 2: Find the license ID to which this suburb belongs
            List<SeeffLicense> containingLicenses = _suburb.GetPolysContainingCentroid(GlobalAreaCache.Instance.SeeffLicenses);
            int? licenseID = null;
            if (containingLicenses.Count == 0)
            {
                if (!_suburb.LicenseID.HasValue || _suburb.LicenseID == 0)
                {
                    result.IsValid = false;
                    result.ValidationMessage = "Suburb must belong to a License.";
                    return result;
                } else {
                    licenseID = _suburb.LicenseID; // User-allocated
                }
            }
            if (containingLicenses.Count == 1)
            {
                if (_suburb.LicenseID.HasValue && _suburb.LicenseID > 0)
                {
                    if (_suburb.LicenseID != containingLicenses[0].LicenseID)
                    {
                        result.IsValid = false;
                        result.ValidationMessage = "This suburb belongs to a different license (" + containingLicenses[0].LicenseID + ") than the one provided.";
                        return result;
                    } else{
                        licenseID = containingLicenses[0].LicenseID;
                    }
                } else {
                    licenseID = containingLicenses[0].LicenseID;
                }
            }
            if (containingLicenses.Count > 1) // TODO: Check this business logic (can a suburb be owned by multiple licenses?)
            {
                result.IsValid = false;
                result.ValidationMessage = "This suburb is contained by multiple license(s). Cannot save.";
                return result;
            }
            result.LicenseID = result.IsValid ? licenseID : null;

            // Step 3: Suburb must have a name
            if (string.IsNullOrWhiteSpace(_suburb.AreaName))
            {
                result.IsValid = false;
                result.ValidationMessage = "Suburb must have a name.";
                return result;
            }

            // Step 4: Suburb must belong to a territory  (TODO: query this)
            var territories = _suburb.GetContainingPolys(GlobalAreaCache.Instance.SeeffTerritories); // Need to find a polygon that completely envelopes this suburb.
            if (territories.Count == 0)
            {
                result.IsValid = false;
                result.ValidationMessage = "This suburb is not fully contained by a territory.";
                return result;
            }
            if (territories.Count == 1)
            {
                result.TerritoryID = territories[0].TerritoryID;
            }
            if (territories.Count > 1)
            {
                result.IsValid = false;
                result.ValidationMessage = "This suburb is fully contained by multiple territories. Cannot save.";
                return result;
            }

            return result;
        }
    }
}