using System.Text.RegularExpressions;

namespace Sigapi.Scraping.Document;

internal sealed partial class Element : IElement
{
    private readonly AngleSharp.Dom.IElement element;

    internal Element(AngleSharp.Dom.IElement element)
    {
        this.element = element;
    }
    
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

    public IElement? Query(string selector)
    {
        var result = element.QuerySelector(selector);
        return result is null
            ? null
            : new Element(result);
    }

    public IElement? QueryNextSibling(string selector)
    {
        var sibling = element.NextElementSibling;
        
        while (sibling is not null)
        {
            if (sibling.Matches(selector))
            {
                return new Element(sibling);
            }
            
            sibling = sibling.NextElementSibling;
        }

        return null;
    }
    
    public IEnumerable<IElement> QueryAll(string selector)
    {
        return element.QuerySelectorAll(selector)
            .Select(e => new Element(e));
    }
    
    public IEnumerable<IElement> QueryAllNextSiblings(string selector)
    {
        var sibling = element.NextElementSibling;
        
        while (sibling is not null)
        {
            if (sibling.Matches(selector))
            {
                yield return new Element(sibling);
            }
            
            sibling = sibling.NextElementSibling;
        }
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultiSpaceRegex();
}