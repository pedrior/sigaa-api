using System.Text.RegularExpressions;
using Sigapi.Scraping.Transformations;

namespace Sigapi.UnitTests.Scraping.Transformations;

[TestSubject(typeof(RegexCaptureTransform))]
public sealed class RegexCaptureTransformTest
{
    [Theory]
    [InlineData("Order #12345", @"#(\d+)", 1, "12345")]
    [InlineData("User: john.doe", @"User: (\S+)", 1, "john.doe")]
    [InlineData("No match here", @"#(\d+)", 1, null)]
    public void Transform_WithGroupIndex_ShouldCaptureCorrectValue(
        string input,
        string pattern,
        int group,
        string? expected)
    {
        // Arrange
        var transform = new RegexCaptureTransform(new Regex(pattern), group);
            
        // Act
        var result = transform.Transform(input);
            
        // Assert
        result.Should().Be(expected);
    }
        
    [Theory]
    [InlineData("Date: 2025-09-22", @"Date: (?<year>\d{4})", "year", "2025")]
    [InlineData("File: declaracao.pdf", @"File: (?<name>\w+)\.(?<ext>\w+)", "name", "declaracao")]
    public void Transform_WithGroupName_ShouldCaptureCorrectValue(
        string input,
        string pattern,
        string groupName,
        string? expected)
    {
        // Arrange
        var transform = new RegexCaptureTransform(new Regex(pattern), groupName);
            
        // Act
        var result = transform.Transform(input);
            
        // Assert
        result.Should().Be(expected);
    }
}