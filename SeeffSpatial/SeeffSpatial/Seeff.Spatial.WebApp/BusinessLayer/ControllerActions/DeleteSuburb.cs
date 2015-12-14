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
        public static DeleteSuburbResult DeleteSuburb(SeeffSuburb suburb)
        {
            SpatialDataWriter spatialWriter = new SpatialDataWriter();
            SeeffSuburb result = spatialWriter.DeleteSuburb(suburb);

            GlobalAreaCache.Instance.RemoveCacheItem(result);

            return new DeleteSuburbResult
            {
                Successful = true
            };
        }
    }
}