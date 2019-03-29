using Hangfire;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    public partial class LightstoneTakeOn
    {
        public static void ProcessRecordsForInsert(StringBuilder reportBuilder, IJobCancellationToken cancellationToken)
        {
            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                using (var prospecting = new seeff_prospectingEntities())
                using (var spatial = new seeff_spatialEntities())
                using (var seeffDeeds = new Seeff_DeedsEntities())
                {
                    List<TakeOnRow> takeonRows = new List<TakeOnRow>();
                    var seeffDeedsConnection = seeffDeeds.Database.Connection as SqlConnection;
                    SqlCommand cmd = new SqlCommand(@"SELECT unique_id, property_id, iregdate, Y, X, erf_key FROM SEEFF_Deeds_Monthly
                                                  WHERE is_for_insert = 1 AND seeff_area_id IS NULL", seeffDeedsConnection);

                    seeffDeedsConnection.Open();
                    cmd.CommandTimeout = 60 * 5;
                    // Load take-on rows into a List for processing
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string uniqueID = reader.GetString(0);
                                    int propertyID = reader.GetInt32(1);
                                    string regDate = reader.GetString(2);
                                    string lat = reader.GetDecimal(3).ToString(nfi);
                                    string lng = reader.GetDecimal(4).ToString(nfi);
                                    string erfKey = reader.GetString(5);
                                    takeonRows.Add(new TakeOnRow
                                    {
                                        unique_id = uniqueID,
                                        property_id = propertyID,
                                        iregdate = regDate,
                                        Y = lat,
                                        X = lng,
                                        erf_key = erfKey
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        reportBuilder.AppendLine("<br /> Error occurred in Step3 at: Load take-on rows into a List for processing");
                        throw;
                    }

                    List<int> unregisteredProperties = null;
                    BOSSWebService bossService = null;
                    try
                    {
                        // Retrieve the unregistered properties from BOSS web service
                        bossService = new BOSSWebService();
                        unregisteredProperties = bossService.GetListUnregisteredProperties();
                    }
                    catch
                    {
                        reportBuilder.AppendLine("<br /> Error occurred in Step3 at: Retrieve the unregistered properties from BOSS web service");
                        throw;
                    }

                    var spatialConnection = spatial.Database.Connection as SqlConnection;
                    SqlCommand spatialCmd = new SqlCommand("", spatialConnection);
                    spatialConnection.Open();
                    foreach (var record in takeonRows)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (spatialCmd.Connection.State != ConnectionState.Open)
                            spatialCmd.Connection.Open();
                        if (seeffDeeds.Database.Connection.State != ConnectionState.Open)
                            seeffDeeds.Database.Connection.Open();
                        try
                        {
                            string streetOrUnitNo = "n/a";
                            string streetAddress = "n/a";
                            // try fetch address from prospecting here. update address on take-on, update address on prospecting new/updating prospecting
                            var prospectingTarget = prospecting.prospecting_property.FirstOrDefault(pp => pp.lightstone_property_id == record.property_id);
                            if (prospectingTarget != null)
                            {
                                streetAddress = prospectingTarget.property_address.TrimStart(new[] { ' ', ',', ' ' });
                                streetOrUnitNo = prospectingTarget.street_or_unit_no;
                                if (prospectingTarget.ss_fh == "SS")
                                {
                                    streetAddress = string.Concat(prospectingTarget.ss_name, ", ", streetAddress);
                                }
                            }
                            else
                            {
                                streetAddress = "Prospect to display address details";
                            }

                            var erfPortion = PropertyAddressParser.GetErfAndPortion(record.erf_key);
                            int? erfNo = erfPortion[0];
                            int? portionNo = erfPortion[1];

                            spatialCmd.CommandText = @"SELECT dbo.get_area_id(" + record.Y + "," + record.X + "," + -1 + ")";
                            int seeff_area_id;
                            try {
                                seeff_area_id = (int)spatialCmd.ExecuteScalar();
                            }
                            catch
                            {
                                Thread.Sleep(15000);
                                seeff_area_id = (int)spatialCmd.ExecuteScalar();
                            }

                            spatialCmd.CommandText = @"SELECT dbo.get_license_id(" + record.Y + "," + record.X + ")";
                            int seeff_lic_id;
                            try {
                               seeff_lic_id = (int)spatialCmd.ExecuteScalar();
                            }
                            catch
                            {
                                Thread.Sleep(15000);
                                seeff_lic_id = (int)spatialCmd.ExecuteScalar();
                            }

                            spatialCmd.CommandText = @"SELECT dbo.get_territory_id(" + record.Y + "," + record.X + ")";
                            int territory_id;
                            try {
                                territory_id = (int)spatialCmd.ExecuteScalar();
                            }
                            catch
                            {
                                Thread.Sleep(15000);
                                territory_id = (int)spatialCmd.ExecuteScalar();
                            }

                            bool seeffDeal = false;
                            string division = string.Empty;
                            if (unregisteredProperties.Contains(record.property_id))
                            {
                                TransactionResult tresult = null;
                                try {
                                    tresult = bossService.IsSeeffRegistered(record.property_id, record.iregdate);
                                    if (string.IsNullOrEmpty(tresult.pErrorMessage) && !string.IsNullOrEmpty(tresult.unique_id))
                                    {
                                        seeffDeal = true;
                                        division = tresult.division;
                                    }
                                }
                                catch (Exception e)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine("property_id:" + record.property_id);
                                    sb.AppendLine("<br />");
                                    sb.AppendLine("iregdate:" + record.iregdate);
                                    sb.AppendLine("<br />");
                                    sb.AppendLine(e.ToString());
                                    LightstoneTakeOn.SendEmailReport("Exception occurred while calling IsSeeffRegistered(). Check status of this record....retrying.", sb);
                                    Thread.Sleep(15000);
                                    try
                                    {
                                        tresult = bossService.IsSeeffRegistered(record.property_id, record.iregdate);
                                        if (string.IsNullOrEmpty(tresult.pErrorMessage) && !string.IsNullOrEmpty(tresult.unique_id))
                                        {
                                            seeffDeal = true;
                                            division = tresult.division;
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        string msg = "The following non-fatal error occurred while calling bossService.IsSeeffRegistered(" + record.property_id + ", " + record.iregdate + ") -- error: " + ex.Message;
                                        msg = msg + " -- PLEASE MANUALLY FIX THIS RECORD.";
                                        reportBuilder.AppendLine("<br /> " + msg);
                                    }
                                }
                            }

                            // Update the target record in Seeff_Deeds_Monthly
                            string sqlText = string.Format(@"UPDATE SEEFF_Deeds_Monthly
                                                     SET property_address = '{0}',
                                                         street_or_unit_no = '{1}',
                                                         erf_no = {2},
                                                         portion_no = {3},
                                                         seeff_area_id = {4},
                                                         Seeff_LIC_ID = {5},
                                                         territory_id = {6},
                                                         seeff_deal = {7},
                                                         division = {8}  
                                                         WHERE unique_id = '{9}'",
                                                                 EscapeSqlStringLiteral(streetAddress),
                                                                 EscapeSqlStringLiteral(streetOrUnitNo),
                                                                 erfNo.HasValue ? erfNo.Value.ToString() : "NULL",
                                                                 portionNo.HasValue ? portionNo.Value.ToString() : "NULL",
                                                                 seeff_area_id,
                                                                 seeff_lic_id,
                                                                 territory_id,
                                                                 seeffDeal ? "1" : "NULL",
                                                                 seeffDeal ? "'" + division + "'" : "NULL",
                                                                 record.unique_id);
                            SqlCommand updateTarget = new SqlCommand(sqlText, seeffDeedsConnection);
                            updateTarget.CommandTimeout = 120;
                            updateTarget.ExecuteNonQuery();
                        }
                        catch
                        {
                            reportBuilder.AppendLine("<br /> Error occurred in Step3 whilst processing take-on row objects:")
                                         .AppendLine("<br /> unique_id of offending row: " + record.unique_id);
                            throw;
                        }
                    }
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Step3 aborting...");
                throw;
            }
        }

        private class TakeOnRow
        {            
            public string unique_id { get; set; }
            public int property_id { get; set; }
            public string iregdate { get; set; }
            public string Y { get; set; }
            public string X { get; set; }
            public string erf_key { get; set; }
        }

        private static T TryGetItem<T>(DataTable dt, string propertyName)
            where T : struct
        {
            try
            {
                var dataRow = dt.Rows[0];
                return (T)Convert.ChangeType(dataRow[propertyName], typeof(T));
            }
            catch { }
            return default(T);
        }

        private static string EscapeSqlStringLiteral(string input)
        {
            return input.Replace("'", "''");
        }
    }
}