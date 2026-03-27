namespace DdotM.EmailClient.Infrastructure;

public class EmailAddress
{
    /// <summary>
    /// Gets or sets the email address value.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name associated with the address.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
