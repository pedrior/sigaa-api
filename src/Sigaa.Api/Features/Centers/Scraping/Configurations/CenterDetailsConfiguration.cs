using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Centers.Models;

namespace Sigaa.Api.Features.Centers.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class CenterDetailsConfiguration : IScrapingModelConfiguration<CenterDetails>
{
    public void Configure(ScrapingModelBuilder<CenterDetails> builder)
    {
        builder.Value(f => f.Name)
            .WithSelector("#colDirTop > h2")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(TitlecaseTransform.Instance);
        
        builder.Value(f => f.Acronym)
            .WithSelector("#colDirTop > h1")
            .WithTransformation(new RegexCaptureTransform(AcronymRegex()))
            .WithTransformation(UppercaseTransform.Instance)
            .IsOptional();

        builder.Value(f => f.Address)
            .WithSelector("#colDirCorpo > dl:not(.apresentacao) > dd:nth-child(6)")
            .WithTransformation(new RegexReplaceTransform(NotInformedRegex()))
            .IsOptional();

        builder.Value(f => f.Director)
            .WithSelector("#colDirCorpo > dl:not(.apresentacao) > dd:nth-child(2)")
            .WithTransformation(new RegexReplaceTransform(NotInformedRegex()))
            .IsOptional();

        builder.Value(f => f.Description)
            .WithSelector("#colDirCorpo > dl.apresentacao")
            .WithTransformation(new RegexReplaceTransform(NotInformedRegex()))
            .IsOptional();

        builder.Value(f => f.LogoUrl)
            .WithSelector("#logo > p > span > a > img, #logo > p > span > img")
            .WithAttribute("src");
    }

    [GeneratedRegex(@"(^[a-zA-Z\s]+-\s*)|\s*\([a-zA-Z]+\)$")]
    private static partial Regex NameRegex();

    [GeneratedRegex(@"^\s*([a-zA-Z][a-zA-Z.\-]*[a-zA-Z]|[a-zA-Z]{2})(?:\.)?(?:\s|$)")]
    private static partial Regex AcronymRegex();

    [GeneratedRegex(@"(?i)\bnão informado\b")]
    private static partial Regex NotInformedRegex();
}