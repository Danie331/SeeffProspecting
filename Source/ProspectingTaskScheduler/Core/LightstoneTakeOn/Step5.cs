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
        public static void UpdateSeeffDeals(StringBuilder reportBuilder)
        {
            try
            {
                using (var baseData = new ls_baseEntities())
                {
                    var lsBaseConnection = baseData.Database.Connection as SqlConnection;
                    lsBaseConnection.Open();

                    string cmdText = @"UPDATE bd
                                    SET bd.agency_id = 1, bd.market_share_type = deeds.division, bd.fated = 1 
                                    FROM base_data bd JOIN [Seeff_Deeds].[dbo].[SEEFF_Deeds_Monthly] deeds on bd.unique_id = deeds.unique_id
                                    WHERE deeds.seeff_deal = 1 AND deeds.is_for_insert = 1";
                    SqlCommand cmd = new SqlCommand(cmdText, lsBaseConnection);
                    cmd.CommandTimeout = 60 * 5;
                    int numRowsAffected = cmd.ExecuteNonQuery();

                    reportBuilder.AppendLine("<br /> Successfully updated seeff_deal in base_data. Number of rows affected: " + numRowsAffected);
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step5 aborting...");
                throw;
            }
        }
    }
}