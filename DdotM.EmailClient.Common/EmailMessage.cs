using System.Collections.Generic;

namespace DdotM.EmailClient.Common
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            FromEmail = new EmailRecipient();
            ToEmails = new List<EmailRecipient>();
            CcEmails = new List<EmailRecipient>();
            BccEmails = new List<EmailRecipient>();
        }

        public EmailRecipient FromEmail { get; }

        public List<EmailRecipient> ToEmails { get; }
        public List<EmailRecipient> CcEmails { get; }
        public List<EmailRecipient> BccEmails { get; }

        public string Subject { get; set; }
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
    }
}
