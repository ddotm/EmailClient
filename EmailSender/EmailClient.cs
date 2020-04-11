using System.Threading.Tasks;
using MimeKit;

namespace EmailSender
{
	public static class EmailClient
	{
		public static async Task SendAsync(EmailClientConfig emailClientConfig)
		{
			var message = new MimeMessage();

			EmailComposer.AddFromAddress(message, emailClientConfig.FromEmail.Name, emailClientConfig.FromEmail.Address);
			foreach (var emailAddress in emailClientConfig.ToEmails)
			{
				EmailComposer.AddToAddress(message, emailAddress.Name, emailAddress.Address);
			}

			foreach (var emailAddress in emailClientConfig.CcEmails)
			{
				EmailComposer.AddCcAddress(message, emailAddress.Name, emailAddress.Address);
			}

			foreach (var emailAddress in emailClientConfig.BccEmails)
			{
				EmailComposer.AddBccAddress(message, emailAddress.Name, emailAddress.Address);
			}

			message.Subject = emailClientConfig.Subject;

			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = emailClientConfig.HtmlBody,
				TextBody = emailClientConfig.TextBody
			};

			message.Body = bodyBuilder.ToMessageBody();

			var smtpClient = await Auth.Office365SmtpClientAsync(emailClientConfig.Id, emailClientConfig.Pwd);

			await smtpClient.SendAsync(message);

			await Auth.CloseAsync(smtpClient);
		}
	}
}
