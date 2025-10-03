using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Sigapi.Common.OpenApi;

internal sealed class ApiInfoDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "SIGAPI",
            Description = "API REST segura e de alto desempenho para o Sistema Integrado de Gestão de Atividades " +
                          "Acadêmicas (SIGAA) da Universidade Federal da Paraíba (UFPB).",
            Version = "1.0.0"
        };

        return Task.CompletedTask;
    }
}