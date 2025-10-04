using System.Reflection;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Scraping.Builders;

internal sealed class DictionaryPropertyBuilder<TModel, TKey, TValue> : IPropertyBuilder where TModel : class
{
    private readonly DictionaryPropertyScrapingConfiguration config;

    internal DictionaryPropertyBuilder(PropertyInfo property)
    {
        config = new DictionaryPropertyScrapingConfiguration(property)
        {
            KeyType = typeof(TKey),
            ValueType = typeof(TValue)
        };
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithSelector(string selector)
    {
        config.Selector = selector;
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithKeyAttribute(string name)
    {
        config.KeyAttribute = name;
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithValueAttribute(string name)
    {
        config.ValueAttribute = name;
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithKeyConverter(IValueConverter converter)
    {
        config.KeyConverter = converter;
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithKeyConverter(Func<string?, TKey> converter) =>
        WithKeyConverter(new LambdaConverter<TKey>(converter));

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithValueConverter(IValueConverter converter)
    {
        config.ValueConverter = converter;
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithValueConverter(Func<string?, TValue> converter) =>
        WithValueConverter(new LambdaConverter<TValue>(converter));

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithKeyTransformation(IValueTransform transform)
    {
        config.KeyTransformations.Add(transform);
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithKeyTransformation(Func<string?, string?> transform) =>
        WithKeyTransformation(new LambdaTransform(transform));

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithValueTransformation(IValueTransform transform)
    {
        config.ValueTransformations.Add(transform);
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithValueTransformation(Func<string?, string?> transform) =>
        WithValueTransformation(new LambdaTransform(transform));

    public DictionaryPropertyBuilder<TModel, TKey, TValue> WithDefaultValue(TValue value)
    {
        config.DefaultValue = value;
        return this;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> IsOptional(bool optional = true)
    {
        config.IsOptional = optional;
        return this;
    }
    
    public DictionaryPropertyBuilder<TModel, TKey, TValue> FromSibling(bool fromSibling = true)
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