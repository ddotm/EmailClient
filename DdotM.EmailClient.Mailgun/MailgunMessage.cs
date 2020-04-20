using System;
using DdotM.EmailClient.Common;
using System.Collections.Generic;

namespace DdotM.EmailClient.Mailgun
{
    public class MailgunMessage
    {
        public MailgunMessage()
        {
            FromEmail = new EmailRecipient();
            ToEmails = new List<EmailRecipient>();
            CcEmails = new List<EmailRecipient>();
            BccEmails = new List<EmailRecipient>();

            Tags = new List<string>();
        }

        public EmailRecipient FromEmail { get; }

        public List<EmailRecipient> ToEmails { get; }
        public List<EmailRecipient> CcEmails { get; }
        public List<EmailRecipient> BccEmails { get; }

        public string Subject { get; set; }
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }

        // Set message delivery time - format "Fri, 14 Oct 2011 23:10:10 -0000"
        public DateTime? DeliveryTime { get; set; }
        public List<string> Tags { get; set; }
        public bool Tracking { get; set; } = false;

    }
}
