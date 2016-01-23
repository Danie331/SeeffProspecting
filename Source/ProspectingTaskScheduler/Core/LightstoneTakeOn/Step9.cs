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
                using (var seeffDeeds = new Seeff_DeedsEntities())
                {
                    var seeffDeedsConnection = seeffDeeds.Database.Connection as SqlConnection;
                    seeffDeedsConnection.Open();

                    string cmdText = @"use [master]; DROP DATABASE Seeff_Deeds;";
                    SqlCommand cmd = new SqlCommand(cmdText, seeffDeedsConnection);
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