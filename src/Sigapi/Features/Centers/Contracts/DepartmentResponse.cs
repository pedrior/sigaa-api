namespace Sigapi.Features.Centers.Contracts;

/// <summary>
/// Representa os dados de um departamento dentro de um centro acadêmico.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record DepartmentResponse
{
    /// <summary>
    /// O identificador único do departamento.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// O nome do departamento.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}
