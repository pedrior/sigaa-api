using System.Runtime.CompilerServices;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.Common.Scraping.Client;

internal interface IDocumentRequestBuilder
{
    IPersistentDocumentRequestBuilder WithPersistentSession();
    
    IDocumentRequestBuilder WithEphemeralSession();

    IDocumentRequestBuilder WithFormData(IDictionary<string, string> data);
    
    TaskAwaiter<IDocument> GetAwaiter();
    
    Task<IDocument> AsTask();
}