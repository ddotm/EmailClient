# DdotM.EmailClient


## DdotM.EmailClient.Mailgun NuGet package

##### Integration
Add DdotM.EmailClient.Mailgun NuGet package

```csharp
using DdotM.EmailClient.Mailgun;

public async Task<IRestResponse> SendMailgunEmail()
{
    var mailgunClientConfig = new MailgunClientConfig
                                    {
                                        ApiKey = "MAILGUN_API_KEY",
                                        SendingDomain = "MAILGUN_SENDING_DOMAIN"
                                    };
    var mailgunClient = new MailgunClient(mailgunClientConfig);
    var mailgunMessage = new MailgunMessage();
    mailgunMessage.FromEmail.Name = "SENDER NAME";
    mailgunMessage.FromEmail.Address = "SENDER EMAIL ADDRESS";

    mailgunMessage.BccEmails.Add(new EmailRecipient
                                    {
                                        Name = "RECEIVER NAME",
                                        Address = "RECEIVER EMAIL"
                                    });

    mailgunMessage.Subject = "YOUR MESSAGE TITLE";
    mailgunMessage.TextBody = "YOUR MESSAGE TEXT";
    mailgunMessage.HtmlBody = $"<html><body><p>{mailgunMessage.TextBody}</p></body></html>";

    mailgunMessage.Tags.Add("YOUR TAG");
    mailgunMessage.Tracking = false;
    mailgunMessage.DeliveryTime = DateTime.Now;
    
    var response = await mailgunClient.SendAsync(mailgunMessage);
    return response;
}
```
