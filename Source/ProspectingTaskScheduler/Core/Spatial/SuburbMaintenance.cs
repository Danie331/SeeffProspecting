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
        public static void ReindexSuburbsRequiringMaintenance()
        {
            try {
                using (var spatial = new seeff_spatialEntities())
                {
                    string connStr = WebConfigurationManager.ConnectionStrings["seeff_spatial"].ConnectionString;
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
    }
}