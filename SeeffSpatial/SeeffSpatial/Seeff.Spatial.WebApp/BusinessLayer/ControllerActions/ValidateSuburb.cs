using Microsoft.SqlServer.Types;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.ControllerActions
{
    public partial class ControllerActions
    {
        public static AreaValidationResult ValidateSuburb(SeeffSuburb suburb)
        {
            try
            {
                if (suburb.Polygon == null)
                {
                    return new AreaValidationResult { IsValid=false, ValidationMessage = "Not a valid polygon." };
                }
  
                SuburbValidator validator = new SuburbValidator(suburb);
                return validator.Validate();
            }
            catch(Exception e)
            {
                Utils.LogException(e, "ValidateSuburbPoly", suburb);
                return new AreaValidationResult { IsValid = false, ValidationMessage = e.Message };
            }           
        }
    }
}