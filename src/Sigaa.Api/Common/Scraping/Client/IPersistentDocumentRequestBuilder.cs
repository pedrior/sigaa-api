namespace Sigaa.Api.Common.Scraping.Client;

internal interface IPersistentDocumentRequestBuilder : IDocumentRequestBuilder
{
    IPersistentDocumentRequestBuilder AllowSessionCreation(string requestedSessionId);
}