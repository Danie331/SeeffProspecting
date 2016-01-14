using Seeff.Spatial.WebApp.BusinessLayer.Models;
using Seeff.Spatial.WebApp.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.ControllerActions
{
    public partial class ControllerActions
    {
        public static OrphanedPropertiesResult GetOrphanedProperties(SeeffLicense license)
        {
            OrphanedPropertiesResult result = new OrphanedPropertiesResult();
            result.Successful = true;
            using (var ls_base = new ls_baseEntities())
            {
                var orphanedRecords = ls_base.base_data.Where(bd => bd.seeff_lic_id == license.LicenseID)
                                                       .Where(bd => bd.seeff_area_id == -1)
                                                       .Where(bd => bd.y != null && bd.x != null)
                                                       .Select(bd => new { bd.y, bd.x, bd.property_id }).ToList();

                orphanedRecords = orphanedRecords.GroupBy(g => g.property_id).Select(g => g.First()).ToList();
                List<OrphanedProperty> orphans = (from o in orphanedRecords
                                                  let lat = Convert.ToDouble(o.y)
                                                  let lng = Convert.ToDouble(o.x)
                                                  select new OrphanedProperty
                                                  {
                                                      LatLng = new SpatialPoint { Lat = lat, Lng = lng },
                                                      LightstonePropertyID = o.property_id
                                                  }).ToList();
                result.Orphans = orphans;
            }

            return result;
        }
    }
}