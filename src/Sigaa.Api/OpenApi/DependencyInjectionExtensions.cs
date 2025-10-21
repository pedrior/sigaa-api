using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Sigaa.Api.OpenApi;

internal static class DependencyInjectionExtensions
{
    private const string ApiTitle = "SIGAA UFPB API";
    private const string ApiVersion = "v1.0.0";

    private const string ApiDescription = """
                                          Uma API REST <strong>não oficial</strong>, segura e de alto desempenho para o 
                                          Sistema Integrado de Gestão de Atividades Acadêmicas (SIGAA) da Universidade 
                                          Federal da Paraíba (UFPB).

                                          <h2>Visão Geral</h2>
                                          Esta API atua como um <em>wrapper</em> sobre o SIGAA, extraindo dados através 
                                          de <em>web scraping</em> para fornecer uma interface moderna e estruturada 
                                          para desenvolvedores. O objetivo é simplificar a integração e a criação de 
                                          novas aplicações que utilizam informações acadêmicas da UFPB.

                                          <h2>Principais Funcionalidades</h2>
                                          <ul>
                                            <li>
                                              <strong>Autenticação</strong>: Gerenciamento de sessão seguro através de 
                                              tokens JWT.
                                            </li>
                                            <li>
                                              <strong>Consulta de Perfil</strong>: Acesso a informações detalhadas do 
                                              perfil do estudante.
                                            </li>
                                            <li>
                                              <strong>Dados Públicos</strong>: Acesso a informações sobre Centros e 
                                              Departamentos Acadêmicos.
                                            </li>
                                          </ul>

                                          <h2>Autenticação</h2>
                                          Para acessar os recursos protegidos, você deve primeiro obter um token de 
                                          acesso através do endpoint <code>POST /login</code> e incluí-lo no cabeçalho 
                                          <code>Authorization</code> de suas requisições no formato 
                                          <code>Bearer {seu-token}</code>.

                                          <br/><hr/>
                                          <em>Este é um projeto não oficial e não possui vínculo direto com a UFPB ou a 
                                          Superintendência de Tecnologia da Informação (STI). As informações são obtidas 
                                          estão sujeitas a alterações conforme o site do SIGAA é atualizado.</em>
                                          """;

    private static readonly OpenApiContact ApiContact = new()
    {
        Name = "Pedro Júnior",
        Email = "pedrojdev@gmail.com",
        Url = new Uri("https://github.com/pedrior/sigaa-api")
    };

    private static readonly OpenApiLicense ApiLicense = new()
    {
        Name = "Licença MIT",
        Url = new Uri("https://opensource.org/licenses/MIT")
    };

    public static WebApplicationBuilder AddOpenApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;

            options.AddScalarTransformers();

            options.AddDocumentTransformer<DynamicBaseServerDocumentTransformer>();
            options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();

            // Enrich the OpenAPI document with API information.
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = ApiTitle,
                    Version = ApiVersion,
                    Description = ApiDescription,
                    Contact = ApiContact,
                    License = ApiLicense
                };

                EnrichDocumentTags(document);

                return Task.CompletedTask;
            });
        });

        return builder;
    }

    private static void EnrichDocumentTags(OpenApiDocument document)
    {
        (string Name, string Description)[] tags =
        [
            (EndpointExtensions.AuthAndProfileTag,
                "Endpoints para gerenciamento de sessão e dados do perfil do usuário."),
            (EndpointExtensions.AcademicCenterTag,
                "Endpoints para consulta de informações sobre os centros acadêmicos da UFPB."),
            (EndpointExtensions.AcademicDepartmentTag,
                "Endpoints para consulta de informações sobre os departamentos acadêmicos da UFPB.")
        ];

        if (document.Tags is null)
        {
            return;
        }

        // Update existing tags with descriptions.
        foreach (var tag in document.Tags)
        {
            if (tags.FirstOrDefault(t => t.Name == tag.Name) is { } matchingTag)
            {
                tag.Description = matchingTag.Description;
            }
        }
    }
}