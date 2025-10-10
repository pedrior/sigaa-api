using Sigapi.Features.Account.Models;
using Sigapi.Scraping.Builders;
using Sigapi.Scraping.Configuration;
using Sigapi.Scraping.Converters;

namespace Sigapi.Features.Account.Scraping.Configurations;

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