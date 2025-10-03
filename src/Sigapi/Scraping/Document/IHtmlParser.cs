namespace Sigapi.Scraping.Document;

internal interface IHtmlParser
{
    Task<IHtmlElement> ParseAsync(Uri url, string html, CancellationToken cancellationToken = default);
}