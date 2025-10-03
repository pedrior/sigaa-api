using Sigapi.Features.Centers.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Features.Centers.Scraping.Configurations;

[UsedImplicitly]
internal sealed class ResearchConfiguration : IScrapingModelConfiguration<Research>
{
    public void Configure(ScrapingModelBuilder<Research> builder)
    {
        builder.WithSelector("table.listagem > tbody > tr");

        builder.Value(r => r.Name)
            .WithSelector("td:nth-child(2)")
            .WithTransformation(TitlecaseTransform.Instance);

        builder.Object(r => r.Coordinator)
            .WithSelector("td:nth-child(3)");
    }
}