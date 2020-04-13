using System.Collections.Generic;

namespace Infrastructure.EmailManager
{
    public class EmailMessageConfig
    {
        public EmailMessageConfig()
        {
            FromEmail = new EmailRecipient();
            ToEmails = new List<EmailRecipient>();
            CcEmails = new List<EmailRecipient>();
            BccEmails = new List<EmailRecipient>();
        }
        public EmailRecipient FromEmail { get; set; }

        public List<EmailRecipient> ToEmails { get; set; }
        public List<EmailRecipient> CcEmails { get; set; }
        public List<EmailRecipient> BccEmails { get; set; }

        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
    }
}
