using Hangfire;
using ProspectingTaskScheduler.Core.Communication.Emailing;
using ProspectingTaskScheduler.Core.Communication.SMSing;
using ProspectingTaskScheduler.Core.Housekeeping;
using ProspectingTaskScheduler.Core.LightstoneTakeOn;
using ProspectingTaskScheduler.Core.Spatial;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using ProspectingTaskScheduler.Core.ClientSynchronisation;

namespace ProspectingTaskScheduler.App_Start
{
    public class HangfireBootstrapper : IRegisteredObject
    {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;

        private BackgroundJobServer _backgroundJobServer;

        private HangfireBootstrapper()
        {
        }

        public void Start()
        {
            lock (_lockObject)
            {
                if (_started) return;
                _started = true;

                HostingEnvironment.RegisterObject(this);

                string cs = ConfigurationManager.ConnectionStrings["seeff_prospectingConnectionString"].ConnectionString;
                GlobalConfiguration.Configuration.UseSqlServerStorage(cs);

                _backgroundJobServer = new BackgroundJobServer();
                EnqueueJobs();
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                if (_backgroundJobServer != null)
                {
                    _backgroundJobServer.Dispose();
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }

        void IRegisteredObject.Stop(bool immediate)
        {
            Stop();
        }

        public static void EnqueueJobs() 
        {

            RecurringJob.AddOrUpdate("Sending Emails", () => EmailHandler.SendEmails(), Cron.Minutely);
            RecurringJob.AddOrUpdate("Sending SMS's", () => SMSHandler.SendSMS(), Cron.Minutely);

            // Job: Query statuses of SMS's sent
            //RecurringJob.AddOrUpdate("Updating delivery statuses", () => SMSHandler.UpdateDeliveryStatuses(), Cron.Minutely); // change to daily.

            // Public URL which they will call with replies

            // Housekeeping tasks
            RecurringJob.AddOrUpdate("Resetting yesterdays locked properties", () => CleanupLockedPropertyRecords.ResetYesterdaysLockedRecords(), Cron.Daily(1));

            RecurringJob.AddOrUpdate("IIS app pool up and running", () => StatusNotifier.SendHealthStatusEmail(), Cron.Daily(7));

            // Spatial tasks
            RecurringJob.AddOrUpdate("Reindexing Suburbs", () => SuburbMaintenance.ReindexSuburbsRequiringMaintenance(), Cron.Minutely);


            RecurringJob.AddOrUpdate("Lightstone base_data take-on", () => LightstoneTakeOn.PerformBaseDataTakeOn(), Cron.Daily(11, 30), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate("Synchronising spatial areas with seeff.com", () => SuburbMaintenance.SynchroniseSuburbNames(), Cron.Daily(6), TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate("Synchronising Prospecting contact information to CMS", () => ProspectingToCmsClientSynchroniser.Synchronise(), Cron.MinuteInterval(30));
        }
    }
}