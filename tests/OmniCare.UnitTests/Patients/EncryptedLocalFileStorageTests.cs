using System.Security.Cryptography;
using System.Text;
using OmniCare.Modules.Patients.Infrastructure.Storage;
using Xunit;

namespace OmniCare.UnitTests.Patients;

public class EncryptedLocalFileStorageTests : IDisposable
{
    private readonly string _rootPath = Path.Combine(Path.GetTempPath(), "omnicare-doc-storage-tests-" + Guid.NewGuid());

    private EncryptedLocalFileStorage NewStorage(byte[]? key = null) =>
        new(_rootPath, key ?? RandomNumberGenerator.GetBytes(32));

    [Fact]
    public async Task SaveAsync_then_ReadAsync_round_trips_content()
    {
        var storage = NewStorage();
        var content = Encoding.UTF8.GetBytes("Contenu de test — données de santé sensibles.");

        var storageKey = await storage.SaveAsync(content);
        var read = await storage.ReadAsync(storageKey);

        Assert.Equal(content, read);
    }

    [Fact]
    public async Task SaveAsync_writes_ciphertext_not_plaintext_to_disk()
    {
        var storage = NewStorage();
        var content = Encoding.UTF8.GetBytes("Donnée strictement confidentielle.");

        var storageKey = await storage.SaveAsync(content);
        var onDisk = await File.ReadAllBytesAsync(Path.Combine(_rootPath, storageKey));

        var containsPlaintext = Chunk(onDisk, content.Length).Any(chunk => chunk.SequenceEqual(content));
        Assert.False(containsPlaintext, "Le contenu en clair ne doit jamais apparaître dans le fichier sur disque.");
    }

    [Fact]
    public async Task ReadAsync_fails_when_a_different_key_is_used()
    {
        var writer = NewStorage();
        var content = Encoding.UTF8.GetBytes("Contenu chiffré avec une clé A.");
        var storageKey = await writer.SaveAsync(content);

        var readerWithWrongKey = NewStorage(); // clé B, générée séparément
        await Assert.ThrowsAnyAsync<CryptographicException>(() => readerWithWrongKey.ReadAsync(storageKey));
    }

    [Fact]
    public void Constructor_rejects_key_of_wrong_size()
    {
        Assert.Throws<ArgumentException>(() => new EncryptedLocalFileStorage(_rootPath, new byte[16]));
    }

    [Fact]
    public async Task DeleteAsync_removes_the_file()
    {
        var storage = NewStorage();
        var storageKey = await storage.SaveAsync(Encoding.UTF8.GetBytes("à supprimer"));

        await storage.DeleteAsync(storageKey);

        await Assert.ThrowsAsync<FileNotFoundException>(() => storage.ReadAsync(storageKey));
    }

    private static IEnumerable<byte[]> Chunk(byte[] source, int length)
    {
        for (var i = 0; i + length <= source.Length; i++)
            yield return source[i..(i + length)];
    }

    public void Dispose()
    {
        if (Directory.Exists(_rootPath))
            Directory.Delete(_rootPath, recursive: true);
    }
}
