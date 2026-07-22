using System.Security.Cryptography;

namespace OmniCare.Modules.Patients.Infrastructure.Storage;

/// <summary>
/// Stockage local sur disque, chiffré en AES-256-GCM avant écriture (défense en
/// profondeur — protège le contenu même si le disque/futur hébergeur est compromis,
/// indépendamment du chiffrement au repos qu'offrira l'hébergeur certifié retenu).
/// Développement uniquement : à remplacer par une implémentation adossée à un
/// hébergement certifié données de santé avant toute mise en production (§5.2).
/// </summary>
internal sealed class EncryptedLocalFileStorage : IDocumentStorage
{
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;

    private readonly string _rootPath;
    private readonly byte[] _key;

    public EncryptedLocalFileStorage(string rootPath, byte[] encryptionKey)
    {
        if (encryptionKey.Length != 32)
            throw new ArgumentException(
                "La clé de chiffrement doit faire 256 bits (32 octets).", nameof(encryptionKey));

        _rootPath = rootPath;
        _key = encryptionKey;
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(byte[] content, CancellationToken cancellationToken = default)
    {
        var storageKey = Guid.CreateVersion7().ToString("N");
        var nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);
        var ciphertext = new byte[content.Length];
        var tag = new byte[TagSizeBytes];

        using (var aes = new AesGcm(_key, TagSizeBytes))
        {
            aes.Encrypt(nonce, content, ciphertext, tag);
        }

        // Format sur disque : nonce (12) + tag (16) + texte chiffré.
        var payload = new byte[NonceSizeBytes + TagSizeBytes + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSizeBytes);
        Buffer.BlockCopy(tag, 0, payload, NonceSizeBytes, TagSizeBytes);
        Buffer.BlockCopy(ciphertext, 0, payload, NonceSizeBytes + TagSizeBytes, ciphertext.Length);

        await File.WriteAllBytesAsync(PathFor(storageKey), payload, cancellationToken);
        return storageKey;
    }

    public async Task<byte[]> ReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var payload = await File.ReadAllBytesAsync(PathFor(storageKey), cancellationToken);

        var nonce = payload[..NonceSizeBytes];
        var tag = payload[NonceSizeBytes..(NonceSizeBytes + TagSizeBytes)];
        var ciphertext = payload[(NonceSizeBytes + TagSizeBytes)..];

        var plaintext = new byte[ciphertext.Length];
        using var aes = new AesGcm(_key, TagSizeBytes);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var path = PathFor(storageKey);
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    private string PathFor(string storageKey) => Path.Combine(_rootPath, storageKey);
}
