using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager.EmailClients
{
    public class MailgunEmailClient : IEmailClient
    {
        private RestClient RestClient { get; set; }
        private readonly EmailClientConfig _emailClientConfig;

        public MailgunEmailClient(EmailClientConfig emailClientConfig)
        {
            _emailClientConfig = emailClientConfig;
        }

        public async Task SendAsync(EmailMessageConfig msg)
        {
            RestClient = new RestClient("https://api.mailgun.net/v3/");

            var base64ApiKey = _emailClientConfig.MailgunApiKey?.Base64Encode();
            var body =
                $"from={msg.FromEmail.Name} <{msg.FromEmail.Address}>&to={msg.BccEmails[0].Address}&subject={msg.Subject}&text={msg.TextBody}";

            var request = new RestRequest($@"{_emailClientConfig.MailgunSendingDomain}/messages", Method.POST)
               .AddHeader("Authorization", $"Basic {base64ApiKey}")
               .AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.Body = new RequestBody("application/x-www-form-urlencoded", "", body);

            var response = await RestClient.ExecuteAsync(request);
        }
    }
}
