namespace Sigaa.Api.Features.Account.Contracts;

/// <summary>
/// Representa os tipos de programas acadêmicos ao quais um estudante pode estar vinculado.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal enum ProgramType
{
    /// <summary>
    /// Programa de graduação.
    /// </summary>
    Undergraduate,
    
    /// <summary>
    /// Programa de pós-graduação.
    /// </summary>
    Postgraduate
}