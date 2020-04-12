namespace Infrastructure.EmailManager
{
    public class EmailClientConfig
    {
        public EmailClientConfig()
        {
        }

        public EmailClientType EmailClientType { get; set; }
        public string Id { get; set; }
        public string Pwd { get; set; }
        public string ApiKey { get; set; }
    }
}
