using Microsoft.SqlServer.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.BusinessLayer.Models
{
    public abstract class SpatialModelBase
    {
        [JsonIgnore]
        public DbGeography Centroid { get; set; }

        [JsonIgnore]
        public DbGeography Polygon { get; set; }

        public string PolyWKT { get; set; }

        public string CentroidWKT { get; set; }

        public void ConvertSpatialToWKT()
        {
            if (Polygon != null)
            {
                PolyWKT = Polygon.WellKnownValue.WellKnownText;
            }

            if (Centroid != null)
            {
                CentroidWKT = Centroid.WellKnownValue.WellKnownText;
            }
        }

        public void ConvertWktToSpatial()
        {
            if (!string.IsNullOrEmpty(PolyWKT))
            {
                Polygon = CreateGeographyFromStringObject(PolyWKT);
            }

            if (!string.IsNullOrEmpty(CentroidWKT))
            {
                Centroid = CreateGeographyFromStringObject(CentroidWKT);
            }
        }

        private DbGeography CreateGeographyFromStringObject(string polyString)
        {
            if (string.IsNullOrEmpty(polyString))
                return null;

            SqlGeography geog = null;
            if (polyString.StartsWith("POLYGON"))
            {
                string[] coordinateSets = polyString.Replace("POLYGON ((", "").Replace("))", "").Split(new[] { ',' });
                string set1 = coordinateSets[0].Trim();
                string set2 = coordinateSets[coordinateSets.Length - 1].Trim();
                if (set1 != set2)
                {
                    var sets = coordinateSets.ToList();
                    sets.Add(set1);
                    string output = sets.AsEnumerable().Aggregate((s1, s2) => s1 + "," + s2);
                    output = "POLYGON ((" + output + "))";

                    polyString = output;
                }
                geog = SqlGeography.STPolyFromText(new System.Data.SqlTypes.SqlChars(polyString), DbGeography.DefaultCoordinateSystemId);
            }

            if (polyString.StartsWith("POINT"))
            {
                geog = SqlGeography.STPointFromText(new System.Data.SqlTypes.SqlChars(polyString), DbGeography.DefaultCoordinateSystemId);
            }

            if (geog == null) return null;

            geog = geog.MakeValid();
            if (geog.EnvelopeAngle() >= 180.0)
            {
                geog = geog.ReorientObject();
            }
            string wellKnownText = geog.ToString();
            polyString = wellKnownText;
            DbGeography polygon = DbGeography.FromText(polyString);

            string wkt = polygon.WellKnownValue.WellKnownText;
            if (!wkt.StartsWith("POLYGON") && !wkt.StartsWith("POINT")) return null;

            return polygon;
        }
    }
}