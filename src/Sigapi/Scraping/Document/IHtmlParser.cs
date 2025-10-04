namespace Sigapi.Scraping.Document;

internal interface IHtmlParser
{
    Task<IElement> ParseAsync(Uri url, string html, CancellationToken cancellationToken = default);
}