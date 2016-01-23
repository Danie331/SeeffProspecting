using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    public partial class LightstoneTakeOn
    {
        public static void UpdateAreaFatingTable(StringBuilder reportBuilder)
        {
            try
            {
                using (var baseData = new ls_baseEntities())
                {
                    var lsBaseConnection = baseData.Database.Connection as SqlConnection;
                    lsBaseConnection.Open();

                    int fromYear = DateTime.Now.Year - 4;
                    string cmdText = @"EXEC dbo.populate_area_fating " + fromYear;
                    SqlCommand cmd = new SqlCommand(cmdText, lsBaseConnection);
                    cmd.CommandTimeout = 60 * 5;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step7 aborting...");
                throw;
            }
        }
    }
}