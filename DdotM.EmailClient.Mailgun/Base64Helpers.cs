using System.Text;

namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Base64 converter utility class
/// </summary>
public static class Base64Helpers
{
    /// <summary>
    /// Encodes string to Base64
    /// </summary>
    /// <param name="plainText">Plain text to be converted to Base64</param>
    /// <returns>Base64 encoded string</returns>
    public static string Base64Encode(this string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Decodes string from Base64 to plain text
    /// </summary>
    /// <param name="base64EncodedData">Base64 encoded string to be decoded to plain text</param>
    /// <returns>Plain text string</returns>
    public static string Base64Decode(this string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}