using System.Collections.Concurrent;
using Sigapi.Scraping.Builders;

namespace Sigapi.Scraping.Configuration;

internal sealed class ScrapingModelConfigurationProvider : IScrapingModelConfigurationProvider
{
    private readonly ConcurrentDictionary<Type, ScrapingModelConfiguration> cache = new();
    
    private readonly IServiceProvider serviceProvider;

    public ScrapingModelConfigurationProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ScrapingModelConfiguration GetConfiguration<TModel>() where TModel : class
    {
        return cache.GetOrAdd(typeof(TModel), _ =>
        {
            var builder = new ScrapingModelBuilder<TModel>();
            var config = serviceProvider.GetRequiredService<IScrapingModelConfiguration<TModel>>();
            
            config.Configure(builder);
            
            return builder.Build();
        });
    }
}