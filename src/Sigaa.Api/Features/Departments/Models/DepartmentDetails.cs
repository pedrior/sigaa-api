using Sigaa.Api.Features.Departments.Contracts;

namespace Sigaa.Api.Features.Departments.Models;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed class DepartmentDetails
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Acronym { get; set; } = string.Empty;

    public string LogoUrl { get; set; } = string.Empty;

    public string? HeadName { get; set; }
    
    public string? HeadSiape { get; set; }

    public string? Presentation { get; set; }

    public string? AlternativeAddress { get; set; }

    public DepartmentDetailsResponse ToResponse() => new()
    {
        Code = Code,
        Name = Name,
        Acronym = Acronym,
        LogoUrl = LogoUrl,
        HeadName = HeadName,
        HeadSiape = HeadSiape,
        Presentation = Presentation,
        AlternativeAddress = AlternativeAddress
    };
}