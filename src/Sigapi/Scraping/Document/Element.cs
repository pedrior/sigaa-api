using System.Text.RegularExpressions;
using AngleSharp.Dom;

namespace Sigapi.Scraping.Document;

internal sealed partial class Element : IHtmlElement
{
    private readonly IElement element;

    public Element(IElement element)
    {
        this.element = element;
    }

    public Element(AngleSharp.Dom.IDocument document, string location) : this(document.DocumentElement)
    {
        IsRoot = true;
        Location = location;
    }

    public bool IsRoot { get; }

    public string? Location { get; set; }

    public string? GetText()
    {
        var text = element.TextContent;
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return MultiSpaceRegex()
            .Replace(text, " ")
            .Trim();
    }

    public string? GetAttribute(string name) => element.GetAttribute(name);

    public IHtmlElement? Query(string selector)
    {
        var result = element.QuerySelector(selector);
        return result is null
            ? null
            : new Element(result);
    }

    public IEnumerable<IHtmlElement> QueryAll(string selector)
    {
        return element.QuerySelectorAll(selector)
            .Select(e => new Element(e));
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultiSpaceRegex();
}