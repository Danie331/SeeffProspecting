using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    public partial class LightstoneTakeOn
    {
        public static bool SeeffDeedsExists(StringBuilder reportBuilder)
        {
            try
            {
                string connStr = WebConfigurationManager.ConnectionStrings["seeff_prospectingEntities"].ConnectionString;
                var efConn = new EntityConnectionStringBuilder(connStr);
                connStr = efConn.ProviderConnectionString;
                using (var connection = new SqlConnection(connStr))
                {
                    using (var command = new SqlCommand(string.Format("SELECT db_id('{0}')", "Seeff_Deeds"), connection))
                    {
                        connection.Open();
                        return (command.ExecuteScalar() != DBNull.Value);
                    }
                }
            }
            catch
            {
                reportBuilder.AppendLine("<br /> Error occurred in Step1 (establishing whether Seeff_Deeds exists)");
                throw;
            }
        }
    }
}