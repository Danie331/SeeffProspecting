using System;

namespace ProspectingTaskScheduler.Core
{
    public class Utils
    {
        public static void LogException(Exception ex)
        {
            using (var prospectingDb = new seeff_prospectingEntities())
            {
                var errorRec = new exception_log
                {
                    friendly_error_msg = ex != null ? ex.Message : "testing 123",
                    exception_string = ex != null ? ex.ToString() : "testing 123",
                    user = new Guid(),
                    date_time = DateTime.Now
                };
                prospectingDb.exception_log.Add(errorRec);
                prospectingDb.SaveChanges();
            }
        }
    }
}