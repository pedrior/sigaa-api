namespace Sigaa.Api.Features.Account.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class LoginForm
{
    public string Action { get; set; } = string.Empty;

    public Dictionary<string, string> Data { get; set; } = new();

    public Dictionary<string, string> PrepareForSubmission(string username, string password) => new(Data)
    {
        ["form:login"] = username,
        ["form:senha"] = password,
        ["form:width"] = "1920",
        ["form:height"] = "1080"
    };
}