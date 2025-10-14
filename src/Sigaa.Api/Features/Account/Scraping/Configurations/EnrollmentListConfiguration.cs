using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping.Configurations;

[UsedImplicitly]
internal sealed class EnrollmentListConfiguration : IScrapingModelConfiguration<UserEnrollments>
{
    public void Configure(ScrapingModelBuilder<UserEnrollments> builder)
    {
        builder.Collection(e => e.Active)
            .WithSelector("section.subformulario:not(#lista-vinculos-inativos) > div")
            .IsOptional();
        
        builder.Collection(e => e.Inactive)
            .WithSelector("#lista-vinculos-inativos > div")
            .IsOptional();

        builder.Dictionary(e => e.Data)
            .WithSelector("form[action*='vinculos.jsf'] input")
            .WithKeyAttribute("name")
            .WithValueAttribute("value");
    }
}