using Seeff.Spatial.Service.BusinessLayer;
using Seeff.Spatial.Service.Database;
using Seeff.Spatial.Service.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Spatial;

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
                        Polygon = rec.geo_polygon
                    };
              
                    results.Add(spatialSuburb);
                }

                return results;
            }
        }

        [HttpGet]
        public List<SpatialSuburb> GetSuburbsListOnly()
        {
            using (var spatialDb = new seeff_spatialEntities())
            {
                List<SpatialSuburb> results = new List<SpatialSuburb>();
                foreach (var rec in spatialDb.spatial_area)
                {
                    var spatialSuburb = new SpatialSuburb
                    {
                        AreaName = rec.area_name,
                        SeeffAreaID = rec.fkAreaId
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
                        Polygon = rec.geo_polygon
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

        [HttpPost]
        public SpatialSuburb GetSuburbFromID([FromBody]int suburbID)
        {
            using (var spatialDb = new seeff_spatialEntities())
            {
                var existingRecord = spatialDb.spatial_area.FirstOrDefault(sub => sub.fkAreaId == suburbID);
                if (existingRecord != null)
                {
                    return new SpatialSuburb
                    {
                        AreaName = existingRecord.area_name,
                        LicenseID = existingRecord.fk_license_id,
                        Polygon = existingRecord.geo_polygon,
                        SeeffAreaID = existingRecord.fkAreaId,
                        TerritoryID = existingRecord.fk_territory_id
                    };
                }

                return null;
            }
        }

        [HttpPost]
        public SpatialSuburb GetSuburbFromPoint([FromBody] SpatialPoint latLng)
        { // TEST THIS!!!
            if (latLng == null)
                return null;
            using (var spatialDb = new seeff_spatialEntities())
            {
                string pointStr = "POINT (" + latLng.Lng + " " + latLng.Lat + ")";
                DbGeography point = DbGeography.PointFromText(pointStr, DbGeography.DefaultCoordinateSystemId);
                var containingArea = (from area in spatialDb.spatial_area
                                      where area.geo_polygon.Intersects(point)
                                      select area).FirstOrDefault();
                if (containingArea != null)
                {
                    return new SpatialSuburb
                    {
                        AreaName = containingArea.area_name,
                        LicenseID = containingArea.fk_license_id,
                        Polygon = containingArea.geo_polygon,
                        SeeffAreaID = containingArea.fkAreaId,
                        TerritoryID = containingArea.fk_territory_id
                    };
                }
                return null;
            }
        }
    }
}
