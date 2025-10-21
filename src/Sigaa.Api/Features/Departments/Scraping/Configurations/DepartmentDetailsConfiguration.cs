using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Departments.Models;

namespace Sigaa.Api.Features.Departments.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class DepartmentDetailsConfiguration : IScrapingModelConfiguration<DepartmentDetails>
{
    public void Configure(ScrapingModelBuilder<DepartmentDetails> builder)
    {
        builder.Value(d => d.Code)
            .WithSelector("#logo a")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(IdRegex()));

        builder.Value(d => d.Name)
            .WithSelector("#colDirTop > h2")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(TitlecaseTransform.Instance);

        builder.Value(d => d.Acronym)
            .WithSelector("#colDirTop > h1")
            .WithTransformation(new RegexCaptureTransform(AcronymRegex()))
            .WithTransformation(UppercaseTransform.Instance);

        builder.Value(d => d.LogoUrl)
            .WithSelector("#logo img")
            .WithAttribute("src");

        builder.Value(d => d.HeadName)
            .WithSelector("#colDirCorpo > dl:not(.apresentacao) > dd:nth-child(2) > a")
            .WithTransformation(TitlecaseTransform.Instance)
            .IsOptional();

        builder.Value(d => d.HeadSiape)
            .WithSelector("#colDirCorpo > dl:not(.apresentacao) > dd:nth-child(2) > a")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(SiapeRegex()))
            .IsOptional();

        builder.Value(d => d.Presentation)
            .WithSelector("#colDirCorpo > dl.apresentacao")
            .WithTransformation(new RegexReplaceTransform(NotInformedRegex()))
            .IsOptional();

        builder.Value(d => d.AlternativeAddress)
            .WithSelector("#colDirCorpo > dl:not(.apresentacao) > dd:nth-child(6)")
            .WithTransformation(new RegexReplaceTransform(NotInformedRegex()))
            .IsOptional();
    }

    [GeneratedRegex(@"[?&]id=(\d+)")]
    private static partial Regex IdRegex();

    [GeneratedRegex(@"[?&]siape=\s*(\d+)")]
    private static partial Regex SiapeRegex();

    [GeneratedRegex(@"(^[a-zA-Z\s]+-\s*)|\s*\([a-zA-Z]+\)$")]
    private static partial Regex NameRegex();

    [GeneratedRegex(@"^\s*(?:[a-zA-Z][a-zA-Z.\-]*[a-zA-Z]|[a-zA-Z]{2})(?:\.)?\s*-\s*(.*)$")]
    private static partial Regex AcronymRegex();

    [GeneratedRegex(@"(?i)\bnão informado\b")]
    private static partial Regex NotInformedRegex();
}