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
            
            var base64ApiKey = Base64Encode(_emailClientConfig.ApiKey);
            var body = $"from={msg.FromEmail.Name} <{msg.FromEmail.Address}>&to={msg.BccEmails[0].Address}&subject={msg.Subject}&text={msg.TextBody}";

            var request = new RestRequest("lilipudra.com/messages", Method.POST)
               .AddHeader("Authorization", $"Basic {base64ApiKey}")
               .AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.Body = new RequestBody("application/x-www-form-urlencoded", "", body);

            var response = await RestClient.ExecuteAsync(request);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
