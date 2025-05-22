namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Email recipient
/// </summary>
public class Recipient
{
    /// <summary>
    /// Email recipient name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email recipient email address
    /// </summary>
    public string Address { get; set; } = string.Empty;
}

public static class RecipientExtensions
{
    /// <summary>
    /// Converts the recipient to a string representation
    /// </summary>
    /// <param name="recipient">The recipient</param>
    /// <returns>The string representation of the recipient</returns>
    public static string ToFullAddress(this Recipient recipient)
    {
        var fullAddress = $"{(string.IsNullOrWhiteSpace(recipient.Name) ? 
            recipient.Address : 
            $"{recipient.Name} <{recipient.Address}>")}";
        
        return fullAddress;
    }
}