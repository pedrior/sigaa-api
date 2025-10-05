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

    public async Task<IDocument> ParseAsync(string html, CancellationToken cancellationToken = default)
    {
        var parsed = await parser.ParseDocumentAsync(html, cancellationToken);
        return new Document(new Element(parsed.DocumentElement));
    }
}