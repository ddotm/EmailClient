using MimeKit;
using System.Threading.Tasks;

namespace EmailSender
{
	public class Sender
	{
		public async Task Send(Config config)
		{
			var message = new MimeMessage();

			EmailComposer.AddFromAddress(message, config.FromEmail.Name, config.FromEmail.Address);
			foreach (var emailAddress in config.ToEmails)
			{
				EmailComposer.AddToAddress(message, emailAddress.Name, emailAddress.Address);
			}

			message.Subject = config.Subject;

			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = config.HtmlBody,
				TextBody = config.TextBody
			};

			message.Body = bodyBuilder.ToMessageBody();

			var smtpClient = await Auth.Office365SmtpClient(config.Id, config.Pwd);

			await smtpClient.SendAsync(message);

			await Auth.Close(smtpClient);
		}
	}
}
