namespace OmniCare.SharedKernel.Application.Auditing;

/// <summary>
/// Marqueur des commandes soumises à l'audit trail (toute modification d'un dossier
/// médical ou d'une facture — exigence RGPD/eHealth, cahier des charges §5.1).
/// Le résumé d'audit ne doit JAMAIS contenir de donnée de santé en clair :
/// uniquement des identifiants techniques et le nom de l'action.
/// </summary>
public interface IAuditableRequest
{
    /// <summary>Nom métier de l'action, ex. « Patient.Register ».</summary>
    string AuditAction { get; }

    /// <summary>Identifiant de l'entité principale concernée, si connu à l'entrée.</summary>
    Guid? AuditTargetId { get; }
}
