namespace DdotM.EmailClient.Infrastructure;

public class EmailAttachment
{
    /// <summary>
    /// Gets or sets the attachment file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the attachment content type, such as application/pdf.
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";

    /// <summary>
    /// Gets or sets the binary attachment content.
    /// </summary>
    public byte[] Content { get; set; } = [];
}
