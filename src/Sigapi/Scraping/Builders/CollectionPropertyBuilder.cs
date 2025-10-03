using System.Reflection;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;

namespace Sigapi.Scraping.Builders;

internal sealed class CollectionPropertyBuilder<TModel, TValue> : IPropertyBuilder where TModel : class
{
    private readonly CollectionPropertyScrapingConfiguration config;
    
    internal CollectionPropertyBuilder(PropertyInfo property)
    {
        config = new CollectionPropertyScrapingConfiguration(property)
        {
            ItemType = typeof(TValue)
        };
    }

    public CollectionPropertyBuilder<TModel, TValue> WithSelector(string selector)
    {
        config.Selector = selector;
        return this;
    }

    public CollectionPropertyBuilder<TModel, TValue> WithAttribute(string name)
    {
        config.Attribute = name;
        return this;
    }

    public CollectionPropertyBuilder<TModel, TValue> WithDefaultValue(TValue value)
    {
        config.DefaultValue = value;
        return this;
    }

    public CollectionPropertyBuilder<TModel, TValue> WithConversion(IValueConverter converter)
    {
        config.Converter = converter;
        return this;
    }

    public CollectionPropertyBuilder<TModel, TValue> WithConversion(Func<string?, TValue> converter) =>
        WithConversion(new LambdaConverter<TValue>(converter));

    public CollectionPropertyBuilder<TModel, TValue> IsOptional(bool optional = true)
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