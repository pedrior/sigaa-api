namespace Sigapi.Scraping.Converters;

internal sealed class LambdaConverter<TValue> : IValueConverter
{
    private readonly Func<string?, TValue> func;

    public LambdaConverter(Func<string?, TValue> func)
    {
        this.func = func;
    }

    public object? Convert(string? input) => func(input);
}