using Seeff.Spatial.WebApp.BusinessLayer;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.ServiceInterface
{
    public class SpatialDataWriter: ServiceBase
    {
        public SeeffSuburb SaveSuburb(SeeffSuburb suburb)
        {
            try
            {
                suburb = PostToService<SeeffSuburb>("api/CreateOrUpdate/SaveSuburb", suburb);
                suburb.ConvertWktToSpatial();
                return suburb;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "SaveSuburb()", null);
                throw;
            }
        }
    }
}