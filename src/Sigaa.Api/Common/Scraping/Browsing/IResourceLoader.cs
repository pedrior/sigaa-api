namespace Sigaa.Api.Common.Scraping.Browsing;

internal interface IResourceLoader
{
    IDocumentRequest LoadDocumentAsync(string url);
}