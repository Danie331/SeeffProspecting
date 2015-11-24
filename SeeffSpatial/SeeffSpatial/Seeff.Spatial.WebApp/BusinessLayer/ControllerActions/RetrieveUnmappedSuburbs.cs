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
        public static UnmappedSuburbs RetrieveUnmappedSuburbs()
        {
            using (var spatialApp = new spatial_web_appEntities())
            {
                var seeffSuburbs = (from seeffArea in spatialApp.seeff_areas
                                   where seeffArea.fkAreaTypeId == 6
                                   select new SeeffSuburb
                                   {
                                       SeeffAreaID = seeffArea.areaId,
                                       AreaName = seeffArea.areaName
                                   }).ToList();

                var mappedSuburbs = from sub in seeffSuburbs
                                    join area in GlobalAreaCache.Instance.AllSuburbs on sub.SeeffAreaID equals area.SeeffAreaID
                                    select sub;

                var unmappedSuburbs = seeffSuburbs.Except(mappedSuburbs);
                return new UnmappedSuburbs
                {
                    Successful = true,
                    Suburbs = unmappedSuburbs.OrderBy(sub => sub.AreaName).ToList()
                };
            }
        }
    }
}