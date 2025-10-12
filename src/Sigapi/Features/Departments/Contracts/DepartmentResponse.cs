namespace Sigapi.Features.Departments.Contracts;

/// <summary>
/// Representa os dados de um departamento.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal sealed record DepartmentResponse
{
    /// <summary>
    /// O identificador único do departamento.
    /// </summary>
    public string Id { get; init; } = null!;
    
    /// <summary>
    /// O nome do departamento.
    /// </summary>
    public string Name { get; init; } = null!;
    
    /// <summary>
    /// O <c>slug</c> (nome para URL) do centro acadêmico ao qual o departamento pertence.
    /// </summary>
    public string CenterSlug { get; init; } = null!;
    
    /// <summary>
    /// O nome do centro acadêmico ao qual o departamento pertence.
    /// </summary>
    public string CenterName { get; init; } = null!;
}
