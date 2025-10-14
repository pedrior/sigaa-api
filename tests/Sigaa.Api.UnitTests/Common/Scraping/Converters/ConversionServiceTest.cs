using Sigaa.Api.Common.Scraping.Converters;

namespace Sigaa.Api.UnitTests.Common.Scraping.Converters;

[TestSubject(typeof(ConversionService))]
public class ConversionServiceTests
{
    private readonly ConversionService sut = new();
    private readonly IValueConverter converter = A.Fake<IValueConverter>();

    [Fact]
    public void Convert_WhenCustomConverterIsProvided_ShouldUseCustomConverter()
    {
        // Arrange
        const string inputValue = "any-value";
        const int expectedValue = 123;

        A.CallTo(() => converter.Convert(inputValue)).Returns(expectedValue);

        // Act
        var result = sut.Convert(typeof(int), inputValue, converter);

        // Assert
        result.Should().Be(expectedValue);

        A.CallTo(() => converter.Convert(inputValue)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("123", typeof(int), 123)]
    [InlineData("true", typeof(bool), true)]
    [InlineData("123.45", typeof(double), 123.45)]
    [InlineData("2025-09-22", typeof(DateTime), "2025-09-22T00:00:00")]
    public void Convert_WhenUsingStandardTypeConverter_ShouldConvertToTargetType(
        string inputValue,
        Type targetType,
        object expectedValue)
    {
        // Arrange
        // For DateTime, we need to parse it to compare correctly
        if (targetType == typeof(DateTime))
        {
            expectedValue = DateTime.Parse((string)expectedValue);
        }

        // Act
        var result = sut.Convert(targetType, inputValue, null);

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public void Convert_WhenTargetIsNullableAndValueIsNull_ShouldReturnNull()
    {
        // Arrange
        string? inputValue = null;

        // Act
        var result = sut.Convert(typeof(int?), inputValue, null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Convert_WhenTargetIsReferenceTypeAndValueIsNull_ShouldReturnNull()
    {
        // Arrange
        string? inputValue = null;

        // Act
        var result = sut.Convert(typeof(string), inputValue, null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Convert_WhenValueIsEmptyAndTargetIsNullable_ShouldReturnNull()
    {
        // Arrange
        const string inputValue = "";

        // Act
        var result = sut.Convert(typeof(int?), inputValue, null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Convert_WhenValueIsEmptyAndTargetIsString_ShouldReturnNull()
    {
        // Arrange
        const string inputValue = "";

        // Act
        var result = sut.Convert(typeof(string), inputValue, null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Convert_WhenValueIsNullAndTargetIsNonNullableValueType_ShouldThrowException()
    {
        // Arrange
        string? inputValue = null;

        // Act
        Action act = () => sut.Convert(typeof(int), inputValue, null);

        // Assert
        act.Should().Throw<ScrapingConversionException>()
            .WithMessage("Cannot convert null/empty string to non-nullable type Int32.");
    }

    [Fact]
    public void Convert_WhenValueIsEmptyAndTargetIsNonNullableValueType_ShouldThrowException()
    {
        // Arrange
        const string inputValue = "";

        // Act
        Action act = () => sut.Convert(typeof(int), inputValue, null);

        // Assert
        act.Should().Throw<ScrapingConversionException>()
            .WithMessage("Cannot convert null/empty string to non-nullable type Int32.");
    }

    [Fact]
    public void Convert_WhenConversionFailsForStandardType_ShouldThrowScrapingConversionException()
    {
        // Arrange
        const string inputValue = "not-an-int";

        // Act
        Action act = () => sut.Convert(typeof(int), inputValue, null);

        // Assert
        act.Should().Throw<ScrapingConversionException>()
            .WithMessage($"Conversion to Int32 failed for value '{inputValue}'.");
    }

    [Fact]
    public void Convert_WhenTypeConverterIsMissing_ShouldThrowScrapingConversionException()
    {
        // Arrange
        const string inputValue = "some-value";

        // Act
        Action act = () => sut.Convert(typeof(TestStruct), inputValue, null);

        // Assert
        act.Should().Throw<ScrapingConversionException>()
            .WithMessage("No type converter found to convert string to TestStruct.");
    }

    [Theory]
    [InlineData("ValueOne", TestEnum.ValueOne)]
    [InlineData("valueone", TestEnum.ValueOne)]
    [InlineData("VALUEONE", TestEnum.ValueOne)]
    [InlineData("ValueTwo", TestEnum.ValueTwo)]
    internal void Convert_ForEnumType_ShouldConvertSuccessfully(string input, TestEnum expected)
    {
        // Arrange & Act
        var result = sut.Convert(typeof(TestEnum), input, null);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Convert_ForInvalidEnumValue_ShouldThrowScrapingConversionException()
    {
        // Arrange
        const string inputValue = "InvalidValue";

        // Act
        Action act = () => sut.Convert(typeof(TestEnum), inputValue, null);

        // Assert
        act.Should().Throw<ScrapingConversionException>()
            .WithMessage($"Cannot convert string '{inputValue}' to enum TestEnum.");
    }

    internal enum TestEnum
    {
        ValueOne,
        ValueTwo
    }

    // A custom struct without a TypeConverter
    private struct TestStruct;
}