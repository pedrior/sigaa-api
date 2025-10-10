using Sigapi.Scraping.Converters;

namespace Sigapi.Scraping.Configuration;

internal sealed record CollectionPropertyScrapingConfiguration(PropertyInfo Property) 
    : PropertyScrapingConfiguration(Property)
{
    public required Type ItemType { get; init; }

    public override string Selector { get; set; } = string.Empty;
    
    public string? Attribute { get; set; }
    
    public object? DefaultValue { get; set; }
    
    public IValueConverter? Converter { get; set; }
    
    public override bool IsOptional { get; set; }
}