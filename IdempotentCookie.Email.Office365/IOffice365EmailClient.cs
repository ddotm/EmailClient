namespace IdempotentCookie.Email.Office365;

public interface IOffice365EmailClient
{
    Task SendAsync(EmailMessage emailMessage);
}