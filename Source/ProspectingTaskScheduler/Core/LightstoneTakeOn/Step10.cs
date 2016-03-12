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
        public static void AutoFateDevelopments(StringBuilder reportBuilder)
        {
            try
            {
                using (var baseData = new ls_baseEntities())
                {
                    var lsBaseConnection = baseData.Database.Connection as SqlConnection;
                    lsBaseConnection.Open();

                    string commandText = @"update bd
                                            set bd.fated = 1, bd.market_share_type = 'D'
                                            from ls_base.dbo.base_data bd
                                            where bd.seller_name in (select fk_seller_name from ls_base.dbo.developers)";

                    SqlCommand cmd = new SqlCommand(commandText, lsBaseConnection);
                    cmd.CommandTimeout = 300;
                    int numRowsAffetced  = cmd.ExecuteNonQuery();

                    reportBuilder.AppendLine("<br /> Auto-fating development, " + numRowsAffetced + " rows updated.");
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step 10 aborting...");
                throw;
            }
        }
    }
}