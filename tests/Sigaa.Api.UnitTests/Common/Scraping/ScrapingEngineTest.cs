using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Sigaa.Api.Common.Scraping;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Document;

namespace Sigaa.Api.UnitTests.Common.Scraping;

[TestSubject(typeof(ScrapingEngine))]
public sealed class ScrapingEngineTests
{
    private readonly IModelScraperFactory modelScraperFactory;
    private readonly IScrapingModelConfigurationProvider configProvider;
    private readonly IElement rootElement;
    private readonly ScrapingEngine sut;

    public ScrapingEngineTests()
    {
        modelScraperFactory = A.Fake<IModelScraperFactory>();
        configProvider = A.Fake<IScrapingModelConfigurationProvider>();
        rootElement = A.Fake<IElement>();

        sut = new ScrapingEngine(A.Fake<ILogger<ScrapingEngine>>(), modelScraperFactory, configProvider);
    }

    [Fact]
    public void Scrape_WhenRootSelectorIsProvidedAndElementExists_ShouldScrapeFromQueriedElement()
    {
        // Arrange
        var modelConfig = new ScrapingModelConfiguration(
            ".content",
            new ReadOnlyCollection<PropertyScrapingConfiguration>(new List<PropertyScrapingConfiguration>()));

        var modelScraper = A.Fake<IModelScraper<TestModel>>();
        var contentElement = A.Fake<IElement>();

        var expectedModel = new TestModel
        {
            Data = "Scraped!"
        };

        A.CallTo(() => configProvider.GetConfiguration<TestModel>()).Returns(modelConfig);
        A.CallTo(() => modelScraperFactory.CreateScraper<TestModel>()).Returns(modelScraper);
        A.CallTo(() => rootElement.Query(".content")).Returns(contentElement);
        A.CallTo(() => modelScraper.Scrape(contentElement)).Returns(expectedModel);

        // Act
        var result = sut.Scrape<TestModel>(rootElement);

        // Assert
        result.Should().Be(expectedModel);

        A.CallTo(() => modelScraper.Scrape(contentElement)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Scrape_WhenRootSelectorIsNotProvided_ShouldScrapeFromRootElement()
    {
        // Arrange
        var modelConfig = new ScrapingModelConfiguration(
            null,
            new ReadOnlyCollection<PropertyScrapingConfiguration>(new List<PropertyScrapingConfiguration>()));

        var modelScraper = A.Fake<IModelScraper<TestModel>>();
        var expectedModel = new TestModel { Data = "Scraped!" };

        A.CallTo(() => configProvider.GetConfiguration<TestModel>()).Returns(modelConfig);
        A.CallTo(() => modelScraperFactory.CreateScraper<TestModel>()).Returns(modelScraper);
        A.CallTo(() => modelScraper.Scrape(rootElement)).Returns(expectedModel);

        // Act
        var result = sut.Scrape<TestModel>(rootElement);

        // Assert
        result.Should().Be(expectedModel);

        A.CallTo(() => rootElement.Query(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public void Scrape_WhenRootElementIsNotFound_ShouldThrowSelectorNotFoundException()
    {
        // Arrange
        var modelConfig = new ScrapingModelConfiguration(
            ".non-existent",
            new ReadOnlyCollection<PropertyScrapingConfiguration>(new List<PropertyScrapingConfiguration>()));

        A.CallTo(() => configProvider.GetConfiguration<TestModel>()).Returns(modelConfig);
        A.CallTo(() => rootElement.Query(".non-existent")).Returns(null);

        // Act
        var act = () => sut.Scrape<TestModel>(rootElement);

        // Assert
        act.Should().Throw<SelectorNotFoundException>()
            .WithMessage("Root element not found using selector '.non-existent'.");
    }

    [Fact]
    public async Task ScrapeAllAsync_WhenElementsAreFound_ShouldReturnCollectionOfModels()
    {
        // Arrange
        var modelConfig = new ScrapingModelConfiguration(
            ".item",
            new ReadOnlyCollection<PropertyScrapingConfiguration>(new List<PropertyScrapingConfiguration>()));

        var modelScraper = A.Fake<IModelScraper<TestModel>>();

        var element1 = A.Fake<IElement>();
        var element2 = A.Fake<IElement>();

        var model1 = new TestModel { Data = "A" };
        var model2 = new TestModel { Data = "B" };

        A.CallTo(() => configProvider.GetConfiguration<TestModel>()).Returns(modelConfig);
        A.CallTo(() => modelScraperFactory.CreateScraper<TestModel>()).Returns(modelScraper);
        A.CallTo(() => rootElement.QueryAll(".item")).Returns([element1, element2]);
        A.CallTo(() => modelScraper.Scrape(element1)).Returns(model1);
        A.CallTo(() => modelScraper.Scrape(element2)).Returns(model2);

        // Act
        var results = await sut.ScrapeAllAsync<TestModel>(rootElement);

        // Assert
        results.Should().HaveCount(2);
        results.Should().ContainInOrder(model1, model2);
    }

    [Fact]
    public async Task ScrapeAllAsync_WhenRootSelectorIsNull_ShouldThrowScrapingConfigurationException()
    {
        // Arrange
        var modelConfig = new ScrapingModelConfiguration(
            null,
            new ReadOnlyCollection<PropertyScrapingConfiguration>(new List<PropertyScrapingConfiguration>()));

        A.CallTo(() => configProvider.GetConfiguration<TestModel>()).Returns(modelConfig);

        // Act
        var act = () => sut.ScrapeAllAsync<TestModel>(rootElement);

        // Assert
        await act.Should().ThrowAsync<ScrapingConfigurationException>()
            .WithMessage("ScrapeAll<TestModel> must be configured with a root selector.");
    }
}