namespace Sigaa.Api.Common.Scraping.Client;

internal interface IFetcher
{
    IDocumentRequestBuilder FetchDocumentAsync(Uri uri, CancellationToken cancellation = default);
    
    IDocumentRequestBuilder FetchDocumentAsync(string uri, CancellationToken cancellation = default);
}