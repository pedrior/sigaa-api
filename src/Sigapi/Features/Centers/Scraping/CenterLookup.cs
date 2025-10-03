using System.Collections.Immutable;

namespace Sigapi.Features.Centers.Scraping;

internal static class CenterLookup
{
    private static readonly ImmutableHashSet<(string id, string slug)> Centers =
    [
        new("1632", "centro-de-ciencias-aplicadas-e-educacao"),
        new("1614", "centro-de-ciencias-medicas"),
        new("2564", "centro-de-comunicacao-turismo-e-artes"),
        new("1852", "centro-de-energias-e-alternativas-e-renovaveis"),
        new("1383", "centro-de-educacao"),
        new("1860", "centro-de-biotecnologia"),
        new("1466", "centro-de-ciencias-agrarias"),
        new("1357", "centro-de-ciencias-da-saude"),
        new("1333", "centro-de-ciencias-exatas-e-da-natureza"),
        new("1345", "centro-de-ciencias-humanas-letras-e-artes"),
        new("1472", "centro-de-ciencias-humanas-sociais-e-agrarias"),
        new("1388", "centro-de-ciencias-juridicas"),
        new("1327", "centro-de-sociais-e-aplicadas"),
        new("1856", "centro-de-informatica"),
        new("3687", "centro-profissional-e-tecnologico-escola-tecnica-de-saude"),
        new("1374", "centro-de-tecnologia"),
        new("1580", "centro-de-tecnologia-e-desenvolvimento-regional")
    ];

    public static (string id, string slug)? FindByIdOrSlug(string idOrSlug)
    {
        var center = Centers.SingleOrDefault(c => c.id == idOrSlug || c.slug == idOrSlug);
        return center == default ? null : center;
    }
}