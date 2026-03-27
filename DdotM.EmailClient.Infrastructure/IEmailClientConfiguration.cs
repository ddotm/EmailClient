namespace DdotM.EmailClient.Infrastructure;

public interface IEmailClientConfiguration
{
  EmailProvider Provider { get; }

  void Validate();
}
