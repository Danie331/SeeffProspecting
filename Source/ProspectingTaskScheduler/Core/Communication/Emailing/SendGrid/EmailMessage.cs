using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class EmailMessage
    {
        public List<Personalization> personalizations { get; set; }
        public MessageFrom from { get; set; }
        public ReplyTo reply_to { get; set; }
        public string subject { get; set; }
        public List<MessageContent> content { get; set; }
        public List<Attachment> attachments { get; set; }
        //public string batch_id { get; set; }
        public MailSettings mail_settings { get; set; }
        public TrackingSettings tracking_settings { get; set; }
    }
}