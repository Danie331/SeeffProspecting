using Seeff.Spatial.WebApp.BusinessLayer;
using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.WebApp.ServiceInterface
{
    public class SpatialDataWriter: ServiceBase
    {
        public SeeffSuburb SaveSuburb(SeeffSuburb suburb)
        {
            try
            {
                suburb = PostToService<SeeffSuburb>("api/CreateOrUpdate/SaveSuburb", suburb);
                return suburb;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "SaveSuburb()", null);
                throw;
            }
        }

        public SeeffLicense SaveLicense(SeeffLicense license)
        {
            try
            {
                license = PostToService<SeeffLicense>("api/CreateOrUpdate/SaveLicense", license);
                return license;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "SaveLicense()", null);
                throw;
            }
        }

        public SeeffTerritory SaveTerritory(SeeffTerritory territory)
        {
            try
            {
                territory = PostToService<SeeffTerritory>("api/CreateOrUpdate/SaveTerritory", territory);
                return territory;
            }
            catch (Exception ex)
            {
                Utils.LogException(ex, "SaveTerritory()", null);
                throw;
            }
        }

        public SeeffSuburb DeleteSuburb(SeeffSuburb suburb)
        {
            try
            {
                suburb = PostToService<SeeffSuburb>("api/CreateOrUpdate/DeleteSuburb", suburb);
                return suburb;
            }
            catch(Exception ex)
            {
                Utils.LogException(ex, "DeleteSuburb() in SpatialWriter", suburb);
                throw;
            }
        }
    }
}