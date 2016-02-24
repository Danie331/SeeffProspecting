using Seeff.Spatial.WebApp.BusinessLayer.Models;
using Seeff.Spatial.WebApp.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer
{
    /// <summary>
    /// Thread-safe singleton
    /// Common shared cache across all user sessions through which data is read and written, with database being the destination for writing.
    /// </summary>
    public sealed class GlobalAreaCache
    {        
        private static readonly Lazy<GlobalAreaCache> _instance = new Lazy<GlobalAreaCache>(() => new GlobalAreaCache());
        // Private instance fields
        private ObjectCache _cache;

        private GlobalAreaCache()
        {
            _cache = MemoryCache.Default;
            Bootstrap();
        }

        private List<SeeffTerritory> _allTerritories = null;
        private List<SeeffLicense> _allLicenses = null;
        private List<SeeffSuburb> _allSuburbs =null;

        public List<SeeffSuburb> AllSuburbs
        {
            get {
                if (_allSuburbs == null)
                {
                    SpatialDataReader service = new SpatialDataReader();
                    var suburbs = service.RetrieveAllSuburbs();
                    _allSuburbs = suburbs.OrderBy(sub => sub.AreaName).ToList();

                    foreach (var territory in _allTerritories)
                    {
                        var targetLicenses = _allLicenses.Where(lic => lic.TerritoryID == territory.TerritoryID).ToList();
                        territory.Licenses = targetLicenses;
                        foreach (var license in territory.Licenses)
                        {
                            var targetSuburbs = _allSuburbs.Where(sub => sub.LicenseID == license.LicenseID).ToList();
                            license.Suburbs = targetSuburbs;
                        }
                    }
                }
                return _allSuburbs; 
            }
        }

        public List<SeeffLicense> SeeffLicenses
        {
            get { return _allLicenses; }
        }

        public List<SeeffTerritory> SeeffTerritories
        {
            get { return _allTerritories; }
        }

        public AreaHierarchy AreaHierarchy
        {
            get { return _cache["AreaHierarchyCache"] as AreaHierarchy; }
        }

        public static GlobalAreaCache Instance { get { return _instance.Value; } }

        /// <summary>
        /// This method loads the entire spatial database into this instance.
        /// We only need to load it once, therefore a check is done
        /// </summary>
        private void Bootstrap()
        {
            AreaHierarchy _areaHierarchy = _cache["AreaHierarchyCache"] as AreaHierarchy;
            if (_areaHierarchy == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();

                SpatialDataReader service = new SpatialDataReader();
                //var suburbs = service.RetrieveAllSuburbs();
                var licenses = service.RetrieveAllLicenses();
                var territories = service.RetrieveAllTerritories();

                _allTerritories = territories;
                _allLicenses = licenses.OrderBy(lic => lic.LicenseName).ToList();
                //_allSuburbs = suburbs.OrderBy(sub => sub.AreaName).ToList();

                _areaHierarchy = new AreaHierarchy();
                _areaHierarchy.Territories = territories;
                foreach (var territory in _areaHierarchy.Territories)
                {
                    var targetLicenses = licenses.Where(lic => lic.TerritoryID == territory.TerritoryID).ToList();
                    territory.Licenses = targetLicenses;
                   //foreach (var license in territory.Licenses)
                   // {
                      // var targetSuburbs = suburbs.Where(sub => sub.LicenseID == license.LicenseID).ToList();
                       //license.Suburbs = targetSuburbs;
                   //}
                }

                _cache.Set("AreaHierarchyCache", _areaHierarchy, policy);
            }
        }

        public void UpdateCacheItem(SeeffSuburb result)
        {
            if (!result.SeeffAreaID.HasValue)
            {
                Exception e = new Exception("UpdateCacheItem() failed because input suburb SeeffAreaID is null.");
                Utils.LogException(e, "GlobalAreaCache.UpdateCacheItem()", result);
                throw e;
            }

            var target = _allSuburbs.FirstOrDefault(sub => sub.SeeffAreaID == result.SeeffAreaID);
            if (target != null)
            {
                target.Polygon = result.Polygon;
                target.LicenseID = result.LicenseID;
                target.AreaName = result.AreaName;
                target.TerritoryID = result.TerritoryID;
            }
            else
            {
                _allSuburbs.Add(result);
                _allSuburbs = _allSuburbs.OrderBy(sub => sub.AreaName).ToList();
            }
        }

        public SeeffLicense UpdateCacheItem(SeeffLicense result)
        {
            var target = _allLicenses.FirstOrDefault(lic => lic.LicenseID == result.LicenseID);
            if (target != null)
            {
                target.Polygon = result.Polygon;
                target.LicenseID = result.LicenseID;
                target.TerritoryID = result.TerritoryID;
            }
            else
            {
                _allLicenses.Add(result);
                _allLicenses = _allLicenses.OrderBy(lic => lic.LicenseName).ToList();          
            }

            // Must ensure that any unallocated suburbs whose centroids fall within this license's poly must be incorporated into this license
            var unallocatedSuburbs = from sub in AllSuburbs
                                     where sub.TerritoryID == result.TerritoryID && (sub.LicenseID == null || sub.LicenseID <= 0)
                                     select sub;
            foreach (var sub in unallocatedSuburbs)
            {
                if (sub.Centroid.Intersects(result.Polygon))
                {
                    sub.LicenseID = result.LicenseID;
                }
            }

            result.Suburbs = _allSuburbs.Where(sub => sub.LicenseID == result.LicenseID).ToList();
            return result;
        }

        public void RemoveCacheItem(SeeffSuburb result)
        {
            var target = _allSuburbs.FirstOrDefault(sub => sub.SeeffAreaID == result.SeeffAreaID);
            if (target != null)
            {
                _allSuburbs.Remove(target);
            }
        }
    }
}