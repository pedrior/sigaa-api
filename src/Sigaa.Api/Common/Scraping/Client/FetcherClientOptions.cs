using System.ComponentModel.DataAnnotations;
using Sigaa.Api.Common.Options;

namespace Sigaa.Api.Common.Scraping.Client;

internal sealed record FetcherClientOptions : IOptions
{
    public static string SectionName => "Scraping:Client";

    [Required, Url] public Uri BaseUrl { get; init; } = null!;

    public Dictionary<string, string> Headers { get; init; } = null!;
}