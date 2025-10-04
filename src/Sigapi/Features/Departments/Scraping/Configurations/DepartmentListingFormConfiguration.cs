using Sigapi.Features.Departments.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;

namespace Sigapi.Features.Departments.Scraping.Configurations;

[UsedImplicitly]
internal sealed class DepartmentListingFormConfiguration : IScrapingModelConfiguration<DepartmentListingForm>
{
    public void Configure(ScrapingModelBuilder<DepartmentListingForm> builder)
    {
        builder.Value(d => d.Action)
            .WithSelector("#form")
            .WithAttribute("action");

        builder.Dictionary(d => d.Data)
            .WithSelector("#form input")
            .WithKeyAttribute("name")
            .WithValueAttribute("value");
    }
}