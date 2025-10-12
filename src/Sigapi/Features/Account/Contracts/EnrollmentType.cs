namespace Sigapi.Features.Account.Contracts;

/// <summary>
/// Representa os tipos de matrícula disponíveis.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Members)]
internal enum EnrollmentType
{
    /// <summary>
    /// Matrícula em programa de graduação.
    /// </summary>
    Undergraduate,
    
    /// <summary>
    /// Matrícula em programa de pós-graduação.
    /// </summary>
    Postgraduate
}