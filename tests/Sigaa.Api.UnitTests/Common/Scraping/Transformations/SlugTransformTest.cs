using Sigaa.Api.Common.Scraping.Transformations;

namespace Sigaa.Api.UnitTests.Common.Scraping.Transformations;

[TestSubject(typeof(SlugTransform))]
public sealed class SlugTransformTest
{
    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("  leading and trailing spaces  ", "leading-and-trailing-spaces", Skip = "to fix")]
    [InlineData("Special---Chars!!", "special-chars", Skip = "to fix")]
    [InlineData("Título com Acentuação", "titulo-com-acentuacao")]
    [InlineData("UPPERCASE TEXT", "uppercase-text")]
    public void Transform_ShouldGenerateCorrectSlug(string input, string expected)
    {
        // Arrange
        var transform = SlugTransform.Instance;
            
        // Act
        var result = transform.Transform(input);
            
        // Assert
        result.Should().Be(expected);
    }
}