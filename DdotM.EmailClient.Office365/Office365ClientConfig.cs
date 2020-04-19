using DdotM.EmailClient.Common;

namespace DdotM.EmailClient.Office365
{
    public class Office365ClientConfig
    {
        public Office365ClientConfig()
        {
        }

        public EmailClientType EmailClientType { get; set; }
        public string Id { get; set; }
        public string Pwd { get; set; }
        public string MailgunApiKey { get; set; }
        public string MailgunSendingDomain { get; set; }
    }
}
