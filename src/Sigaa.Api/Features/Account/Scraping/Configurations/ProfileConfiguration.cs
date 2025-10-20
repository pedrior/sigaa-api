using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Transformations;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping.Configurations;

[UsedImplicitly]
internal sealed class ProfileConfiguration : IScrapingModelConfiguration<Profile>
{
    public void Configure(ScrapingModelBuilder<Profile> builder)
    {
        builder.Value(s => s.Name)
            .WithSelector("td.detalhes-usuario-nome > span")
            .WithTransformation(TitlecaseTransform.Instance);

        builder.Value(s => s.Email)
            .WithSelector("#detalhes-usuario tbody > tr:last-child > td:last-child")
            .WithTransformation(LowercaseTransform.Instance);

        builder.Value(s => s.Enrollment)
            .WithSelector("td.detalhes-usuario-matricula > small");

        builder.Value(s => s.IsProgramCompletionAvailable)
            .WithSelector("a[href*='integralizacao']")
            .WithConversion(s => !string.IsNullOrEmpty(s))
            .IsOptional();

        builder.Value(s => s.PhotoUrl)
            .WithSelector("img[class*='fotoPerfil']")
            .WithAttribute("src")
            .IsOptional();

        builder.Value(s => s.Biography)
            .WithSelector("textarea[id*='descricaoPessoal']")
            .IsOptional();

        builder.Value(s => s.Interests)
            .WithSelector("textarea[id*='areasInteresse']")
            .IsOptional();

        builder.Value(s => s.Curriculum)
            .WithSelector("input[id*='curriculoLattes']")
            .IsOptional();
    }
}