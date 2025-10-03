using System.Collections.Frozen;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Sigapi.Common.OpenApi.Constants;

namespace Sigapi.Common.OpenApi;

internal sealed class TagsEnhancerDocumentTransformer : IOpenApiDocumentTransformer
{
    private static readonly FrozenDictionary<string, string> Descriptions = new Dictionary<string, string>
    {
        [Tags.Account] = "Endpoints para autenticação, gerenciamento de sessão do usuário e recuperação de dados " +
                         "pessoais do estudante autenticado.",
        [Tags.Centers] = "Endpoints para obter informações sobre os centros acadêmicos, incluindo listagem e perfis " +
                         "detalhados com seus respectivos cursos, programas e departamentos.",
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    public Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        foreach (var tag in document.Tags)
        {
            if (Descriptions.TryGetValue(tag.Name, out var description))
            {
                tag.Description = description;
            }
        }

        return Task.CompletedTask;
    }
}