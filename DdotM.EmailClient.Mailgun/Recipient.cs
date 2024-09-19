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