namespace IdempotentCookie.Email;

/// <summary>
/// Represents an email address with an optional display name.
/// </summary>
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
