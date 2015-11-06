using Seeff.Spatial.Service.BusinessLayer;
using Seeff.Spatial.Service.Database;
using Seeff.Spatial.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Seeff.Spatial.Service.Controllers
{
    public class ReadController : ApiController
    {
        [HttpGet]
        public List<SpatialSuburb> GetAllSuburbs()
        {
            using (var spatialDb = new seeff_spatialEntities())
            {
                List<SpatialSuburb> results = new List<SpatialSuburb>();
                foreach (var rec in spatialDb.spatial_area)
                {
                    var spatialSuburb = new SpatialSuburb
                    {
                        AreaName = rec.area_name,
                        SeeffAreaID = rec.fkAreaId,
                        LicenseID = rec.fk_license_id,
                        TerritoryID = rec.fk_territory_id,
                        Polygon = rec.geo_polygon,
                        Centroid = rec.area_center_point
                    };
                    results.Add(spatialSuburb);
                }

                return results;
            }
        }

        [HttpGet]
        public List<SpatialLicense> GetAllLicenses()
        {
            using (var spatialDb = new seeff_spatialEntities())
            {
                List<SpatialLicense> results = new List<SpatialLicense>();
                foreach (var rec in spatialDb.spatial_license)
                {
                    var spatialLicense = new SpatialLicense
                    {

                        LicenseID = rec.fk_license_id,
                        TerritoryID = rec.fk_territory_id,
                        Polygon = rec.geo_polygon,
                        Centroid = rec.area_center_point
                    };
                    results.Add(spatialLicense);
                }

                return results;
            }
        }

        [HttpGet]
        public List<SpatialTerritory> GetAllTerritories()
        {
            using (var spatialDb = new seeff_spatialEntities())
            {
                List<SpatialTerritory> results = new List<SpatialTerritory>();
                foreach (var rec in spatialDb.spatial_terretory)
                {
                    var spatialTerritory = new SpatialTerritory
                    {
                        TerritoryID = rec.territory_id,
                        TerritoryName = rec.territory_name,
                         Polygon = rec.geo_polygon
                    };
                    results.Add(spatialTerritory);
                }
                return results;
            }
        }
    }
}
