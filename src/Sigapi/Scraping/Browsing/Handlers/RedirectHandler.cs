namespace Sigapi.Scraping.Browsing.Handlers;

internal sealed class RedirectHandler(ILogger<RedirectHandler> logger) : DelegatingHandler
{
    private const int MaxRedirects = 10;

    private static readonly HashSet<int> RedirectStatusCodes = [301, 302, 303, 307, 308];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var redirectCount = 0;

        while (redirectCount < MaxRedirects)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!IsRedirectResponse(response))
            {
                return response;
            }

            redirectCount++;

            var location = response.Headers.Location;
            if (location is null)
            {
                logger.LogWarning(
                    "Redirect status code {StatusCode} received from {Uri} but Location header is missing",
                    response.StatusCode,
                    request.RequestUri);

                return response;
            }

            // Resolve potential relative URLs.
            location = new Uri(request.RequestUri!, location);

            logger.LogInformation(
                "Redirect #{RedirectCount} ({StatusCode}) detected to location: {Location}",
                redirectCount,
                response.StatusCode,
                location);

            // HTTPS -> HTTP redirect.
            if (request.RequestUri?.Scheme == Uri.UriSchemeHttps && location.Scheme == Uri.UriSchemeHttp)
            {
                logger.LogWarning("Upgrading unsafe redirect from HTTP to HTTPS");

                location = new UriBuilder(location)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = location.Port is 80
                        ? 443
                        : location.Port
                }.Uri;
            }

            var nextRequest = await CreateNextRequestMessageAsync(
                request,
                response,
                location);

            request.Dispose();
            response.Dispose();

            request = nextRequest;
        }

        logger.LogError(
            "Maximum number of redirects ({MaxRedirects}) exceeded at {Uri}",
            MaxRedirects,
            request.RequestUri);

        throw new HttpRequestException(
            $"Maximum number of redirects ({MaxRedirects}) exceeded at {request.RequestUri}");
    }

    private static bool IsRedirectResponse(HttpResponseMessage response) =>
        RedirectStatusCodes.Contains((int)response.StatusCode) && response.Headers.Location is not null;

    private static async Task<HttpRequestMessage> CreateNextRequestMessageAsync(HttpRequestMessage previousRequest,
        HttpResponseMessage redirectResponse,
        Uri newLocation)
    {
        var method = previousRequest.Method;
        var cloneContent = true;

        switch ((int)redirectResponse.StatusCode)
        {
            case 301:
            case 302:
                if (method == HttpMethod.Post)
                {
                    method = HttpMethod.Get;
                    cloneContent = false;
                }

                break;
            case 303:
                method = HttpMethod.Get;
                cloneContent = false;
                break;
            // For 307 and 308, method and content are preserved.
        }

        var nextRequest = await CloneHttpRequestMessageAsync(previousRequest, cloneContent);

        nextRequest.RequestUri = newLocation;
        nextRequest.Method = method;
        nextRequest.Headers.Authorization = null; // Removes authorization header for security reasons.

        return nextRequest;
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage originalRequest,
        bool cloneContent)
    {
        var clone = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

        foreach (var header in originalRequest.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var (key, value) in originalRequest.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(key), value);
        }

        if (!cloneContent || originalRequest.Content is null)
        {
            return clone;
        }

        var stream = new MemoryStream();
        await originalRequest.Content.CopyToAsync(stream);

        stream.Position = 0;
        clone.Content = new StreamContent(stream);

        foreach (var header in originalRequest.Content.Headers)
        {
            clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}