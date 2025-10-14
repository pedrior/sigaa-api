using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Converters;
using Sigaa.Api.Common.Scraping.Transformations;

namespace Sigaa.Api.Common.Scraping.Builders;

internal sealed class ValuePropertyBuilder<TModel, TValue> : IPropertyBuilder where TModel : class
{
    private readonly ValuePropertyScrapingConfiguration config;

    internal ValuePropertyBuilder(PropertyInfo property)
    {
        config = new ValuePropertyScrapingConfiguration(property);
    }

    public ValuePropertyBuilder<TModel, TValue> WithSelector(string selector)
    {
        config.Selector = selector;
        return this;
    }

    public ValuePropertyBuilder<TModel, TValue> WithAttribute(string name)
    {
        config.Attribute = name;
        return this;
    }

    public ValuePropertyBuilder<TModel, TValue> WithDefaultValue(TValue value)
    {
        config.DefaultValue = value;
        return this;
    }

    public ValuePropertyBuilder<TModel, TValue> WithConversion(IValueConverter converter)
    {
        config.Converter = converter;
        return this;
    }

    public ValuePropertyBuilder<TModel, TValue> WithConversion(Func<string?, TValue> converter) =>
        WithConversion(new LambdaConverter<TValue>(converter));

    public ValuePropertyBuilder<TModel, TValue> WithTransformation(IValueTransform transform)
    {
        config.Transformations.Add(transform);
        return this;
    }

    public ValuePropertyBuilder<TModel, TValue> WithTransformation(Func<string?, string?> transform) =>
        WithTransformation(new LambdaTransform(transform));

    public ValuePropertyBuilder<TModel, TValue> IsOptional(bool optional = true)
    {
        config.IsOptional = optional;
        return this;
    }
    
    public ValuePropertyBuilder<TModel, TValue> FromSibling(bool fromSibling = true)
    {
        config.SelectorStrategy = fromSibling ? SelectorStrategy.Sibling : SelectorStrategy.Nested;
        return this;
    }

    PropertyScrapingConfiguration IPropertyBuilder.BuildConfiguration()
    {
        config.Validate();
        return config;
    }
}