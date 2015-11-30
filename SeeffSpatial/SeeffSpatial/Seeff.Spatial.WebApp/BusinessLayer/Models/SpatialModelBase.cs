using Microsoft.SqlServer.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer.Models
{
    public abstract class SpatialModelBase
    {
        [JsonIgnore]
        private DbGeography _polygon;
        [JsonIgnore]
        private DbGeography _centroid;

        private string _polygonWKT;
        private string _centroidWKT;

        [JsonIgnore]
        public DbGeography Centroid
        {
            get { return _centroid;  } 
        }

        [JsonIgnore]
        public DbGeography Polygon 
        {
            get { return _polygon;  }
            set 
            {
                _polygon = value;
                if (value != null)
                {
                    _polygonWKT = value.WellKnownValue.WellKnownText;
                    CalculateCentroid();
                }
            }
        }

        public string PolyWKT 
        {
            get { return _polygonWKT;  }
            set
            {
                _polygon = CreateGeographyFromStringObject(value);
                if (_polygon != null)
                {
                    _polygonWKT = _polygon.WellKnownValue.WellKnownText;
                    CalculateCentroid();
                }
            }
        }

        public string CentroidWKT { get { return _centroidWKT; } }

        public abstract int? PolyID { get; }

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

        private void CalculateCentroid()
        {
            if (_polygon == null) return;
            string wkt = _polygon.WellKnownValue.WellKnownText;
            var geog = SqlGeography.STPolyFromText(new System.Data.SqlTypes.SqlChars(wkt), DbGeography.DefaultCoordinateSystemId);
            geog = geog.EnvelopeCenter();
            geog = geog.MakeValid();
            wkt = geog.ToString();

            _centroid = DbGeography.FromText(wkt);
            _centroidWKT = _centroid.WellKnownValue.WellKnownText;
        }

        public List<T> GetIntersectingPolys<T>(List<T> polys)
            where T: SpatialModelBase
        {   
            List<T> results = new List<T>();
            if (polys == null || this.Polygon == null)
                return results;

            foreach (var poly in polys)
            {
                if (poly.Polygon == null)
                    continue;
                if (poly.Polygon.Intersects(this.Polygon) && poly.PolyID != this.PolyID)
                {
                    results.Add(poly);
                }
            }
            return results;
        }

        public List<T> GetPolysContainingCentroid<T>(List<T> polys)
            where T: SpatialModelBase
        {
            List<T> results = new List<T>();
            if (this.Centroid == null)
                return results;

            foreach (var poly in polys)
            {
                if (poly.Polygon == null)
                    continue;
                if (this.Centroid.Intersects(poly.Polygon))
                {
                    results.Add(poly);
                }
            }
            return results;
        }

        public List<T> GetContainingPolys<T>(List<T> polys)
            where T:  SpatialModelBase
        {
            List<T> results = new List<T>();
            if (polys == null || this.Polygon == null || this.Centroid == null)
                return results;

            List<T> polysContainingCentroid = GetPolysContainingCentroid(polys);
            foreach (var poly in polysContainingCentroid)
            {
                DbGeometry containerTest = DbGeometry.PolygonFromText(poly.Polygon.WellKnownValue.WellKnownText, DbGeography.DefaultCoordinateSystemId);
                DbGeometry containedTest = DbGeometry.PolygonFromText(this.Polygon.WellKnownValue.WellKnownText, DbGeography.DefaultCoordinateSystemId);
                bool contained = containedTest.Within(containerTest);
                if (contained)
                {
                    results.Add(poly);
                }
            }
            return results;
        }

        public List<T> GetIntersectingPolysIgnoreTouching<T>(List<T> polys)
            where T: SpatialModelBase
        {
            List<T> results = new List<T>();
            if (polys == null || this.Polygon == null)
                return results;

            foreach (var poly in polys)
            {
                if (poly.Polygon == null)
                    continue;
                var geomToTest = DbGeometry.PolygonFromText(poly.Polygon.WellKnownValue.WellKnownText, DbGeography.DefaultCoordinateSystemId);
                var sourceGeom = DbGeometry.PolygonFromText(this.Polygon.WellKnownValue.WellKnownText, DbGeography.DefaultCoordinateSystemId);
                if (sourceGeom.Intersects(geomToTest)) 
                //if (poly.Polygon..Intersects(this.Polygon) && poly.PolyID != this.PolyID)
                {
                    results.Add(poly);
                }
            }
            return results;
        }
    }
}