namespace DdotM.EmailClient.Infrastructure;

public interface IEmailClient
{
  EmailProvider Provider { get; }

  Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
