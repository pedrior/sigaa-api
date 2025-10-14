using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping.Configurations;

[UsedImplicitly]
internal sealed class LoginFormConfiguration : IScrapingModelConfiguration<LoginForm>
{
    public void Configure(ScrapingModelBuilder<LoginForm> builder)
    {
        builder.Value(l => l.Action)
            .WithSelector("#form")
            .WithAttribute("action");

        builder.Dictionary(l => l.Data)
            .WithSelector("#form input")
            .WithKeyAttribute("name")
            .WithValueAttribute("value");
    }
}