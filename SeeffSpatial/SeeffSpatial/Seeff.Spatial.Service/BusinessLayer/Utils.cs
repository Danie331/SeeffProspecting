using Microsoft.SqlServer.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seeff.Spatial.Service.BusinessLayer.Models;
using Seeff.Spatial.Service.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Seeff.Spatial.Service.BusinessLayer
{
    public class Utils
    {
        public static void LogException(Exception ex, string context, object contextObject)
        {
            using (var spatialDb = new seeff_spatialEntities())
            {
                exception_log newRec = new exception_log
                {
                    context = context,
                    created = DateTime.Now,
                    exception_friendly_msg = ex.Message,
                    exception_string = ex.ToString()
                };

                spatialDb.exception_log.Add(newRec);
                spatialDb.SaveChanges();
            }
        }

        public class CustomBodyModelValidator : System.Web.Http.Validation.DefaultBodyModelValidator
        {
            public override bool ShouldValidateType(Type type)
            {
                return type != typeof(DbGeography) && base.ShouldValidateType(type);
            }
        }
    }
}