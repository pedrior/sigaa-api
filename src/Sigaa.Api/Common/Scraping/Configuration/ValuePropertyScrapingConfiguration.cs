using Sigaa.Api.Common.Scraping.Converters;
using Sigaa.Api.Common.Scraping.Transformations;

namespace Sigaa.Api.Common.Scraping.Configuration;

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