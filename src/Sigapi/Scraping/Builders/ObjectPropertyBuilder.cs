using System.Reflection;
using Sigapi.Scraping.Configuration;

namespace Sigapi.Scraping.Builders;

internal sealed class ObjectPropertyBuilder<TModel, TValue> : IPropertyBuilder
    where TModel : class
    where TValue : class, new()
{
    private readonly ObjectPropertyScrapingConfiguration config;

    internal ObjectPropertyBuilder(PropertyInfo property)
    {
        config = new ObjectPropertyScrapingConfiguration(property);
    }

    public ObjectPropertyBuilder<TModel, TValue> WithSelector(string selector)
    {
        config.Selector = selector;
        return this;
    }

    public ObjectPropertyBuilder<TModel, TValue> IsOptional(bool optional = true)
    {
        config.IsOptional = optional;
        return this;
    }

    PropertyScrapingConfiguration IPropertyBuilder.BuildConfiguration()
    {
        config.Validate();
        return config;
    }
}