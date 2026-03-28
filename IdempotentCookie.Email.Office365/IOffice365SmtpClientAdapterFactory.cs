namespace IdempotentCookie.Email.Office365;

internal interface IOffice365SmtpClientAdapterFactory
{
    IOffice365SmtpClientAdapter Create();
}