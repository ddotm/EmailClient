namespace IdempotentCookie.Email.SendGrid;

internal interface ISendGridClientAdapterFactory
{
    ISendGridClientAdapter Create(SendGridClientConfig config);
}