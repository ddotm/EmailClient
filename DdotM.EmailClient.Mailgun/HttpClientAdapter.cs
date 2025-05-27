namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Concrete implementation of <see cref="IHttpClientAdapter"/>, wrapping HttpClient.
/// </summary>
internal class HttpClientAdapter : IHttpClientAdapter
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientAdapter"/> class.
    /// </summary>
    public HttpClientAdapter()
    {
        _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
    }

    ///<inheritdoc/>
    public void AddHeader(string name, string value)
    {
        if (_httpClient.DefaultRequestHeaders.Contains(name))
            _httpClient.DefaultRequestHeaders.Remove(name);

        _httpClient.DefaultRequestHeaders.Add(name, value);
    }

    ///<inheritdoc/>
    public Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content, CancellationToken cancellationToken = default)
    {
        return _httpClient.PostAsync(endpoint, content, cancellationToken);
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}