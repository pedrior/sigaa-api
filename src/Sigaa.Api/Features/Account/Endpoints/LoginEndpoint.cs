using Sigaa.Api.Common.Endpoints;
using Sigaa.Api.Common.RateLimiting;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Client;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Common.Security.Tokens;
using Sigaa.Api.Features.Account.Contracts;
using Sigaa.Api.Features.Account.Exceptions;
using Sigaa.Api.Features.Account.Models;
using Sigaa.Api.Features.Account.Scraping;

namespace Sigaa.Api.Features.Account.Endpoints;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class LoginEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder route)
    {
        route.MapPost("/login", HandleAsync)
            .RequireRateLimiting(RateLimiterPolicies.Account.SessionManagement)
            .Accepts<LoginRequest>("application/json")
            .Produces<LoginResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    /// <summary>
    /// Autentica um estudante e retorna um token de acesso (Login).
    /// </summary>
    /// <remarks>
    /// Este endpoint realiza a autenticação de um estudante utilizando seu nome de usuário e senha.
    /// Em caso de sucesso, um token JWT é gerado para ser utilizado em endpoints que requerem autenticação.
    /// <br/><br/>
    /// É possível especificar uma matrícula (<c>enrollment</c>) para vincular a sessão a um curso específico.
    /// Se nenhuma matrícula for fornecida e o estudante possuir múltiplos vínculos, a matrícula mais recente será
    /// utilizada por padrão.
    /// </remarks>
    /// <param name="request">O corpo da requisição contendo o nome de usuário, senha e opcionalmente a matrícula.</param>
    /// <returns>Um objeto contendo o token de acesso e sua data de expiração.</returns>
    /// <response code="200">Autenticação bem-sucedida. Retorna o token de acesso.</response>
    /// <response code="400">A requisição é inválida. Verifique os campos enviados.</response>
    /// <response code="401">Credenciais inválidas (usuário/senha) ou matrícula não encontrada para o usuário.</response>
    internal static async Task<IResult> HandleAsync(LoginRequest request,
        HttpContext context,
        IFetcher fetcher,
        IScraper scraper,
        IEnumerable<ILoginResponseHandler> responseHandlers,
        ISecurityTokenProvider securityTokenProvider,
        CancellationToken cancellationToken)
    {
        var sessionId = $"{Guid.NewGuid()}";
        var loginDocument = await fetcher.FetchDocumentAsync(AccountPages.Login, cancellationToken)
            .WithPersistentSession()
            .AllowSessionCreation(sessionId);

        var loginForm = scraper.Scrape<LoginForm>(loginDocument);
        var loginFormData = loginForm.PrepareForSubmission(request.Username, request.Password);
        var loginResponseDocument = await fetcher.FetchDocumentAsync(loginForm.Action, cancellationToken)
            .WithFormData(loginFormData)
            .WithPersistentSession();

        var user = await HandleLoginResponseAsync(
            loginResponseDocument,
            responseHandlers,
            request.Enrollment,
            cancellationToken);
        
        var response = CreateAccessToken(
            sessionId,
            request.Username,
            user.Enrollment,
            user.Enrollments,
            securityTokenProvider);

        return Results.Ok(response);
    }
    
    private static Task<User> HandleLoginResponseAsync(IDocument document,
        IEnumerable<ILoginResponseHandler> handlers,
        string? enrollment = null,
        CancellationToken cancellationToken = default)
    {
        var handler = handlers.FirstOrDefault(strategy => strategy.Evaluate(document));
        return handler is null
            ? throw new LoginException($"No login response handler found for page {document.Url}.")
            : handler.HandleAsync(document, enrollment, cancellationToken);
    }

    private static LoginResponse CreateAccessToken(string sessionId,
        string username,
        Enrollment enrollment,
        IEnumerable<Enrollment> enrollments,
        ISecurityTokenProvider securityTokenProvider)
    {
        var claims = new Dictionary<string, object>
        {
            [CustomClaimTypes.Username] = username,
            [CustomClaimTypes.Enrollment] = enrollment.Number,
            [CustomClaimTypes.Enrollments] = enrollments.OrderByDescending(e => e.Number)
                .Select(e => e.Number)
                .ToArray(),
            [JwtRegisteredClaimNames.Sid] = sessionId
        };

        var (token, expiresAt) = securityTokenProvider.CreateToken(claims);
        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}