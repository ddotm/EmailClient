namespace IdempotentCookie.Email.SendGrid;

internal sealed class SendGridClientAdapterFactory : ISendGridClientAdapterFactory
{
    public ISendGridClientAdapter Create(SendGridClientConfig config)
    {
        return new SendGridClientAdapter(config);
    }
}