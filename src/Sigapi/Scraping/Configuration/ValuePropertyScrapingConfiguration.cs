using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Scraping.Configuration;

internal sealed record ValuePropertyScrapingConfiguration(PropertyInfo Property)
    : PropertyScrapingConfiguration(Property)
{
    public override string Selector { get; set; } = string.Empty;

    public string? Attribute { get; set; }

    public object? DefaultValue { get; set; }

    public IValueConverter? Converter { get; set; }

    public List<IValueTransform> Transformations { get; } = [];

    public override bool IsOptional { get; set; }
}