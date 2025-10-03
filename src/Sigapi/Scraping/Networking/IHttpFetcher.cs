using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Networking;

internal interface IHttpFetcher
{
    Task<(Uri url, string html)> FetchAsync(string url, 
        ISession? session,
        CancellationToken cancellationToken = default);

    Task<(Uri url, string html)> FetchWithFormSubmissionAsync(string url,
        IReadOnlyDictionary<string, string> data,
        ISession? session,
        CancellationToken cancellationToken = default);
}