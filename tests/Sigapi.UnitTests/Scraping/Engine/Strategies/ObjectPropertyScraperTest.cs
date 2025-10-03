using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine;

namespace Sigapi.UnitTests.Scraping.Engine.Strategies;

[TestSubject(typeof(ObjectPropertyScraper))]
public sealed class ObjectPropertyScraperTest
{
    private readonly IConversionService conversionService = A.Fake<IConversionService>();
    private readonly IModelScraperFactory modelScraperFactory = A.Fake<IModelScraperFactory>();
    private readonly IHtmlElement rootElement = A.Fake<IHtmlElement>();

    private readonly ObjectPropertyScraper sut;

    public ObjectPropertyScraperTest()
    {
        sut = new ObjectPropertyScraper(conversionService, modelScraperFactory);
    }

    [Fact]
    public void Execute_WhenElementFound_ShouldScrapeAndSetObject()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Nested))!;

        var config = new ObjectPropertyScrapingConfiguration(property)
        {
            Selector = ".nested-data"
        };

        var nestedElement1 = A.Fake<IHtmlElement>();
        var nestedScraper2 = A.Fake<IModelScraper>();

        var expectedNestedModel = new TestNestedModel
        {
            Value = "Nested Info"
        };

        var nestedModelType = typeof(TestNestedModel);

        A.CallTo(() => rootElement.Query(".nested-data")).Returns(nestedElement1);
        A.CallTo(() => modelScraperFactory.CreateScraper(nestedModelType)).Returns(nestedScraper2);
        A.CallTo(() => nestedScraper2.Scrape(nestedElement1)).Returns(expectedNestedModel);

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Nested.Should().Be(expectedNestedModel);
    }

    [Fact]
    public void Execute_WhenElementNotFoundAndOptional_ShouldDoNothing()
    {
        // Arrange
        var model = new TestModel
        {
            Nested = new TestNestedModel
            {
                Value = "default-value"
            }
        };
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Nested))!;

        var config = new ObjectPropertyScrapingConfiguration(property)
        {
            Selector = ".non-existent",
            IsOptional = true
        };

        A.CallTo(() => rootElement.Query(".non-existent")).Returns(null);

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Nested.Value.Should().Be("default-value");
    }
}