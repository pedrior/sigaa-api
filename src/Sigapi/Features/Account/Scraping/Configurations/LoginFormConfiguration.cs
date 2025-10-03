using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;

namespace Sigapi.Features.Account.Scraping.Configurations;

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