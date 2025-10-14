using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Centers.Models;

namespace Sigaa.Api.Features.Centers.Scraping.Configurations;

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