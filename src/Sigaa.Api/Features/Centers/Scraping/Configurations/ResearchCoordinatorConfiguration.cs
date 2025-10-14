using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Centers.Models;

namespace Sigaa.Api.Features.Centers.Scraping.Configurations;

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