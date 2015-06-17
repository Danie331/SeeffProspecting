
using System.Collections.Generic;
namespace ProspectingTaskScheduler.Core.Communication.Emailing.Mandrill
{
    public class MandrillMessageBuilder
    {
        private string _html;
        private string _subject;
        private string _fromEmail;
        private string _fromName;
        private string _toEmail;
        private string _toName;
        public MandrillMessageBuilder (string html, string subject, string fromEmail, string fromName, string toEmail, string toName/* rest to come */)
        {
            _html = html;
            _subject = subject;
            _fromEmail = fromEmail;
            _fromName = fromName;
            _toEmail = toEmail;
            _toName = toName;
        }

        public MandrillSendRequest BuildMandrillSendRequest()
        {
            MandrillSendRequest req = new MandrillSendRequest();
            req.message = BuildEmailMessage();
            req.key = "e6zjKORiBEyq8Fe87oeGnw";
            req.async = true;
            req.ip_pool = "Main Pool";

            return req;
        }

        private  MandrillEmailMessage BuildEmailMessage()
        {
            MandrillEmailMessage message = new MandrillEmailMessage();
            message.html = _html;
            message.subject = _subject;
            message.from_email = _fromEmail;
            message.from_name = _fromName;
            message.to = BuildTo();

            return message;
        }

        private List<MandrillMessageTo> BuildTo()
        {
            MandrillMessageTo to = new MandrillMessageTo { email = _toEmail, name = _toName, type = "to" };
            return new List<MandrillMessageTo>(new [] {to});
        }
    }
}
