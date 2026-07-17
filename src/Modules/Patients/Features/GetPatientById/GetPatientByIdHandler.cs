using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Features.GetPatientById;

public class GetPatientByIdHandler : IQueryHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IPatientsDbContext _context;

    public GetPatientByIdHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .Include(p => p.Consents)
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);

        if (patient is null)
            return BusinessFailures.NotFound<PatientDto>($"Patient {request.PatientId} introuvable.");

        var dto = new PatientDto(
            patient.Id,
            patient.Name.FirstName,
            patient.Name.LastName,
            patient.NationalRegistryNumber?.Masked,
            patient.BirthDate,
            patient.Contact.Email,
            patient.Contact.Phone,
            patient.Contact.AddressLine,
            patient.Contact.PostalCode,
            patient.Contact.City,
            patient.Mutuality?.MutualityCode,
            patient.Mutuality?.HasPreferentialRate ?? false,
            patient.Insurability.State.ToString(),
            patient.Insurability.LastCheckedOn,
            patient.TreatingPhysicianName,
            patient.EmergencyContact,
            patient.ReferentPractitionerId,
            patient.Status.ToString(),
            patient.Consents
                .Select(c => new ConsentDto(c.Type.ToString(), c.GrantedOn, c.RevokedOn))
                .ToList());

        return Result.Success(dto);
    }
}
