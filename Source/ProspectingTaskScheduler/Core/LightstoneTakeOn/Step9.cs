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
        public static void DropSeeffDeeds(StringBuilder reportBuilder)
        {
            try
            {
                using (var lsBase = new ls_baseEntities())
                {
                    var connection = lsBase.Database.Connection as SqlConnection;
                    connection.Open();

                    string cmdText = @"ALTER DATABASE Seeff_Deeds SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                       DROP DATABASE [Seeff_Deeds]";
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    cmd.CommandTimeout = 60 * 5;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step9 aborting...");
                throw;
            }
        }
    }
}