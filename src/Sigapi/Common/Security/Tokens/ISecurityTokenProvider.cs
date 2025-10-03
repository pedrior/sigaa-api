namespace Sigapi.Common.Security.Tokens;

internal interface ISecurityTokenProvider
{
    (string token, DateTimeOffset expiresAt) CreateToken(IDictionary<string, object> claims);
}