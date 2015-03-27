using SeeffSpatialApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace SeeffSpatialApi.Controllers
{
    /// <summary>
    /// Remember to add authorization and logging.
    /// </summary>
    public class SeeffSpatialLookupController : ApiController
    {        
        [HttpGet]
        public int? GetAreaId(double lat, double lng)
        {
            return InternalGetAreaId(lat, lng);
        }

        [HttpPost]
        public int? GetAreaId([FromBody]SpatialLatLng input)
        {
            return InternalGetAreaId(input.Lat, input.Lng);
        }

        [HttpGet]
        public int? GetLicenseId(double lat, double lng)
        {
            return InternalGetLicenseId(lat, lng);
        }

        [HttpPost]
        public int? GetLicenseId([FromBody]SpatialLatLng input)
        {
            return InternalGetLicenseId(input.Lat, input.Lng);
        }

        [HttpPost]
        public AreaLicenseId GetAreaIdAndLicenseId([FromBody]SpatialLatLng input)
        {
            Task<int?> taskGetAreaId = new Task<int?>(() => InternalGetAreaId(input.Lat, input.Lng));
            taskGetAreaId.Start();
            Task<int?> taskGetLicenseId = new Task<int?>(() => InternalGetLicenseId(input.Lat, input.Lng));
            taskGetLicenseId.Start();

            Task.WaitAll(taskGetAreaId, taskGetLicenseId);
            return new AreaLicenseId
            {
                AreaId = taskGetAreaId.Result,
                LicenseId = taskGetLicenseId.Result
            };
        }

        private int? InternalGetLicenseId(double lat, double lng)
        {
            try
            {
                using (var database = new SeeffSpatialDbDataContext())
                {
                    return database.get_license_id(lat, lng);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private int? InternalGetAreaId(double lat, double lng)
        {
            try
            {
                using (var database = new SeeffSpatialDbDataContext())
                {
                    return database.get_area_id(lat, lng);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
