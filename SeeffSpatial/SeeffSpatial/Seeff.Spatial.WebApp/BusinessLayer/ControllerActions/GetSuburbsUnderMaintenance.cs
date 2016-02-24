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
        public static SuburbsUnderMaintenanceResult GetSuburbsUnderMaintenance()
        {
            SpatialDataReader spatialReader = new SpatialDataReader();
            var targetSuburbs = spatialReader.GetSuburbsUnderMaintenance();

            SuburbsUnderMaintenanceResult result = new SuburbsUnderMaintenanceResult
            {
                Successful = true,
                Suburbs = targetSuburbs
            };

            return result;         
        }
    }
}