using System.Reflection;

namespace Sigapi.Scraping.Configuration;

internal abstract record PropertyScrapingConfiguration(PropertyInfo Property)
{
    public abstract string Selector { get; set; }
    
    public abstract bool IsOptional { get; set; }
    
    public virtual void Validate()
    {
        if (string.IsNullOrWhiteSpace(Selector))
        {
            throw new InvalidConfigurationException($"Selector must be provided for property '{Property.Name}'.");
        }
    }
}