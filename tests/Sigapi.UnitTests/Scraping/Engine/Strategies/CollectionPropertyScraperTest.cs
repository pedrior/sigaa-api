using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Engine.Strategies;

namespace Sigapi.UnitTests.Scraping.Engine.Strategies;

[TestSubject(typeof(CollectionPropertyScraper))]
public sealed class CollectionPropertyScraperTest
{
    private readonly IConversionService conversionService = A.Fake<IConversionService>();
    private readonly IModelScraperFactory modelScraperFactory = A.Fake<IModelScraperFactory>();
    private readonly IElement rootElement = A.Fake<IElement>();

    private readonly CollectionPropertyScraper sut;

    public CollectionPropertyScraperTest()
    {
        sut = new CollectionPropertyScraper(conversionService, modelScraperFactory);
    }
    
    [Fact]
    public void Execute_ForPrimitiveTypes_ShouldScrapeCollection()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Items))!;

        var config = new CollectionPropertyScrapingConfiguration(property)
        {
            ItemType = typeof(string),
            Selector = ".item"
        };

        var element1 = A.Fake<IElement>();
        var element2 = A.Fake<IElement>();

        A.CallTo(() => element1.GetText()).Returns("First");
        A.CallTo(() => element2.GetText()).Returns("Second");

        A.CallTo(() => rootElement.QueryAll(".item")).Returns([element1, element2]);
        A.CallTo(() => conversionService.Convert(typeof(string), "First", null)).Returns("First");
        A.CallTo(() => conversionService.Convert(typeof(string), "Second", null)).Returns("Second");

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Items.Should().NotBeNull();
        model.Items.Should().HaveCount(2);
        model.Items.Should().ContainInOrder("First", "Second");
    }

    [Fact]
    public void Execute_ForComplexTypes_ShouldScrapeCollection()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.NestedItems))!;

        var config = new CollectionPropertyScrapingConfiguration(property)
        {
            ItemType = typeof(TestNestedModel),
            Selector = ".nested-item"
        };

        var element1 = A.Fake<IElement>();
        var element2 = A.Fake<IElement>();
        var nestedModel1 = new TestNestedModel { Value = "A" };
        var nestedModel2 = new TestNestedModel { Value = "B" };

        var nestedScraper = A.Fake<IModelScraper>();
        var nestedModelType = typeof(TestNestedModel);

        A.CallTo(() => rootElement.QueryAll(".nested-item")).Returns([element1, element2]);
        A.CallTo(() => modelScraperFactory.CreateScraper(nestedModelType)).Returns(nestedScraper);
        A.CallTo(() => nestedScraper.Scrape(element1)).Returns(nestedModel1);
        A.CallTo(() => nestedScraper.Scrape(element2)).Returns(nestedModel2);

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.NestedItems.Should().NotBeNull();
        model.NestedItems.Should().HaveCount(2);
        model.NestedItems.Should().ContainInOrder(nestedModel1, nestedModel2);
    }
}