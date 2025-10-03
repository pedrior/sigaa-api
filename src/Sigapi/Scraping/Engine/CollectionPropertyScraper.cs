using System.Collections;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Exceptions;

namespace Sigapi.Scraping.Engine;

internal sealed class CollectionPropertyScraper : PropertyScraper<CollectionPropertyScrapingConfiguration>
{
    private readonly IModelScraperFactory modelScraperFactory;

    public CollectionPropertyScraper(IConversionService conversionService, 
        IModelScraperFactory modelScraperFactory) : base(conversionService)
    {
        this.modelScraperFactory = modelScraperFactory;
    }

    protected override void ProcessProperty(object model,
        CollectionPropertyScrapingConfiguration config,
        IHtmlElement parent)
    {
        var list = CreateListInstance(config.ItemType);
        var isPrimitiveType = config.ItemType.IsPrimitive || config.ItemType == typeof(string);

        var scraper = isPrimitiveType
            ? null
            : modelScraperFactory.CreateScraper(config.ItemType);

        var elements = parent.QueryAll(config.Selector)
            .ToArray();

        if (elements is [])
        {
            return;
        }

        foreach (var element in elements)
        {
            try
            {
                object? convertedValue;

                if (isPrimitiveType)
                {
                    var rawValue = ExtractRawValue(element, config.Attribute);
                    convertedValue = ConvertValue(
                        config.ItemType,
                        rawValue,
                        config.DefaultValue,
                        config.Converter);
                }
                else
                {
                    convertedValue = scraper!.Scrape(element);
                }

                list.Add(convertedValue);
            }
            catch (ScrapingException ex)
            {
                throw new ScrapingException(
                    $"Failed to scrape item for collection property '{config.Property.Name}'.",
                    ex);
            }
        }
        
        config.Property.SetValue(model, list);
    }

    private static IList CreateListInstance(Type itemType)
    {
        var listType = typeof(List<>).MakeGenericType(itemType);
        return (IList)Activator.CreateInstance(listType)!;
    }
}