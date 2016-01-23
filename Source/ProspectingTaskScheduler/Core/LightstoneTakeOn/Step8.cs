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
        public static void FlagNewRegistrations(StringBuilder reportBuilder)
        {
            try
            {
                using (var lsBase = new ls_baseEntities())
                {
                    var connection = lsBase.Database.Connection as SqlConnection;
                    connection.Open();

                    string cmdText = @";WITH reg_dates 
                                 ( 
                                      property_id, 
                                      new_reg_date, 
                                      rn 
                                 ) 
                                 AS 
                                 ( 
                                          SELECT   pp.lightstone_property_id, 
                                                   bd.iregdate, 
                                                   row_number() OVER (partition BY bd.property_id ORDER BY cast(bd.iregdate AS date) DESC) AS rn
                                          FROM     dbo.base_data bd 
                                          JOIN     seeff_prospecting.dbo.prospecting_property pp 
                                          ON       pp.lightstone_property_id = bd.property_id 
                                          WHERE    isdate(bd.iregdate) = 1 
                                          AND      (( 
                                                                     isdate(pp.lightstone_reg_date) = 1 
                                                            AND      cast(bd.iregdate AS date) > cast(pp.lightstone_reg_date AS date))
                                          OR       pp.lightstone_reg_date = '')
                                 )
	                             UPDATE pp
	                             SET pp.latest_reg_date = rg.new_reg_date
	                             FROM seeff_prospecting.dbo.prospecting_property pp JOIN reg_dates rg on rg.property_id = pp.lightstone_property_id
	                             WHERE rg.rn = 1";
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    cmd.CommandTimeout = 60 * 5;
                    int numRowsAffected = cmd.ExecuteNonQuery();

                    reportBuilder.AppendLine("<br /> Successfully updated Prospecting with new registration info. Number of new registrations: " + numRowsAffected);
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step8 aborting...");
                throw;
            }
        }
    }
}