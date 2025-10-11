using System.Collections.Frozen;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
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
        [Tags.Departments] = "Endpoints para acessar informações sobre os departamentos acadêmicos, incluindo " +
                             "listagem e consultas detalhadas."
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    public Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (document.Tags is not { } tags)
        {
            return Task.CompletedTask;
        }

        foreach (var tag in tags)
        {
            if (!string.IsNullOrEmpty(tag.Name) && Descriptions.TryGetValue(tag.Name, out var description))
            {
                tag.Description = description;
            }
        }

        return Task.CompletedTask;
    }
}