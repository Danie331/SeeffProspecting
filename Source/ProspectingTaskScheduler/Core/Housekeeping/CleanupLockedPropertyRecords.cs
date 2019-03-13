using Hangfire;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Housekeeping
{
    public class CleanupLockedPropertyRecords
    {
        [AutomaticRetry(Attempts = 0)]
        public static void ResetYesterdaysLockedRecords(IJobCancellationToken cancellationToken)
        {
            using (var prospecting = new seeff_prospectingEntities())
            {
                try
                {
                    // Select all of yesterdays locked records
                    var yesterday = DateTime.Today.AddDays(-1);
                    var yesterdaysRecords = prospecting.prospecting_property
                                        .Where(pp => pp.locked_datetime != null &&
                                                        DbFunctions.TruncateTime(pp.locked_datetime) <= DbFunctions.TruncateTime(yesterday));
                    foreach (var record in yesterdaysRecords)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        record.locked_by_guid = null;
                        record.locked_datetime = null;
                    }

                    prospecting.SaveChanges();
                }
                catch (OperationCanceledException)
                {
                    // Suppress and return as the job will be retried during its next scheduled run.
                    return;
                }
                catch (Exception ex)
                {
                    StatusNotifier.SendEmail("danie.vdm@seeff.com", "Task Scheduler", "reports@seeff.com", null, "Exception in ResetYesterdaysLockedRecords()", ex.ToString());
                }
            }
        }
    }
}