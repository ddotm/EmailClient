using System;
using System.Linq;
using System.Threading.Tasks;
using DdotM.EmailClient.Common;
using RestSharp;

namespace DdotM.EmailClient.Mailgun
{
    public class MailgunEmailClient : IMailgunEmailClient
    {
        private readonly MailgunClientConfig _mailgunClientConfig;

        public MailgunEmailClient(MailgunClientConfig mailgunClientConfig)
        {
            _mailgunClientConfig = mailgunClientConfig;
        }

        public async Task<IRestResponse> SendAsync(EmailMessageConfig msg)
        {
            // Mailgun API documentation: https://documentation.mailgun.com/en/latest/user_manual.html#sending-via-api
            var client = new RestClient
                         {
                             BaseUrl = new Uri("https://api.mailgun.net/v3")
                         };

            var request = new RestRequest();
            request.AddHeader("Authorization", $"Basic {_mailgunClientConfig.MailgunApiKey?.Base64Encode()}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("domain", _mailgunClientConfig.MailgunSendingDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";

            request.AddParameter("from", $"{msg.FromEmail.Name} <{msg.FromEmail.Address}>");

            if (msg.ToEmails.Any())
            {
                foreach (var toRecipient in msg.ToEmails)
                {
                    request.AddParameter("to", $"{toRecipient.Name} <{toRecipient.Address}>");
                }
            }
            else
            {
                request.AddParameter("to", $"{msg.FromEmail.Name} <{msg.FromEmail.Address}>");
            }

            foreach (var ccRecipient in msg.CcEmails)
            {
                request.AddParameter("cc", $"{ccRecipient.Name} <{ccRecipient.Address}>");
            }

            foreach (var bccRecipient in msg.BccEmails)
            {
                request.AddParameter("bcc", $"{bccRecipient.Name} <{bccRecipient.Address}>");
            }

            request.AddParameter("subject", msg.Subject);
            request.AddParameter("text", msg.TextBody);
            request.AddParameter("html", msg.HtmlBody);

            // This will disable link rewriting for this message
            request.AddParameter("o:tracking", true);
            // Set message delivery time - format "Fri, 14 Oct 2011 23:10:10 -0000"
            // request.AddParameter("o:deliverytime", "Fri, 14 Oct 2011 23:10:10 -0000");
            // Add tag(s)
            request.AddParameter("o:tag", "registration");

            // Send a message with custom connection settings
            request.AddParameter("o:require-tls", true);
            request.AddParameter("o:skip-verification", false);

            request.Method = Method.POST;
            var response = await client.ExecuteAsync(request);

            return response;
        }
    }
}
