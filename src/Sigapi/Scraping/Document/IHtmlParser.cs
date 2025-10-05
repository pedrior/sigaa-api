namespace Sigapi.Scraping.Document;

internal interface IHtmlParser
{
    Task<IDocument> ParseAsync(string html, CancellationToken cancellationToken = default);
}