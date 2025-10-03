using Sigapi.Scraping.Document;
using ISession = Sigapi.Scraping.Networking.Sessions.ISession;

namespace Sigapi.Scraping.Networking;

internal interface IPageFetcher
{
    Task<IDocument> FetchAndParseAsync(string url,
        ISession? session = null,
        CancellationToken cancellationToken = default);
    
    Task<IDocument> FetchAndParseWithFormSubmissionAsync(string url,
        IReadOnlyDictionary<string, string> data,
        ISession? session = null,
        CancellationToken cancellationToken = default);
}