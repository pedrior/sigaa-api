using Sigapi.Features.Centers.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Features.Centers.Scraping.Configurations;

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