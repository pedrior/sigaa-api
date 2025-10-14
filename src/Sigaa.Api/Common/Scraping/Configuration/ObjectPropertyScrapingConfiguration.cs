namespace Sigaa.Api.Common.Scraping.Configuration;

internal sealed record ObjectPropertyScrapingConfiguration(PropertyInfo Property) 
    : PropertyScrapingConfiguration(Property)
{
    public override string Selector { get; set; } = string.Empty;
    
    public override bool IsOptional { get; set; }
}