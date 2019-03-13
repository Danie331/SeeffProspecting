using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using System.Configuration;
using ProspectingTaskScheduler.Core.Communication.Emailing;
using ProspectingTaskScheduler.Core.Communication.SMSing;
using ProspectingTaskScheduler.Core.Housekeeping;
using System.Data.Entity.Core.EntityClient;

[assembly: OwinStartup(typeof(ProspectingTaskScheduler.App_Start.Startup))]

namespace ProspectingTaskScheduler.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            string cs = ConfigurationManager.ConnectionStrings["seeff_prospectingEntities"].ConnectionString;
            var efConn = new EntityConnectionStringBuilder(cs);
            string adoConn = efConn.ProviderConnectionString;
            GlobalConfiguration.Configuration.UseSqlServerStorage(adoConn);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
