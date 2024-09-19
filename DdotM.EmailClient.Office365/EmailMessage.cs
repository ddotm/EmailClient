namespace DdotM.EmailClient.Office365;

public class EmailMessage
{
    public EmailRecipient FromEmail { get; } = new();

    public List<EmailRecipient> ToEmails { get; } = [];

    public List<EmailRecipient> CcEmails { get; } = [];
    
    public List<EmailRecipient> BccEmails { get; } = [];

    public string Subject { get; set; } = string.Empty;
    
    public string TextBody { get; set; } = string.Empty;
    
    public string HtmlBody { get; set; } = string.Empty;
}