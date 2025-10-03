using Sigapi.Scraping.Transformations;

namespace Sigapi.UnitTests.Scraping.Transformations;

[TestSubject(typeof(TitlecaseTransform))]
public sealed class TitlecaseTransformTest
{
    [Theory]
    [InlineData("centro de informatica", "Centro de Informatica")]
    [InlineData("joao e maria foram a feira", "Joao e Maria Foram a Feira")]
    [InlineData("UM TITULO EM CAIXA ALTA", "Um Titulo em Caixa Alta")]
    [InlineData("guerra e paz, livro I", "Guerra e Paz, Livro I")]
    [InlineData("guerra e paz, livro ix", "Guerra e Paz, Livro IX")]
    [InlineData("capitulo-dois", "Capitulo-Dois")]
    public void Transform_ShouldApplyTitleCasingCorrectly(string input, string expected)
    {
        // Arrange
        var transform = TitlecaseTransform.Instance;
            
        // Act
        var result = transform.Transform(input);
            
        // Assert
        result.Should().Be(expected);
    }
}