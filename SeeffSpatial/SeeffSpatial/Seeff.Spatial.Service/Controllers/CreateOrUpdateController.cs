using Seeff.Spatial.Service.Database;
using Seeff.Spatial.Service.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seeff.Spatial.Service.BusinessLayer;

namespace Seeff.Spatial.Service.Controllers
{
    public class CreateOrUpdateController : ApiController
    {
        [HttpPost]
        public SpatialSuburb SaveSuburb([FromBody] SpatialSuburb suburb)
        {
            try
            {
                using (var spatialDb = new seeff_spatialEntities())
                {
                    if (!suburb.SeeffAreaID.HasValue)
                    {
                        int nextAreaID = spatialDb.spatial_area.Max(sub => sub.fkAreaId).Value + 1;
                        suburb.SeeffAreaID = nextAreaID;
                    }
                    var existingrecord = spatialDb.spatial_area.FirstOrDefault(area => area.fkAreaId == suburb.SeeffAreaID);
                    if (existingrecord != null)
                    {
                        existingrecord.geo_polygon = suburb.Polygon;
                    }
                    else
                    {
                        existingrecord = new spatial_area
                        {
                            area_name = suburb.AreaName,
                            fk_license_id = suburb.LicenseID,
                            fk_territory_id = suburb.TerritoryID,
                            fkAreaId = suburb.SeeffAreaID,
                            geo_polygon = suburb.Polygon
                        };
                        spatialDb.spatial_area.Add(existingrecord);
                    }

                    spatialDb.SaveChanges();

                    var result = new SpatialSuburb
                    {
                        AreaName = existingrecord.area_name,
                        LicenseID = existingrecord.fk_license_id,
                        Polygon = existingrecord.geo_polygon,
                        SeeffAreaID = existingrecord.fkAreaId,
                        TerritoryID = existingrecord.fk_territory_id
                    };
       
                    return result;
                }
            }
            catch(Exception ex) 
            {
                Utils.LogException(ex, "SaveSuburb()", suburb);
                throw;
            }
        }
    }
}
