using System.Text.RegularExpressions;
using Sigapi.Features.Departments.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Features.Departments.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class DepartmentConfiguration : IScrapingModelConfiguration<Department>
{
    public void Configure(ScrapingModelBuilder<Department> builder)
    {
        builder.WithSelector("table.listagem > tbody > tr:has(a[href*='id='])");
        
        builder.Value(d => d.Id)
            .WithSelector("a")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(IdRegex()));

        builder.Value(d => d.Name)
            .WithSelector("a")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(TitlecaseTransform.Instance);
        
        builder.Value(d => d.CenterAcronym)
            .WithSelector("a")
            .WithTransformation(new RegexCaptureTransform(AcronymRegex()))
            .WithTransformation(UppercaseTransform.Instance);
    }

    [GeneratedRegex(@"[?&]id=(\d+)")]
    private static partial Regex IdRegex();
    
    [GeneratedRegex(@"(^[a-zA-Z\s]+-\s*)|\s*\([a-zA-Z]+\)$")]
    private static partial Regex NameRegex();

    [GeneratedRegex(@"^\s*([a-zA-Z][a-zA-Z.\-]*[a-zA-Z]|[a-zA-Z]{2})(?:\.)?(?:\s|$)")]
    private static partial Regex AcronymRegex();
}