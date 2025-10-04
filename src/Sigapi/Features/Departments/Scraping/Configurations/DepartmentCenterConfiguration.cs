using System.Text.RegularExpressions;
using Sigapi.Features.Departments.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Features.Departments.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class DepartmentCenterConfiguration : IScrapingModelConfiguration<DepartmentCenter>
{
    public void Configure(ScrapingModelBuilder<DepartmentCenter> builder)
    {
        builder.WithSelector("table.listagem > tbody > tr:has(td.subListagem)");

        builder.Value(g => g.Name)
            .WithSelector("span.departamento")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(TitlecaseTransform.Instance);

        builder.Value(g => g.Slug)
            .WithSelector("span.departamento")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(SlugTransform.Instance);

        // Some center rows don't have an acronym, so we need to get it from the
        // next row (which is the first department row). Every department row is
        // prefixed by its center acronym. Another alternative would apply a 
        // transformation function to create the acronym based on the name.
        builder.Value(g => g.Acronym)
            .WithSelector("tr")
            .WithTransformation(new RegexCaptureTransform(AcronymRegex()))
            .WithTransformation(UppercaseTransform.Instance)
            .FromSibling();
    }

    [GeneratedRegex(@"(^[a-zA-Z\s]+-\s*)|\s*\([a-zA-Z]+\)$")]
    private static partial Regex NameRegex();

    [GeneratedRegex(@"^\s*([a-zA-Z][a-zA-Z.\-]*[a-zA-Z]|[a-zA-Z]{2})(?:\.)?(?:\s|$)")]
    private static partial Regex AcronymRegex();
}