using System.Text.RegularExpressions;
using Sigapi.Features.Centers.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Features.Centers.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class ResearchCoordinatorConfiguration : IScrapingModelConfiguration<ResearchCoordinator>
{
    public void Configure(ScrapingModelBuilder<ResearchCoordinator> builder)
    {
        builder.Value(c => c.Id)
            .WithSelector("a")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(IdRegex()));
        
        builder.Value(c => c.Name)
            .WithSelector("a")
            .WithTransformation(TitlecaseTransform.Instance);
    }

    [GeneratedRegex("siape=(\\d+)")]
    private static partial Regex IdRegex();
}