using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Features.Departments.Models;

namespace Sigaa.Api.Features.Departments.Scraping.Configurations;

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