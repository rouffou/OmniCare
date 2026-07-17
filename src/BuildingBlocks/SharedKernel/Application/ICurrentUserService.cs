namespace OmniCare.SharedKernel.Application;

/// <summary>
/// Utilisateur courant (praticien, secrétariat…) vu par les behaviors transverses.
/// Implémenté côté Api (HttpContext) ; une implémentation « système » sert de repli
/// tant que l'authentification forte (MFA, exigence §5.1) n'est pas branchée.
/// </summary>
public interface ICurrentUserService
{
    string UserId { get; }
    string DisplayName { get; }
}
