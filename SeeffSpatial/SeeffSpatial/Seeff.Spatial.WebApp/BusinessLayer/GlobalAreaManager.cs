using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.BusinessLayer
{
    /// <summary>
    /// Thread-safe singleton
    /// Common shared cache across all user sessions through which data is read and written, with database being the destination for writing.
    /// Use this class to manage all area state information directly. 
    /// Remember to reload database before go-live
    /// 
    /// As you scroll - load every polygon in the whole country.
    /// Layers - Suburbs, Licenses, Territories.
    /// Mouse-over highlighting areas.
    /// Creating new polygons.
    /// Test this class for concurrency across multiple user sessions.
    /// Check if this class is shared concurrently among multiple sessions
    /// Get prospecting + MS to use the service only.
    /// Re-area event fires when changes made - affects downstream systems.
    /// Exception logging on both projects!
    /// Creating areas, validating areas, checking session conflicts within global cache.
    /// </summary>
    public sealed class GlobalAreaManager
    {        
        private static readonly Lazy<GlobalAreaManager> _instance = new Lazy<GlobalAreaManager>(() => new GlobalAreaManager());
        private GlobalAreaManager()
        {
            Bootstrap();
        }

        // Private instance fields
        private AreaHierarchy _areaHierarchyCache = null;

        private List<SeeffTerritory> _allTerritories = null;
        private List<SeeffLicense> _allLicenses = null;
        private List<SeeffSuburb> _allSuburbs =null;

        public List<SeeffSuburb> AllSuburbs
        {
            get { return _allSuburbs; }
        }

            public static GlobalAreaManager Instance { get { return _instance.Value; } }

        /// <summary>
        /// This method loads the entire spatial database into this instance.
        /// We only need to load it once, therefore a check is done
        /// </summary>
        private void Bootstrap()
        {
            if (_areaHierarchyCache == null)
            {
                ServiceManager service = new ServiceManager();
                var suburbs = service.RetrieveAllSuburbs();
                var licenses = service.RetrieveAllLicenses();
                var territories = service.RetrieveAllTerritories();

                _allTerritories = territories;
                _allLicenses = licenses;
                _allSuburbs = suburbs.OrderBy(sub => sub.AreaName).ToList();

                _areaHierarchyCache = new AreaHierarchy();
                _areaHierarchyCache.Territories = territories;
                foreach (var territory in _areaHierarchyCache.Territories)
                {
                    var targetLicenses = licenses.Where(lic => lic.TerritoryID == territory.TerritoryID).ToList();
                    territory.Licenses = targetLicenses;
		           foreach (var license in territory.Licenses)
	                {
                       var targetSuburbs = suburbs.Where(sub => sub.LicenseID == license.LicenseID).ToList();
                       license.Suburbs = targetSuburbs;
                   }
                }
            }
        }

        public void Invalidate() 
        {

        }
    }
}