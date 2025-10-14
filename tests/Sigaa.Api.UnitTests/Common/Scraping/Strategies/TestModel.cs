namespace Sigaa.Api.UnitTests.Common.Scraping.Strategies;

public sealed class TestModel
{
    public string Name { get; set; } = string.Empty;
    
    public int Age { get; set; }

    public TestNestedModel Nested { get; set; } = new();
    
    public List<string> Items { get; set; } = [];
    
    public List<TestNestedModel> NestedItems { get; set; } = [];
    
    public Dictionary<string, string> Data { get; set; } = new();
}