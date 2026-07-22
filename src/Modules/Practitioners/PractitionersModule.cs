using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniCare.Modules.Practitioners.Features.GetCabinetById;
using OmniCare.Modules.Practitioners.Features.GetPractitionerById;
using OmniCare.Modules.Practitioners.Features.RegisterCabinet;
using OmniCare.Modules.Practitioners.Features.RegisterPractitioner;
using OmniCare.Modules.Practitioners.Infrastructure;
using OmniCare.Modules.Practitioners.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Practitioners;

/// <summary>Composition root du module : persistance, contrat inter-modules, endpoints.</summary>
public static class PractitionersModule
{
    public static IServiceCollection AddPractitionersModule(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<PractitionersDbContext>(configureDb);
        services.AddScoped<IPractitionersDbContext>(sp => sp.GetRequiredService<PractitionersDbContext>());
        services.AddScoped<IPractitionerDirectory, PractitionerDirectory>();
        return services;
    }

    public static IEndpointRouteBuilder MapPractitionersModule(this IEndpointRouteBuilder app)
    {
        app.MapRegisterCabinet();
        app.MapRegisterPractitioner();
        app.MapGetCabinetById();
        app.MapGetPractitionerById();
        return app;
    }
}
