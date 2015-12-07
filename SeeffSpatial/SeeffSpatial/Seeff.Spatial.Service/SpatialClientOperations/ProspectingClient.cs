using Seeff.Spatial.Service.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.SpatialClientOperations
{
    public class ProspectingClient
    {
        public void ReindexSuburbAreaIDs(int? seeffAreaID)
        {   // test that the method runs async and test for concurrency issues when executing before previous execution finishes + test how activities + folowups are carried over from re-indexed props.
            if (seeffAreaID.HasValue)
            {
                using (var prospecting = new seeff_prospectingEntities())
                {
                    using (var spatialDb = new seeff_spatialEntities())
                    {
                        var affectedProperties = prospecting.prospecting_property.Where(pp => pp.seeff_area_id == seeffAreaID);
                        // FH and FRM..
                        var freeHolds = affectedProperties.Where(pp => pp.ss_fh == "FH" || pp.ss_fh == "FRM" || pp.ss_fh == null);
                        foreach (var record in freeHolds)
                        {
                            string pointStr = "POINT (" + record.longitude + " " + record.latitude + ")";
                            DbGeography point = DbGeography.PointFromText(pointStr, DbGeography.DefaultCoordinateSystemId);
                            var containingArea = (from area in spatialDb.spatial_area
                                                  where area.geo_polygon.Intersects(point)
                                                  select area).FirstOrDefault();

                            record.seeff_area_id = containingArea != null ? containingArea.fkAreaId : -1;
                        }

                        // SS
                        var sectionalTitleUnits = affectedProperties.Except(freeHolds);
                        var sectionalTitles = sectionalTitleUnits.GroupBy(unit => unit.ss_unique_identifier);
                        Dictionary<string, int?> unitAreaIDLookup = new Dictionary<string, int?>();
                        foreach (var complex in sectionalTitles)
                        {
                            var unit = complex.First();
                            string pointStr = "POINT (" + unit.longitude + " " + unit.latitude + ")";
                            DbGeography point = DbGeography.PointFromText(pointStr, DbGeography.DefaultCoordinateSystemId);
                            var containingArea = (from area in spatialDb.spatial_area
                                                  where area.geo_polygon.Intersects(point)
                                                  select area).FirstOrDefault();

                            int? areaID = containingArea != null ? containingArea.fkAreaId : -1;
                            unitAreaIDLookup[unit.ss_unique_identifier] = areaID;
                        }
                        foreach (var unit in sectionalTitleUnits)
                        {
                            int? areaID = unitAreaIDLookup[unit.ss_unique_identifier];
                            unit.seeff_area_id = areaID;
                        }

                        prospecting.SaveChanges();
                    }
                }
            }
        }
    }
}