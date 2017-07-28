using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.Communication.Emailing.SendGrid
{
    public class MessageBuilder
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
        private int? _userBusinessUnitID;

        public MessageBuilder(string html, string subject, string fromEmail, string fromName, string toEmail, string toName, string attachment1Name, string attachment1Type, string attachment1Content, int? userBusinessUnitID)
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
            _userBusinessUnitID = userBusinessUnitID;
        }

        public EmailMessage BuildMessage()
        {
            EmailMessage msg = new EmailMessage();
            msg.personalizations = new List<Personalization>
            {
                new Personalization {
                    to = new List<MessageTo> {
                        new MessageTo
                        {
                            email = _toEmail,
                            name = _toName
                        }
                    },
                     subject = _subject,
                     custom_args = new CustomData { TransactionIdentifier = Guid.NewGuid().ToString() }  
                }
            };
            if (_userBusinessUnitID.HasValue && _userBusinessUnitID == 10)
            {
                msg.personalizations.First().bcc = new List<MessageTo> { new MessageTo { email = _fromEmail } };
            }

            msg.from = new MessageFrom { email = _fromEmail, name = _fromName };
            msg.reply_to = new ReplyTo { email = _fromEmail, name = _fromName };
            msg.subject = _subject;
            msg.content = new List<MessageContent> { new MessageContent { type = "text/html", value = _html  } };

            if (!string.IsNullOrEmpty(_attachment1Name))
            {
                msg.attachments = new List<Attachment> {
                    new Attachment {
                        content = _attachment1Content,
                        type = _attachment1Type,
                        filename = _attachment1Name,
                        content_id = _attachment1Name,
                        disposition = "inline"
                    }
                };
            }

            //msg.mail_settings = new MailSettings {
            //    spam_check = new SpamCheck {
            //        enable = true,
            //        threshold = 1,
            //        post_to_url = "",
            //    }
            //};

            msg.tracking_settings = new TrackingSettings
            {
                click_tracking = new ClickTracking
                {
                    enable = true,
                    enable_text = true
                },
                open_tracking = new OpenTracking
                {
                    enable = true
                }
            };            

            return msg;
        }
    }
}