using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    public partial class LightstoneTakeOn
    {
        public static void FlagRecordsForInsert(StringBuilder reportBuilder)
        {
            try
            {
                using (var seeffDeeds = new Seeff_DeedsEntities())
                {
                    var connection = seeffDeeds.Database.Connection as SqlConnection;
                    // Augment the Seeff_Deeds_monthly table with columns necessary to process the rows
                    var command = new SqlCommand(@";ALTER TABLE SEEFF_Deeds_Monthly
                                                   ADD [is_for_insert] bit default(0) NOT NULL,
                                                     [seeff_area_id] int null,
                                                     [territory_id] int null,
	                                                 [unique_id] nvarchar(50) null,
	                                                 [seeff_deal] bit null,
                                                     [division] varchar(1) null,
	                                                 [property_address] varchar(255) null,
	                                                 [street_or_unit_no] varchar(255) null,
	                                                 [erf_no] int null,
	                                                 [portion_no] int null;", connection);

                    command.CommandTimeout = 60 * 10;
                    command.Connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        reportBuilder.AppendLine("<br />NON-FATAL: Cannot augment the Seeff_Deeds_monthly table with columns necessary to process the rows");
                        //throw;
                    }

                    // Populate the unique_id column
                    command.CommandText = @"UPDATE SEEFF_Deeds_Monthly
                                               SET unique_id = concat(cast(property_id as nvarchar), cast(iregdate as nvarchar));";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        reportBuilder.AppendLine("<br /> Error occurred in Step2 at: Populate the unique_id column");
                        throw;
                    }

                    // Ensure that each take-on row has a valid unique_id
                    command.CommandText = @"ALTER TABLE [SEEFF_Deeds_Monthly] ALTER COLUMN [unique_id]  nvarchar(50) NOT NULL";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        reportBuilder.AppendLine("<br />NON-FATAL: Error occurred in Step2 at: Ensure that each take-on row has a valid unique_id");
                        //throw;
                    }

                    // Flag records for insert
                    command.CommandText = @"MERGE SEEFF_Deeds_Monthly AS target
                                               USING (SELECT unique_id FROM ls_base.dbo.base_data) AS source (unique_id)
                                               ON (target.unique_id = source.unique_id)
                                               WHEN NOT MATCHED BY SOURCE THEN
		                                       UPDATE SET [is_for_insert] = 1;";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        reportBuilder.AppendLine("<br /> Error occurred in Step2 at: Flag records for insert");
                        throw;
                    }
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step2 aborting...");
                throw;
            }
        }
    }
}