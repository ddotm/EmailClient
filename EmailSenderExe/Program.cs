using EmailSender;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailSenderExe
{
	internal class Program
	{
		private static async Task Main(string[] args)
		{
			var config = new Config
			{
				ClientType = ClientType.Office365,
				Id = "",
				Pwd = "", // DO NOT POPULATE, FILLED IN BELOW
				FromEmail = new EmailAddress
				{
					Name = "",
					Address = ""
				},
				ToEmails = new List<EmailAddress>
				{
					new EmailAddress
					{
						Name = "",
						Address = ""
					}
				},
				Subject = "Test email subject",
				HtmlBody = "<h1>Hello World!</h1>",
				TextBody = "Hello World!"
			};
			Console.WriteLine($"Please enter pwd for {config.Id}");
			var pwd = Console.ReadLine();
			config.Pwd = pwd;
			Console.Clear();

			var emailSender = new Sender();

			await emailSender.Send(config);

			await Task.FromResult(0);
		}
	}
}
