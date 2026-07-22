namespace OmniCare.Modules.Patients.Infrastructure.Storage;

/// <summary>
/// Abstraction du stockage de contenu binaire (documents joints, ticket #31),
/// indépendante du fournisseur d'hébergement — un hébergeur certifié données de santé
/// n'a pas encore été retenu (cahier des charges §5.2). L'implémentation locale
/// (<see cref="EncryptedLocalFileStorage"/>) sert au développement uniquement ; une
/// implémentation adossée à l'hébergeur choisi se substituera via ce même contrat sans
/// changer les handlers qui en dépendent.
/// </summary>
public interface IDocumentStorage
{
    /// <summary>Chiffre et persiste le contenu, retourne une clé opaque à conserver
    /// pour la relecture (<see cref="ReadAsync"/>) — jamais le chemin/URL brut.</summary>
    Task<string> SaveAsync(byte[] content, CancellationToken cancellationToken = default);

    Task<byte[]> ReadAsync(string storageKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
