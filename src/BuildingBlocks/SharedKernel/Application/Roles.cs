namespace OmniCare.SharedKernel.Application;

/// <summary>
/// Rôles applicatifs (cahier des charges §5.1 et §7, ticket #26) — génériques, valables
/// pour toute profession de santé (pas de rôle nommé par métier). Portés par le fournisseur
/// d'identité OIDC (claim <c>role</c>/<c>roles</c> du token), pas stockés par OmniCare.
/// </summary>
public static class Roles
{
    /// <summary>Praticien : accès complet aux données cliniques de ses patients (secret
    /// médical — cf. séparation avec Secretariat, CLAUDE.md).</summary>
    public const string Practitioner = "Practitioner";

    /// <summary>Secrétariat : données administratives uniquement (agenda, facturation),
    /// jamais les données cliniques.</summary>
    public const string Secretariat = "Secretariat";

    /// <summary>Administrateur du cabinet : gestion des utilisateurs, configuration,
    /// référentiels — pas nécessairement accès aux données cliniques.</summary>
    public const string Admin = "Admin";

    /// <summary>Patient : accès à son propre portail (Phase 3, cf. ticket #39).</summary>
    public const string Patient = "Patient";

    public static readonly IReadOnlyList<string> All = [Practitioner, Secretariat, Admin, Patient];
}
