namespace Sigapi.Scraping.Document;

internal interface IDocument : IElement
{
    public Uri Url { get; internal set; }
}