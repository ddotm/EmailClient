namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Abstraction over HTTP client for sending requests (enables testability and DI).
/// </summary>
public interface IHttpClientAdapter : IDisposable
{
    /// <summary>
    /// Adds a default header to all outgoing requests sent by this adapter.
    /// </summary>
    /// <param name="name">Header name.</param>
    /// <param name="value">Header value.</param>
    void AddHeader(string name, string value);

    /// <summary>
    /// Sends a POST request to the specified endpoint with the given content.
    /// </summary>
    /// <param name="endpoint">Request URL.</param>
    /// <param name="content">Request content.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The HTTP response message.</returns>
    Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content, CancellationToken cancellationToken = default);
}