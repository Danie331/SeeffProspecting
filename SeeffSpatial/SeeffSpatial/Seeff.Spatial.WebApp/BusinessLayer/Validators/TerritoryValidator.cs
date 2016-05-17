using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Validators
{
    public class TerritoryValidator
    {
        private SeeffTerritory _territory;
        public TerritoryValidator(SeeffTerritory territory)
        {
            _territory = territory;
        }

        public AreaValidationResult Validate()
        {
            AreaValidationResult result = new AreaValidationResult { IsValid = true, ConflictingPolys = new List<SpatialModelBase>() };

            // territories may not intersect territories, licenses, suburbs.
            // all licenses and suburbs owned by the territory must be fully contained by the territory.
            // all license and suburbs contained by the territory must be owned by the territory

            // Step 1: Find intersecting suburbs
            //List<SeeffSuburb> intersectingSuburbs = _territory.GetIntersectingPolys(GlobalAreaCache.Instance.AllSuburbs);
            //if (intersectingSuburbs.Count > 0)
            //{
            //    result.IsValid = false;
            //    result.ValidationMessage = "This territory's polygon intersects with one or more suburbs.";
            //    return result;
            //}

            //List<SeeffLicense> intersectingLicenses = _territory.GetIntersectingPolys(GlobalAreaCache.Instance.SeeffLicenses);
            //if (intersectingLicenses.Count > 0)
            //{
            //    result.IsValid = false;
            //    result.ValidationMessage = "This territory's polygon intersects with one or more licenses.";
            //    return result;
            //}

            //List<SeeffTerritory> intersectingTerritories = _territory.GetIntersectingPolys(GlobalAreaCache.Instance.SeeffTerritories);
            //if (intersectingTerritories.Count > 0)
            //{
            //    result.IsValid = false;
            //    result.ValidationMessage = "This territory's polygon intersects with one or more other territories.";
            //    return result;
            //}

            //List<SeeffSuburb> containedSuburbs = _territory.GetContainingPolys(GlobalAreaCache.Instance.AllSuburbs);
            //List<SeeffSuburb> ownedSuburbs = GlobalAreaCache.Instance.AllSuburbs.Where(sub => sub.TerritoryID == _territory.TerritoryID).ToList();
            //foreach (var containedSuburb in containedSuburbs)
            //{
            //    if (containedSuburb.TerritoryID != _territory.TerritoryID)
            //    {
            //        result.IsValid = false;
            //        result.ValidationMessage = "Suburb " + containedSuburb.AreaName + "(" + containedSuburb.SeeffAreaID + ") is contained by this territory but not owned by it.";
            //        return result;
            //    }
            //}

            //if (containedSuburbs.Count != ownedSuburbs.Count)
            //{
            //    result.IsValid = false;
            //    result.ValidationMessage = "The number of suburbs contained by this territory and the number owned by this territory differ.";
            //    return result;
            //}

            //List<SeeffLicense> containedLicenses = _territory.GetContainingPolys(GlobalAreaCache.Instance.SeeffLicenses);
            //List<SeeffLicense> ownedLicenses = GlobalAreaCache.Instance.SeeffLicenses.Where(lic => lic.TerritoryID == _territory.TerritoryID).ToList();
            //foreach (var containedLic in containedLicenses)
            //{
            //    if (containedLic.TerritoryID != _territory.TerritoryID)
            //    {
            //        result.IsValid = false;
            //        result.ValidationMessage = "License " + containedLic.LicenseName + "(" + containedLic.LicenseID + ") is contained by this territory but not owned by it.";
            //        return result;
            //    }
            //}

            //if (containedLicenses.Count != ownedLicenses.Count)
            //{
            //    result.IsValid = false;
            //    result.ValidationMessage = "The number of licenses contained by this territory and the number owned by this territory differ.";
            //    return result;
            //}

            return result;
        }
    }
}