using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler
{
    /// <summary>
    /// Summary description for TestHandler
    /// </summary>
    public class TestHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var vars = context.Request.QueryString;
            if (vars != null)
            {
                string str = "";
                foreach (var item in vars)
                {
                    str += item + Environment.NewLine;
                }

                using (var p = new ProspectingDataContext())
                {
                    exception_log e = new exception_log
                    {
                         date_time = DateTime.Now,
                          user = Guid.NewGuid(),
                           friendly_error_msg = str,
                            exception_string = ""
                    };
                    p.exception_logs.InsertOnSubmit(e);
                    p.SubmitChanges();
                }
            }
            else
            {
                var json = context.Request.Form[0];
                if (json != null)
                {
                    using (var p = new ProspectingDataContext())
                    {
                        exception_log e = new exception_log
                        {
                            date_time = DateTime.Now,
                            user = Guid.NewGuid(),
                            friendly_error_msg = json.ToString(),
                            exception_string = ""
                        };
                        p.exception_logs.InsertOnSubmit(e);
                        p.SubmitChanges();
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}