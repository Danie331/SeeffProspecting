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

                using (var bd  = new ls_baseEntities())
                {
                    var lsBaseConnection = bd.Database.Connection as SqlConnection;
                    lsBaseConnection.Open();

                    string cmd = @"update bd
                                    set bd.seeff_deal = 1,
	                                    bd.fated = 1,
                                        bd.agency_id = 1, 
	                                    bd.market_share_type = (case tr.sps_transaction_division
								                                    when 'Residential' then 'R'
								                                    when 'Agriculture' then 'A'
								                                    when 'Commercial' then 'C'
								                                    when 'Development' then 'D'
								                                    end)
	                                    from
                                    ls_base.dbo.base_data bd
                                    join  boss.dbo.sps_transaction tr on bd.property_id = tr.sps_property_id
                                         and bd.iregdate = tr.sps_reg_date
	                                     and bd.iregdate = tr.lightstone_reg_date
	                                     and bd.unique_id = tr.unique_id
                                    where 
                                     tr.sps_cancelled = 0 
                                    and tr.sps_transaction_type = 'Sale'
                                    and tr.sps_refferal_type != 'External paid to you'
                                    and tr.branch_id != 260
                                    and tr.branch_id != 263
                                    and tr.sps_property_id > 0
                                    and tr.sps_reporting_date >= '2012-01-01'
                                    and bd.seeff_deal is null";

                    SqlCommand sqlcmd = new SqlCommand(cmd, lsBaseConnection);
                    sqlcmd.CommandTimeout = 60 * 5;
                    int numRowsAffected = sqlcmd.ExecuteNonQuery();

                    reportBuilder.AppendLine("<br /> Successfully synchronised seeff_deals in base_data with boss.dbo.sps_transaction. Number of rows affected: " + numRowsAffected);
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