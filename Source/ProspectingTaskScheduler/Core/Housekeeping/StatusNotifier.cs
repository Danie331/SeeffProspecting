using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace ProspectingTaskScheduler.Core.Housekeeping
{
    public class StatusNotifier
    {
        public static void SendHealthStatusEmail()
        {
            string body = "ProspectingTaskScheduler service running at " + DateTime.Now + " (local time on server)";
            SendEmail("danie.vdm@seeff.com", "ProspectingTaskScheduler", "reports@seeff.com", null, "Notification from ProspectingTaskScheduler service", body);
        }

        public static void SendYesterdaysLightstoneCallLog()
        {
            StringBuilder sb = new StringBuilder();
            using (var boss = new bossEntities())
                using (var prospecting = new ProspectingDataContext())
            {
                DateTime yesterday = DateTime.Now.AddDays(-1).Date;
                var yesterdaysCalls = prospecting.lightstone_call_logs.Where(s => s.date_time.Date == yesterday);
                foreach (var item in yesterdaysCalls)
                {
                    var target = boss.user_registration.FirstOrDefault(ur => ur.user_guid == item.user.ToString().ToLower());
                    string recordItem = item.date_time.ToString() + " | " + target.user_name + " " + target.user_surname + " | " + item.call_location_src;
                    sb.AppendLine(recordItem);
                }

                string subject = "Prospecting Lightstone call history for " + DateTime.Now.AddDays(-1).Date.ToShortDateString();
                string contents = sb.ToString();

                SendEmail("danie.vdm@seeff.com", "ProspectingTaskScheduler", "reports2@seeff.com", null, subject, contents);
            }
        }

        public static void SendEmail(string toAddress, string displayName, string fromAddress, string ccAddress, string subject, string body)
        {
            try
            {
                MailAddress from = new MailAddress(fromAddress, fromAddress, System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(toAddress, displayName);
                MailMessage message = new MailMessage(from, to);
                if (!string.IsNullOrEmpty(ccAddress))
                {
                    message.CC.Add(ccAddress);
                }

                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.Subject = subject;

                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Body = body;

                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(customCertValidation);

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("reports@seeff.com", "cvbiv76c6c");
                    smtpClient.Host = "smtp2.macrolan.co.za";
                    smtpClient.Port = 587;
                    smtpClient.Timeout = 20000;
                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Add logging code later.
            }
        }

        private static bool customCertValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}