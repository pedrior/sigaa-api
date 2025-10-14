namespace Sigaa.Api.Common.Scraping.Transformations;

internal sealed class LambdaTransform : IValueTransform
{
    private readonly Func<string?, string?> func;

    public LambdaTransform(Func<string?, string?> func)
    {
        this.func = func;
    }

    public string? Transform(string? value) => func(value);
}