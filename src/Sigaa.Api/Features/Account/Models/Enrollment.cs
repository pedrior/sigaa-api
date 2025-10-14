namespace Sigaa.Api.Features.Account.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed record Enrollment
{
    private const string NumberKey = "identificador";

    public Dictionary<string, string> Data { get; set; } = new();

    public string Number => Data.GetValueOrDefault(NumberKey, string.Empty);
}