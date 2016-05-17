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
        public static AreaValidationResult ValidateTerritory(SeeffTerritory territory)
        {
            try
            {
                if (territory.Polygon == null)
                {
                    return new AreaValidationResult { IsValid = false, ValidationMessage = "Not a valid polygon." };
                }

                TerritoryValidator validator = new TerritoryValidator(territory);
                return validator.Validate();
            }
            catch (Exception e)
            {
                Utils.LogException(e, "ValidateTerritory", territory);
                return new AreaValidationResult { IsValid = false, ValidationMessage = e.Message };
            }
        }
    }
}