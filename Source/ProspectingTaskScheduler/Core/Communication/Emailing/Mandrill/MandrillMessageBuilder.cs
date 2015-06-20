
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
        private string _attachment1Name;
        private string _attachment1Type;
        private string _attachment1Content;

        public MandrillMessageBuilder (string html, string subject, string fromEmail, string fromName, string toEmail, string toName, string attachment1Name, string attachment1Type, string attachment1Content)
        {
            _html = html;
            _subject = subject;
            _fromEmail = fromEmail;
            _fromName = fromName;
            _toEmail = toEmail;
            _toName = toName;
            _attachment1Name = attachment1Name;
            _attachment1Type = attachment1Type;
            _attachment1Content = attachment1Content;
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
            if (!string.IsNullOrEmpty(_attachment1Name))
            {
                message.attachments = new List<MandrillAttachment>();
                string name = _attachment1Name;
                string type =_attachment1Type;
                string content = _attachment1Content;
                message.attachments.Add(new MandrillAttachment { name = name, type = type, content = content });
            }

            return message;
        }

        private List<MandrillMessageTo> BuildTo()
        {
            MandrillMessageTo to = new MandrillMessageTo { email = _toEmail, name = _toName, type = "to" };
            return new List<MandrillMessageTo>(new [] {to});
        }
    }
}
