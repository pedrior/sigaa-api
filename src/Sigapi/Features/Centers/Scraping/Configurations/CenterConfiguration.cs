using System.Text.RegularExpressions;
using Sigapi.Features.Centers.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Transformations;

namespace Sigapi.Features.Centers.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class CenterConfiguration : IScrapingModelConfiguration<Center>
{
    public void Configure(ScrapingModelBuilder<Center> builder)
    {
        builder.WithSelector("form > table.listagem:not(.unidade)");

        builder.Value(f => f.Id)
            .WithSelector("a[class='iconeCentro']")
            .WithAttribute("href")
            .WithTransformation(new RegexCaptureTransform(IdRegex()));

        builder.Value(f => f.Slug)
            .WithSelector("a[class='nomeCentro']")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(SlugTransform.Instance);

        builder.Value(f => f.Name)
            .WithSelector("a[class='nomeCentro']")
            .WithTransformation(new RegexReplaceTransform(NameRegex()))
            .WithTransformation(TitlecaseTransform.Instance);
        
        builder.Value(f => f.Acronym)
            .WithSelector("a[class='nomeCentro']")
            .WithTransformation(new RegexCaptureTransform(AcronymRegex(), "sn"))
            .WithTransformation(UppercaseTransform.Instance);
    }

    [GeneratedRegex(@"[?&]id=(\d+)")]
    private static partial Regex IdRegex();
    
    [GeneratedRegex(@"^(?:[A-Za-z-]+\s+-\s+)|(?:\s+\([A-Za-z-]+\))?\s+-\s+[A-Za-z-]+\.?\s*$")]
    private static partial Regex NameRegex();

    [GeneratedRegex(
        @"^\s*(?<sn>[A-Z-]+)\s*-\s*(.*?)\s*-\s*\k<sn>\s*\.?$|^\s*(.*?)\s*(?:\([A-Z-]+\))?\s*-\s*(?<sn>[A-Z-]+)\s*\.?$")]
    private static partial Regex AcronymRegex();
}