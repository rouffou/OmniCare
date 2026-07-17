using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniCare.Modules.Patients.Features.ArchivePatient;
using OmniCare.Modules.Patients.Features.GetClinicalRecord;
using OmniCare.Modules.Patients.Features.GetPatientById;
using OmniCare.Modules.Patients.Features.GrantConsent;
using OmniCare.Modules.Patients.Features.RecordClinicalEntry;
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
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<PatientsDbContext>(configureDb);
        services.AddScoped<IPatientsDbContext>(sp => sp.GetRequiredService<PatientsDbContext>());
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
        return app;
    }
}
