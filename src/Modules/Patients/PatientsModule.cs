using System.Security.Cryptography;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniCare.Modules.Patients.Infrastructure;
using OmniCare.Modules.Patients.Infrastructure.Storage;
using OmniCare.SharedKernel.Application;
using OmniCare.Modules.Patients.Features.ArchivePatient;
using OmniCare.Modules.Patients.Features.DownloadClinicalDocument;
using OmniCare.Modules.Patients.Features.GetClinicalRecord;
using OmniCare.Modules.Patients.Features.GetPatientById;
using OmniCare.Modules.Patients.Features.GrantConsent;
using OmniCare.Modules.Patients.Features.ListClinicalDocuments;
using OmniCare.Modules.Patients.Features.RecordClinicalEntry;
using OmniCare.Modules.Patients.Features.RegisterClinicalDocument;
using OmniCare.Modules.Patients.Features.RegisterPatient;
using OmniCare.Modules.Patients.Features.RegisterPrescription;
using OmniCare.Modules.Patients.Features.RevokeConsent;
using OmniCare.Modules.Patients.Features.SearchPatients;
using OmniCare.Modules.Patients.Features.UpdatePatientContact;
using OmniCare.Modules.Patients.Infrastructure.Persistence;

namespace OmniCare.Modules.Patients;

/// <summary>Composition root du module : persistance + endpoints des slices.</summary>
public static class PatientsModule
{
    public static IServiceCollection AddPatientsModule(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb,
        IConfiguration configuration)
    {
        services.AddDbContext<PatientsDbContext>(configureDb);
        services.AddScoped<IPatientsDbContext>(sp => sp.GetRequiredService<PatientsDbContext>());
        // Contrat public consommé par Agenda/Billing sans dépendance au Domain Patients.
        services.AddScoped<IPatientDirectory, PatientDirectory>();

        // Stockage des documents joints (ticket #31) — chiffré en local en attendant le
        // choix d'un hébergeur certifié données de santé (§5.2), cf. IDocumentStorage.
        var storageRoot = configuration["DocumentStorage:RootPath"] ?? "clinical-documents";
        var keyBase64 = configuration["DocumentStorage:EncryptionKeyBase64"];
        services.AddSingleton<IDocumentStorage>(sp =>
        {
            byte[] encryptionKey;
            if (string.IsNullOrWhiteSpace(keyBase64))
            {
                encryptionKey = RandomNumberGenerator.GetBytes(32);
                sp.GetRequiredService<ILoggerFactory>().CreateLogger("DocumentStorage").LogWarning(
                    "DocumentStorage:EncryptionKeyBase64 non configurée — clé éphémère générée en " +
                    "mémoire (documents illisibles après redémarrage). À configurer via un coffre-fort " +
                    "de secrets avant toute mise en production.");
            }
            else
            {
                encryptionKey = Convert.FromBase64String(keyBase64);
            }
            return new EncryptedLocalFileStorage(storageRoot, encryptionKey);
        });

        return services;
    }

    public static IEndpointRouteBuilder MapPatientsModule(this IEndpointRouteBuilder app)
    {
        app.MapRegisterPatient();
        app.MapGetPatientById();
        app.MapSearchPatients();
        app.MapUpdatePatientContact();
        app.MapArchivePatient();
        app.MapGrantConsent();
        app.MapRevokeConsent();
        app.MapRecordClinicalEntry();
        app.MapRegisterPrescription();
        app.MapGetClinicalRecord();
        app.MapRegisterClinicalDocument();
        app.MapListClinicalDocuments();
        app.MapDownloadClinicalDocument();
        return app;
    }
}
