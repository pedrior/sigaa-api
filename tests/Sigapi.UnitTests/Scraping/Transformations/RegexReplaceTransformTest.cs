using System.Text.RegularExpressions;
using Sigapi.Scraping.Transformations;

namespace Sigapi.UnitTests.Scraping.Transformations;

[TestSubject(typeof(RegexReplaceTransform))]
public sealed class RegexReplaceTransformTest
{
    [Theory]
    [InlineData("Remove all digits 123.", @"\d", "", "Remove all digits .")]
    [InlineData("Replace whitespace with hyphen", @"\s", "-", "Replace-whitespace-with-hyphen")]
    public void Transform_ShouldReplaceMatches(string input, string pattern, string replacement, string expected)
    {
        // Arrange
        var transform = new RegexReplaceTransform(new Regex(pattern), replacement);

        // Act
        var result = transform.Transform(input);

        // Assert
        result.Should().Be(expected);
    }
}