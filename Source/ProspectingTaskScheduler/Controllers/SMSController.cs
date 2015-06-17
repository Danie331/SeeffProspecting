using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProspectingTaskScheduler.Controllers
{
    public class SMSController : ApiController
    {
        [HttpGet]
        public void ProcessSmsReply(string senderid, string userid, string Fullsms, string timestamp)
        {
            string a = "senderid=" + senderid + Environment.NewLine +
                "userid=" + userid + Environment.NewLine +
                "Fullsms=" + Fullsms + Environment.NewLine +
                "timestamp=" + timestamp;
            using (var prospecting = new ProspectingDataContext())
            {
                exception_log entry = new exception_log
                {
                     date_time = DateTime.Now,
                      exception_string = "",
                       friendly_error_msg = a,
                        user = Guid.NewGuid()
                };
                prospecting.exception_logs.InsertOnSubmit(entry);
                prospecting.SubmitChanges();
            }
        }

        [HttpGet]
        public void ProcessSmsReply(string[] obj)
        {
            string a = "";
            obj.ToList().ForEach(s => a = a + s + Environment.NewLine);
            using (var prospecting = new ProspectingDataContext())
            {
                exception_log entry = new exception_log
                {
                    date_time = DateTime.Now,
                    exception_string = "",
                    friendly_error_msg = a,
                    user = Guid.NewGuid()
                };
                prospecting.exception_logs.InsertOnSubmit(entry);
                prospecting.SubmitChanges();
            }
        }
    }
}
