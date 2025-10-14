using Sigaa.Api.Common.Scraping.Converters;
using Sigaa.Api.Common.Scraping.Transformations;

namespace Sigaa.Api.Common.Scraping.Configuration;

internal sealed record DictionaryPropertyScrapingConfiguration(PropertyInfo Property)
    : PropertyScrapingConfiguration(Property)
{
    public required Type KeyType { get; init; }

    public required Type ValueType { get; init; }

    public override string Selector { get; set; } = string.Empty;

    public string? KeyAttribute { get; set; }

    public string? ValueAttribute { get; set; }

    public IValueConverter? KeyConverter { get; set; }

    public IValueConverter? ValueConverter { get; set; }

    public List<IValueTransform> KeyTransformations { get; } = [];

    public List<IValueTransform> ValueTransformations { get; } = [];

    public object? DefaultValue { get; set; }

    public override bool IsOptional { get; set; }
}