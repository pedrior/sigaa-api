using System.Linq.Expressions;
using System.Reflection;
using Sigapi.Scraping.Configuration;

namespace Sigapi.Scraping.Builders;

internal sealed class ScrapingModelBuilder<TModel> where TModel : class
{
    private string? selectorValue;
    private readonly List<IPropertyBuilder> builders = [];

    internal ScrapingModelBuilder()
    {
    }

    public ScrapingModelBuilder<TModel> WithSelector(string selector)
    {
        selectorValue = selector;

        return this;
    }

    public ValuePropertyBuilder<TModel, TValue> Value<TValue>(Expression<Func<TModel, TValue>> expression)
    {
        var builder = new ValuePropertyBuilder<TModel, TValue>(GetProperty(expression));
        builders.Add(builder);

        return builder;
    }

    public ObjectPropertyBuilder<TModel, TValue> Object<TValue>(Expression<Func<TModel, TValue>> expression)
        where TValue : class, new()
    {
        var builder = new ObjectPropertyBuilder<TModel, TValue>(GetProperty(expression));
        builders.Add(builder);

        return builder;
    }

    public CollectionPropertyBuilder<TModel, TValue> Collection<TValue>(
        Expression<Func<TModel, IEnumerable<TValue>>> expression)
    {
        var builder = new CollectionPropertyBuilder<TModel, TValue>(GetProperty(expression));
        builders.Add(builder);

        return builder;
    }

    public DictionaryPropertyBuilder<TModel, TKey, TValue> Dictionary<TKey, TValue>(
        Expression<Func<TModel, IReadOnlyDictionary<TKey, TValue>>> expression)
    {
        var builder = new DictionaryPropertyBuilder<TModel, TKey, TValue>(GetProperty(expression));
        builders.Add(builder);

        return builder;
    }

    internal ScrapingModelConfiguration Build()
    {
        var properties = builders.Select(b => b.BuildConfiguration())
            .ToList()
            .AsReadOnly();

        return new ScrapingModelConfiguration(selectorValue, properties);
    }

    private static PropertyInfo GetProperty(LambdaExpression expression)
    {
        return expression.Body is MemberExpression { Member: PropertyInfo property }
            ? property
            : throw new InvalidOperationException("The expression must be a property access.");
    }
}