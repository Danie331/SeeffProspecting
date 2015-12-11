using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ProspectingTaskScheduler.Core.Spatial
{
    public class ProspectingSuburbMaintenance
    {
        public static void ReindexSuburbsRequiringMaintenance()
        {
            using (var spatial = new seeff_spatialEntities())
            {
                var spatialSuburb = spatial.spatial_area.FirstOrDefault(sub => sub.requires_maintenance);
                if (spatialSuburb != null)
                {
                    string connStr = WebConfigurationManager.ConnectionStrings["seeff_spatial"].ConnectionString;
                    using (var conn = new SqlConnection(connStr))
                    using (var command = new SqlCommand("reindex_prospecting_suburb", conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    })
                    {
                        command.CommandTimeout = 1000;
                        command.Parameters.Add(new SqlParameter("@area_id", spatialSuburb.fkAreaId));
                        conn.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}