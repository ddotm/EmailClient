namespace DdotM.EmailClient.Mailgun
{
    public class MailgunClientConfig
    {
        public string ApiKey { get; set; }
        public string SendingDomain { get; set; }
        public bool RequireTls { get; set; } = true;
        public bool SkipVerification { get; set; } = false;
    }
}
