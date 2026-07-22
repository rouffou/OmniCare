namespace OmniCare.SharedKernel.Application;

/// <summary>
/// Utilisateur courant (praticien, secrétariat…) vu par les behaviors transverses.
/// Implémenté côté Api à partir des claims du token JWT (ticket #26) ; une identité
/// « system » anonyme sert de repli tant qu'aucun token n'est présenté — l'authentification
/// n'est pour l'instant pas imposée sur les endpoints existants (infrastructure seule,
/// l'activation par endpoint est un ticket suivant).
/// </summary>
public interface ICurrentUserService
{
    string UserId { get; }
    string DisplayName { get; }

    /// <summary>Rôles portés par le token (claim <c>role</c>), vide pour l'identité
    /// anonyme de repli. Cf. <see cref="Roles"/>.</summary>
    IReadOnlyCollection<string> Roles { get; }

    bool IsAuthenticated { get; }
}
