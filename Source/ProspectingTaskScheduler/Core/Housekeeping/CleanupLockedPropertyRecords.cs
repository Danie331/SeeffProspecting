using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Housekeeping
{
    public class CleanupLockedPropertyRecords
    {
        public static void ResetYesterdaysLockedRecords()
        {
            using (var prospecting = new ProspectingDataContext())
            {
                // Select all of yesterdays locked records
                var yesterday = DateTime.Today.AddDays(-1).Date;
                var yesterdaysRecords = prospecting.prospecting_properties.Where(pp => pp.locked_datetime != null && pp.locked_datetime.Value.Date == yesterday);
                foreach (var record in yesterdaysRecords)
                {
                    record.locked_by_guid = null;
                    record.locked_datetime = null;
                }

                prospecting.SubmitChanges();
            }
        }
    }
}