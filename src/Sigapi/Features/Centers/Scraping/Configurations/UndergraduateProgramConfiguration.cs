using Sigapi.Features.Centers.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Features.Centers.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class UndergraduateProgramConfiguration 
    : IScrapingModelConfiguration<UndergraduateProgram>
{
    public void Configure(ScrapingModelBuilder<UndergraduateProgram> builder)
    {
        builder.WithSelector("table[class='listagem'] > tbody > tr");

        builder.Value(f => f.Id)
            .WithSelector("td:last-child > a")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(IdRegex()));

        builder.Value(f => f.Name)
            .WithSelector("td")
            .WithTransformation(new RegexCaptureTransform(NameRegex()))
            .WithTransformation(TitlecaseTransform.Instance);

        builder.Value(f => f.City)
            .WithSelector("td:nth-child(2)")
            .WithTransformation(TitlecaseTransform.Instance);

        builder.Value(f => f.Coordinator)
            .WithSelector("td:nth-child(4)")
            .WithTransformation(TitlecaseTransform.Instance)
            .IsOptional();

        builder.Value(f => f.IsOnsite)
            .WithSelector("td:nth-child(3):contains('Presencial')")
            .WithConversion(v => !string.IsNullOrEmpty(v))
            .WithDefaultValue(false)
            .IsOptional();
    }
    
    [GeneratedRegex(@"[?&]id=(\d+)")]
    private static partial Regex IdRegex();

    [GeneratedRegex(@"^(.+?)(?=\s*\/?\s*-|/)")]
    private static partial Regex NameRegex();
}