using AngleSharp.Html.Parser;

namespace Sigapi.Scraping.Document;

internal sealed class HtmlParser : IHtmlParser
{
    private readonly AngleSharp.Html.Parser.HtmlParser parser = new(new HtmlParserOptions
    {
        SkipComments = true,
        SkipScriptText = true,
        IsStrictMode = false,
        IsPreservingAttributeNames = true
    });

    public async Task<IHtmlElement> ParseAsync(Uri url, string html, CancellationToken cancellationToken = default) =>
        new Element(await parser.ParseDocumentAsync(html, cancellationToken), url.AbsoluteUri);
}