using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Centers.Models;

namespace Sigaa.Api.Features.Centers.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class DepartmentConfiguration : IScrapingModelConfiguration<Department>
{
    public void Configure(ScrapingModelBuilder<Department> builder)
    {
        builder.WithSelector("table[class='listagem'] > tbody > tr");

        builder.Value(f => f.Id)
            .WithSelector("td > a")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(IdRegex()));

        builder.Value(f => f.Name)
            .WithSelector("td > a")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(TitlecaseTransform.Instance);
    }

    [GeneratedRegex(@"[?&]id=(\d+)")]
    private static partial Regex IdRegex();

    [GeneratedRegex(@"^(?:[A-Za-z-]+\s+-\s+)|(?:\s+\([A-Za-z-]+\))?\s+-\s+[A-Za-z-]+\.?\s*$")]
    private static partial Regex NameRegex();
}