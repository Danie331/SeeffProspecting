using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core
{
    public class Utils
    {
        public static void LogException(Exception ex)
        {
            using (var prospectingDb = new ProspectingDataContext())
            {
                var errorRec = new exception_log
                {
                    friendly_error_msg = ex.Message,
                    exception_string = ex.ToString(),
                    user = new Guid(),
                    date_time = DateTime.Now
                };
                prospectingDb.exception_logs.InsertOnSubmit(errorRec);
                prospectingDb.SubmitChanges();
            }
        }
    }
}