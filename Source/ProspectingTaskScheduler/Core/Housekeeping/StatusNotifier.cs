using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ProspectingTaskScheduler.Core.Housekeeping
{
    public class StatusNotifier
    {
        public static void SendNotificationEmail()
        {
            string body = "ProspectingTaskScheduler service alive and well as at " + DateTime.Now + " (local time on server)";
            SendEmail("danie@learnit.co.za", "ProspectingTaskScheduler", "reports@seeff.com", "adam.roberts@learnit.co.za", "Notification from ProspectingTaskScheduler service", body);
        }

        private static void SendEmail(string toAddress, string displayName, string fromAddress, string ccAddress, string subject, string body)
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

            try
            {
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
            catch
            {
                // Add logging code later.
            }
        }
    }
}