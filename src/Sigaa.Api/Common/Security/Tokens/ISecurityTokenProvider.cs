namespace Sigaa.Api.Common.Security.Tokens;

internal interface ISecurityTokenProvider
{
    SecurityToken CreateToken(IDictionary<string, object> claims);
}