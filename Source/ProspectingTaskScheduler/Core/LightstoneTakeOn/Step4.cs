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
        public static void InsertRecords(StringBuilder reportBuilder)
        {
            try
            {
                using (var seeffDeeds = new Seeff_DeedsEntities())
                using (var baseData = new ls_baseEntities())
                {
                    var seeffDeedsConnection = seeffDeeds.Database.Connection as SqlConnection;
                    var lsBaseConnection = baseData.Database.Connection as SqlConnection;

                    string commandText = @"INSERT INTO base_data (
                                       [ls_area_id]
                                      ,[seeff_lic_area]
                                      ,[property_id]
                                      ,[iregdate]
                                      ,[ipurchdate]
                                      ,[title_deed_no]
                                      ,[non_gar_props_on_title]
                                      ,[purch_price]
                                      ,[erf_key]
                                      ,[ea_code]
                                      ,[suburb]
                                      ,[munic_name]
                                      ,[province]
                                      ,[property_type]
                                      ,[ss_fh]
                                      ,[property_currently_in_pvt_hands]
                                      ,[private_registration]
                                      ,[bonded_transfer]
                                      ,[suburb_id]
                                      ,[erf_size]
                                      ,[buyer_name]
                                      ,[seller_name]
                                      ,[property_address]
                                      ,[street_or_unit_no]
                                      ,[ls_resflg]
                                      ,[estate_name]
                                      ,[buyer_name_2]
                                      ,[multibuyer]
                                      ,[seeff_suburb]
                                      ,[seeff_suburb_id]
                                      ,[seeff_suburb_type]
                                      ,[seeff_lic_id]
                                      ,[fk_territory_id]
	                                  ,[unique_id]
	                                  ,[seeff_deal]
	                                  ,[seeff_area_id]
                                      ,[x]
                                      ,[y]
	                                  ,[erf_no]
	                                  ,[portion_no]) 
	                                SELECT [ls_area_id]
                                      ,[seeff_lic_area]
                                      ,[property_id]
                                      ,[iregdate]
                                      ,[ipurchdate]
                                      ,[title_deed_no]
                                      ,[non_gar_props_on_title]
                                      ,[purch_price]
                                      ,[erf_key]
                                      ,[ea_code]
                                      ,[suburb]
                                      ,[munic_name]
                                      ,[province]
                                      ,[property_type]
                                      ,[ss_fh]
                                      ,[property_currently_in_pvt_hands]
                                      ,[private_registration]
                                      ,[bonded_transfer]
                                      ,[suburb_id]
                                      ,[erf_size]
                                      ,[buyer_name]
                                      ,[seller_name]
                                      ,[property_address]
	                                  ,[street_or_unit_no]
                                      ,[ls_resflg]
                                      ,[estate_name]
                                      ,[buyer_name_2]
                                      ,[multibuyer]
                                      ,[seeff_suburb]
                                      ,[seeff_suburb_id]
                                      ,[seeff_suburb_type]
                                      ,[seeff_lic_id]
                                      ,[territory_id]
	                                  ,[unique_id]
	                                  ,[seeff_deal]
	                                  ,[seeff_area_Id]
                                      ,[x]
                                      ,[y]
	                                  ,[erf_no]
	                                  ,[portion] 
	                                 FROM Seeff_Deeds.dbo.SEEFF_Deeds_Monthly
                                     WHERE is_for_insert = 1;";
                    SqlCommand cmd = new SqlCommand(commandText, lsBaseConnection);
                    cmd.CommandTimeout = 60 * 5;
                    lsBaseConnection.Open();
                    int numRowsInserted = cmd.ExecuteNonQuery();

                    reportBuilder.AppendLine("<br /> Take-on rows successfully inserted into base_data. Total rows inserted: " + numRowsInserted);
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step4 aborting...");
                throw;
            }
        }
    }
}