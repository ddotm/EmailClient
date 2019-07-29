using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace EmailSender
{
	public class Auth
	{
		public static async Task<SmtpClient> Office365SmtpClient(string user, string pwd)
		{
			var client = new SmtpClient();
			client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
			await client.AuthenticateAsync(user, pwd);

			return client;
		}

		public static async Task Close(SmtpClient smtpClient)
		{
			await smtpClient.DisconnectAsync(true);
			smtpClient.Dispose();
		}
	}
}
