using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Departments.Models;

namespace Sigaa.Api.Features.Departments.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class DepartmentEntryConfiguration : IScrapingModelConfiguration<DepartmentEntry>
{
    public void Configure(ScrapingModelBuilder<DepartmentEntry> builder)
    {
        builder.WithSelector("table.listagem > tbody > tr:has(a[href*='id='])");

        builder.Value(d => d.Code)
            .WithSelector("a")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(CodeRegex()));
    }

    [GeneratedRegex(@"[?&]id=(\d+)")]
    private static partial Regex CodeRegex();
}