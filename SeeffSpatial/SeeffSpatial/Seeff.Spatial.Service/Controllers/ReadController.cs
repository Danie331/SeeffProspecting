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
using System.Net.Http.Headers;

namespace Seeff.Spatial.Service.Controllers
{
    public class ReadController : ApiController
    {
        [HttpGet]
        public List<SpatialSuburb> GetAllSuburbs()
        {
            try
            {
                using (var spatialDb = new seeff_spatialEntities())
                {
                    List<SpatialSuburb> results = new List<SpatialSuburb>();
                    var query = spatialDb.spatial_area.Where(sub => !sub.is_deleted);
                    foreach (var rec in query)
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
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetAllSuburbs()", null);
                throw;
            }
        }

        [HttpGet]
        public List<SpatialSuburb> GetSuburbsListOnly()
        {
            try
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
                            IsDeleted = rec.is_deleted
                        };

                        results.Add(spatialSuburb);
                    }

                    return results;
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetSuburbsListOnly()", null);
                throw;
            }
        }

        [HttpGet]
        public List<SpatialLicense> GetAllLicenses()
        {
            try
            {
                List<SeeffLicense> licenseNames = null;
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri("http://localhost/bossservices/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = client.GetAsync("api/BOSS/GetLicenseList").Result;
                        response.EnsureSuccessStatusCode();
                        licenseNames = response.Content.ReadAsAsync<List<SeeffLicense>>().Result;
                    }
                    catch
                    {
                        // Supress: this code is only used to get license names
                    }
                }

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

                    if (licenseNames != null)
                    {
                        foreach (var item in results)
                        {
                            var targetLicense = licenseNames.FirstOrDefault(lic => lic.LicenseID == item.LicenseID);
                            item.LicenseName = targetLicense != null ? targetLicense.LicenseName : "(not available)";
                        }
                    }

                    return results;
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetAllLicenses()", null);
                throw;
            }
        }

        [HttpGet]
        public List<SpatialTerritory> GetAllTerritories()
        {
            try
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
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetAllTerritories()", null);
                throw;
            }
        }

        [HttpGet]
        public SpatialSuburb GetSuburbUnderMaintenance()
        {
            try
            {
                using (var spatialDb = new seeff_spatialEntities())
                {
                    var target = spatialDb.spatial_area.FirstOrDefault(sub => sub.under_maintenance);
                    if (target != null)
                    {
                        return new SpatialSuburb
                        {
                            AreaName = target.area_name
                        };
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetSuburbUnderMaintenance()", null);
                throw;
            }
        }

        [HttpGet]
        public List<SpatialSuburb> GetSuburbsUnderMaintenance()
        {
            try
            {
                using (var spatialDb = new seeff_spatialEntities())
                {
                    var targets = spatialDb.spatial_area.Where(sub => sub.requires_maintenance || sub.under_maintenance);
                    return (from sub in targets
                            select new SpatialSuburb
                            {
                                AreaName = sub.area_name,
                                SeeffAreaID = sub.fkAreaId
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetSuburbsUnderMaintenance()", null);
                throw;
            }
        }

        [HttpPost]
        public SpatialSuburb GetSuburbFromID([FromBody]int suburbID)
        {
            try
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
                            TerritoryID = existingRecord.fk_territory_id,
                            UnderMaintenance = existingRecord.requires_maintenance || existingRecord.under_maintenance
                        };
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetSuburbFromID()", null);
                throw;
            }
        }

        [HttpPost]
        public SpatialSuburb GetSuburbFromPoint([FromBody] SpatialPoint latLng)
        {
            string pointStr = null;
            try
            {
                if (latLng == null)
                    return null;
                using (var spatialDb = new seeff_spatialEntities())
                {
                    pointStr = "POINT (" + latLng.Lng + " " + latLng.Lat + ")";
                    DbGeography point = DbGeography.PointFromText(pointStr, DbGeography.DefaultCoordinateSystemId);
                    var containingArea = (from area in spatialDb.spatial_area
                                          where area.geo_polygon.Intersects(point) && !area.is_deleted
                                          select area).FirstOrDefault();
                    if (containingArea != null)
                    {
                        return new SpatialSuburb
                        {
                            AreaName = containingArea.area_name,
                            LicenseID = containingArea.fk_license_id,
                            Polygon = containingArea.geo_polygon,
                            SeeffAreaID = containingArea.fkAreaId,
                            TerritoryID = containingArea.fk_territory_id,
                            UnderMaintenance = containingArea.requires_maintenance || containingArea.under_maintenance
                        };
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetSuburbFromPoint()", pointStr);
                return null;
            }
        }

        [HttpPost]
        public SpatialLicense GetLicenseFromID([FromBody] int licenseID)
        {
            try
            {
                using (var spatialDb = new seeff_spatialEntities())
                {
                    SpatialLicense result = new SpatialLicense();
                    var target = spatialDb.spatial_license.FirstOrDefault(lic => lic.fk_license_id == licenseID);
                    if (target != null)
                    {
                        result.LicenseID = target.fk_license_id;
                        result.TerritoryID = target.fk_territory_id;

                        result.Suburbs = (from sub in spatialDb.spatial_area
                                          where sub.fk_license_id == target.fk_license_id
                                          select new SpatialSuburb
                                          {
                                              SeeffAreaID = sub.fkAreaId,
                                              AreaName = sub.area_name
                                          }).ToList();

                        return result;
                    };

                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "GetLicenseFromID()", null);
                throw;
            }
        }
    }
}
