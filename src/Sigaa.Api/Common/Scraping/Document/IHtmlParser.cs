namespace Sigaa.Api.Common.Scraping.Document;

internal interface IHtmlParser
{
    Task<IDocument> ParseAsync(string html, CancellationToken cancellationToken = default);
}