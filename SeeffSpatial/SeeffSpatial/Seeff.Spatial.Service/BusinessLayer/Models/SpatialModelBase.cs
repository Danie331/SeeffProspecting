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
        private DbGeography _polygon;

        private string _polygonWKT;

        [JsonIgnore]
        public DbGeography Polygon
        {
            get { return _polygon; }
            set
            {
                _polygon = value;
                _polygonWKT = value.WellKnownValue.WellKnownText;
            }
        }

        public string PolyWKT
        {
            get { return _polygonWKT; }
            set
            {
                _polygon = CreateGeographyFromStringObject(value);
                _polygonWKT = _polygon.WellKnownValue.WellKnownText;
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
