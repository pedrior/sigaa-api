using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Centers.Models;

namespace Sigaa.Api.Features.Centers.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class GraduateProgramConfiguration : IScrapingModelConfiguration<GraduateProgram>
{
    public void Configure(ScrapingModelBuilder<GraduateProgram> builder)
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

    [GeneratedRegex(@"(^[a-zA-Z\s]+-\s*)|\s*\([a-zA-Z]+\)$")]
    private static partial Regex NameRegex();
}