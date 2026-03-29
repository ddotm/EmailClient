using System.Net;

namespace IdempotentCookie.Email.SendGrid;

internal sealed record SendGridResponse(HttpStatusCode StatusCode, string Body);