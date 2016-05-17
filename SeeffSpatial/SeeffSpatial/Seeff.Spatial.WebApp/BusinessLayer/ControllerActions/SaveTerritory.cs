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
        public static SaveTerritoryResult SaveTerritory(SeeffTerritory territory)
        {
            var validationResult = ControllerActions.ValidateTerritory(territory);
            if (!validationResult.IsValid)
            {
                return new SaveTerritoryResult
                {
                    Successful = false,
                    SaveMessage = validationResult.ValidationMessage
                };
            }

            SpatialDataWriter spatialWriter = new SpatialDataWriter();
            SeeffTerritory result = spatialWriter.SaveTerritory(territory);

            GlobalAreaCache.Instance.UpdateCacheItem(result);

            return new SaveTerritoryResult
            {
                Successful = true,
                TerritoryResult = result
            };
        }
    }
}