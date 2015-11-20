using Seeff.Spatial.WebApp.BusinessLayer.Models;
using Seeff.Spatial.WebApp.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.ControllerActions
{
    public partial class ControllerActions
    {
        public static SaveSuburbResult SaveSuburb(SeeffSuburb suburb)
        {
            var validationResult = ControllerActions.ValidateSuburb(suburb);
            if (!validationResult.IsValid)
            {
                return new SaveSuburbResult
                {
                    Successful = false,
                    SaveMessage = validationResult.ValidationMessage
                };
            }

            PrepareSuburb(suburb, validationResult);
            SpatialDataWriter spatialWriter = new SpatialDataWriter();
            SeeffSuburb result = spatialWriter.SaveSuburb(suburb);
            result.ConvertSpatialToWKT();
            GlobalAreaCache.Instance.UpdateCacheItem(result);

            return new SaveSuburbResult
            {
                Successful = true,
                SuburbResult = result
            };
        }

        private static void PrepareSuburb(SeeffSuburb suburb, AreaValidationResult validation)
        {
            suburb.LicenseID = validation.LicenseID;
            suburb.TerritoryID = validation.TerritoryID;
        }
    }
}