using Hangfire;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ProspectingTaskScheduler.Core.Spatial
{
    public class SuburbMaintenance
    {
        [AutomaticRetry(Attempts = 0)]
        public static void ReindexSuburbsRequiringMaintenance()
        {
            try
            {
                using (var spatial = new seeff_spatialEntities())
                {
                    string connStr = WebConfigurationManager.ConnectionStrings["seeff_spatialEntities"].ConnectionString;
                    var efConn = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(connStr);
                    connStr = efConn.ProviderConnectionString;
                    using (var conn = new SqlConnection(connStr))
                    using (var command = new SqlCommand("reindex_seeff_suburb", conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    })
                    {
                        command.CommandTimeout = int.MaxValue;
                        conn.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        //[AutomaticRetry(Attempts=1)]
        //public static void SynchroniseSuburbNames()
        //{
        //    try
        //    {
        //        Utils.LogException(new Exception("<<< Starting SynchroniseSuburbNames() >>>", null));
        //        using (var spatial = new seeff_spatialEntities())
        //        {
        //            var spatialConnection = spatial.Database.Connection as SqlConnection;
        //            spatialConnection.Open();

        //            SqlCommand cmd = new SqlCommand(@"update sa
        //                                            set sa.area_name = seeff.areaName
        //                                            from seeff_spatial.dbo.spatial_area sa
        //                                            join [41.222.226.215].seeff.dbo.area seeff on sa.fkAreaId = seeff.areaId", spatialConnection);
        //            cmd.CommandTimeout = int.MaxValue;

        //            cmd.ExecuteNonQuery();
        //            Utils.LogException(new Exception("<<< Finished SynchroniseSuburbNames() >>>", null));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Utils.LogException(ex);
        //        // Fire email here.
        //        Utils.LogException(new Exception("<<< Error in SynchroniseSuburbNames() >>>", ex));
        //    }
        //}
    }
}