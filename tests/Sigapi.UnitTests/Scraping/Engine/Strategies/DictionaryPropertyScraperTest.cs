using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;
using Sigapi.Scraping.Document;
using Sigapi.Scraping.Engine;
using Sigapi.Scraping.Engine.Strategies;

namespace Sigapi.UnitTests.Scraping.Engine.Strategies;

[TestSubject(typeof(DictionaryPropertyScraper))]
public sealed class DictionaryPropertyScraperTest
{
    private readonly IConversionService conversionService = A.Fake<IConversionService>();
    private readonly IHtmlElement rootElement = A.Fake<IHtmlElement>();

    private readonly DictionaryPropertyScraper sut;

    public DictionaryPropertyScraperTest()
    {
        sut = new DictionaryPropertyScraper(conversionService);
    }
    
    [Fact]
    public void Execute_WhenElementsFound_ShouldPopulateDictionary()
    {
        // Arrange
        var model = new TestModel();
        var property = typeof(TestModel).GetProperty(nameof(TestModel.Data))!;
        
        var config = new DictionaryPropertyScrapingConfiguration(property)
        {
            Selector = ".data-row",
            KeyAttribute = "data-key",
            ValueAttribute = "data-value",
            KeyType = typeof(string),
            ValueType = typeof(string)
        };

        var element1 = A.Fake<IHtmlElement>();
        var element2 = A.Fake<IHtmlElement>();
        
        A.CallTo(() => element1.GetAttribute("data-key")).Returns("Color");
        A.CallTo(() => element1.GetAttribute("data-value")).Returns("Blue");
        A.CallTo(() => element2.GetAttribute("data-key")).Returns("Size");
        A.CallTo(() => element2.GetAttribute("data-value")).Returns("Large");

        A.CallTo(() => rootElement.QueryAll(".data-row")).Returns([element1, element2]);
            
        A.CallTo(() => conversionService.Convert(typeof(string), "Color", null)).Returns("Color");
        A.CallTo(() => conversionService.Convert(typeof(string), "Blue", null)).Returns("Blue");
        A.CallTo(() => conversionService.Convert(typeof(string), "Size", null)).Returns("Size");
        A.CallTo(() => conversionService.Convert(typeof(string), "Large", null)).Returns("Large");

        // Act
        sut.Execute(model, config, rootElement);

        // Assert
        model.Data.Should().NotBeNull();
        model.Data.Should().HaveCount(2);
        model.Data.Should().ContainKeys("Color", "Size");
        model.Data["Color"].Should().Be("Blue");
        model.Data["Size"].Should().Be("Large");
    }
}