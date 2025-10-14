using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Converters;
using Sigaa.Api.Common.Scraping.Document;
using Sigaa.Api.Common.Scraping.Strategies;
using Sigaa.Api.Common.Scraping.Transformations;

namespace Sigaa.Api.UnitTests.Common.Scraping.Strategies;

[TestSubject(typeof(ValuePropertyScraper))]
public sealed class ValuePropertyScraperTest
{
    private readonly IConversionService conversionService = A.Fake<IConversionService>();
    private readonly IElement rootElement = A.Fake<IElement>();

    private readonly ValuePropertyScraper sut;

    public ValuePropertyScraperTest()
    {
        sut = new ValuePropertyScraper(conversionService);
    }

    [Fact]
    public void Execute_WhenElementFound_ShouldSetValueFromTextContent()
    {
        // Arrange
        var model = new TestModel();
        var propertyInfo = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;
        var config = new ValuePropertyScrapingConfiguration(propertyInfo)
        {
            Selector = ".name"
        };

        var element = A.Fake<IElement>();

        A.CallTo(() => element.GetText()).Returns("John Doe");
        A.CallTo(() => rootElement.Query(".name")).Returns(element);
        A.CallTo(() => conversionService.Convert(typeof(string), "John Doe", null)).Returns("John Doe");

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Name.Should().Be("John Doe");
    }

    [Fact]
    public void Execute_WhenAttributeIsSpecified_ShouldSetValueFromAttribute()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;
        var config = new ValuePropertyScrapingConfiguration(property)
        {
            Selector = "a",
            Attribute = "href"
        };

        var element = A.Fake<IElement>();

        A.CallTo(() => element.GetAttribute("href")).Returns("/profile");
        A.CallTo(() => rootElement.Query("a")).Returns(element);
        A.CallTo(() => conversionService.Convert(typeof(string), "/profile", null)).Returns("/profile");

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Name.Should().Be("/profile");
    }

    [Fact]
    public void Execute_WhenTransformationsArePresent_ShouldApplyThemInOrder()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;

        var transform1 = A.Fake<IValueTransform>();
        var transform2 = A.Fake<IValueTransform>();

        var config = new ValuePropertyScrapingConfiguration(property)
        {
            Selector = ".name",
            Transformations = { transform1, transform2 }
        };

        var fakeNameElement = A.Fake<IElement>();
        A.CallTo(() => fakeNameElement.GetText()).Returns("  raw value  ");
        A.CallTo(() => rootElement.Query(".name")).Returns(fakeNameElement);

        A.CallTo(() => transform1.Transform("  raw value  ")).Returns("raw value");
        A.CallTo(() => transform2.Transform("raw value")).Returns("RAW VALUE");
        A.CallTo(() => conversionService.Convert(typeof(string), "RAW VALUE", null)).Returns("RAW VALUE");

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Name.Should().Be("RAW VALUE");

        A.CallTo(() => transform1.Transform("  raw value  ")).MustHaveHappenedOnceExactly();
        A.CallTo(() => transform2.Transform("raw value")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Execute_WhenElementNotFoundAndNotOptional_ShouldThrowSelectorNotFoundException()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;
        var config = new ValuePropertyScrapingConfiguration(property)
        {
            Selector = ".non-existent",
            IsOptional = false
        };

        A.CallTo(() => rootElement.Query(".non-existent")).Returns(null);

        // Act
        var act = () => sut.Execute(model, config, rootElement);

        // Assert
        act.Should().Throw<SelectorNotFoundException>();
    }

    [Fact]
    public void Execute_WhenElementNotFoundAndHasDefaultValue_ShouldSetDefaultValue()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Age))!;
        var config = new ValuePropertyScrapingConfiguration(property)
        {
            Selector = ".age",
            DefaultValue = 25
        };

        A.CallTo(() => rootElement.Query(".age")).Returns(null);

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Age.Should().Be(25);
    }
    
    [Fact]
    public void Execute_WhenSelectorStrategyIsSibling_ShouldQueryNextSibling()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Name))!;
        var config = new ValuePropertyScrapingConfiguration(property)
        {
            Selector = ".sibling-name",
            SelectorStrategy = SelectorStrategy.Sibling
        };

        var siblingElement = A.Fake<IElement>();

        A.CallTo(() => siblingElement.GetText()).Returns("Sibling Name");
        A.CallTo(() => rootElement.QueryNextSibling(".sibling-name")).Returns(siblingElement);
        A.CallTo(() => conversionService.Convert(typeof(string), "Sibling Name", null)).Returns("Sibling Name");

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Name.Should().Be("Sibling Name");
        
        A.CallTo(() => rootElement.Query(".sibling-name")).MustNotHaveHappened();
        A.CallTo(() => rootElement.QueryNextSibling(".sibling-name")).MustHaveHappenedOnceExactly();
    }
}