using Sigaa.Api.Common.Scraping.Builders;
using Sigaa.Api.Common.Scraping.Configuration;
using Sigaa.Api.Common.Scraping.Converters;
using Sigaa.Api.Features.Account.Models;

namespace Sigaa.Api.Features.Account.Scraping.Configurations;

[UsedImplicitly]
internal sealed partial class EnrollmentConfiguration : IScrapingModelConfiguration<Enrollment>
{
    public void Configure(ScrapingModelBuilder<Enrollment> builder)
    {
        builder.Value(e => e.Data)
            .WithSelector("a")
            .WithAttribute("onclick")
            .WithConversion(ConvertEnrollmentDataToDictionary);
    }

    private static Dictionary<string, string> ConvertEnrollmentDataToDictionary(string? input)
    {
        input ??= string.Empty;
        var matches = DataRegex()
            .Matches(input);

        if (matches.Count is 0)
        {
            throw new ScrapingConversionException($"Failed to parse enrollment data from string: {input}.");
        }

        var data = matches.ToDictionary(
            m => m.Groups[1].Value,
            m => m.Groups[2].Value);

        return data;
    }

    [GeneratedRegex(@"'([^']+)':\s*'([^']*)'")]
    private static partial Regex DataRegex();
}