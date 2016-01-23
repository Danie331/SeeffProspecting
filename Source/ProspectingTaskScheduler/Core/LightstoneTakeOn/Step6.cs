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
        public static void ApplyAdditionalRules(StringBuilder reportBuilder)
        {
            try
            {
                using (var baseData = new ls_baseEntities())
                {
                    var lsBaseConnection = baseData.Database.Connection as SqlConnection;
                    lsBaseConnection.Open();

                    string cmdText = @"UPDATE bd									
	                                SET bd.fated = 1, bd.market_share_type = 'O'
									FROM base_data bd JOIN [Seeff_Deeds].[dbo].[SEEFF_Deeds_Monthly] deeds ON bd.unique_id = deeds.unique_id
	                                WHERE  (deeds.purch_price IS NULL OR deeds.purch_price < 250000) AND deeds.is_for_insert = 1";
                    SqlCommand cmd = new SqlCommand(cmdText, lsBaseConnection);
                    cmd.CommandTimeout = 60 * 5;
                    int affectedCount = cmd.ExecuteNonQuery();

                    cmd.CommandText = @";WITH multiple_props (title_deed, price) AS
	                                (
	                                SELECT title_deed_no, purch_price FROM base_data
	                                WHERE purch_price IS NOT NULL AND seeff_area_id > -1
	                                GROUP BY title_deed_no, purch_price
	                                HAVING COUNT(*) > 1
	                                )
	                                UPDATE base_data 
	                                SET sale_includes_others_flag = 1
	                                FROM multiple_props WHERE title_deed_no = title_deed AND purch_price = price;";
                    affectedCount = cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step6 aborting...");
                throw;
            }
        }
    }
}