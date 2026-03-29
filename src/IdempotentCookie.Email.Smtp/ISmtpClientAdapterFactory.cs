namespace IdempotentCookie.Email.Smtp;

internal interface ISmtpClientAdapterFactory
{
    ISmtpClientAdapter Create();
}