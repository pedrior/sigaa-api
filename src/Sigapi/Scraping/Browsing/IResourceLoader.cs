namespace Sigapi.Scraping.Browsing;

internal interface IResourceLoader
{
    IDocumentRequest LoadDocumentAsync(string url);
}