using Seeff.Spatial.Service.BusinessLayer;
using Seeff.Spatial.Service.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Seeff.Spatial.Service.SpatialClientOperations
{
    public class SpatialClients
    {
        public static void PropogateChangesAsync(SpatialModelBase spatialObject)
        {
            Task.Run(() =>
            {
                try
                {
                    if (spatialObject is SpatialSuburb)
                    {
                        var prospectingClient = new ProspectingClient();
                        prospectingClient.ReindexSuburbAreaIDs(spatialObject.PolyID);
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogException(ex, "SpatialClients.PropogateChanges()", spatialObject);
                }
            });
        }
    }
}