namespace DdotM.EmailClient.Infrastructure;

public enum EmailProvider
{
  Unknown = 0,
  Smtp = 1,
  Mailgun = 2,
  SendGrid = 3,
  Office365 = 4
}
