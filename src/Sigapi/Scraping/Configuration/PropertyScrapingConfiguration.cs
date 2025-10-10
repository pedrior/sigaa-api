namespace Sigapi.Scraping.Configuration;

internal abstract record PropertyScrapingConfiguration(PropertyInfo Property)
{
    public abstract string Selector { get; set; }
    
    public abstract bool IsOptional { get; set; }
    
    public SelectorStrategy SelectorStrategy { get; set; } = SelectorStrategy.Nested;
    
    public virtual void Validate()
    {
    }
}