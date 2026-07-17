using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Domain.ValueObjects;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.RegisterPatient;

public class RegisterPatientHandler : ICommandHandler<RegisterPatientCommand, Result<Guid>>
{
    private readonly IPatientsDbContext _context;

    public RegisterPatientHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(RegisterPatientCommand request, CancellationToken cancellationToken = default)
    {
        NationalRegistryNumber? niss;
        try
        {
            niss = string.IsNullOrWhiteSpace(request.NationalRegistryNumber)
                ? null
                : NationalRegistryNumber.Create(request.NationalRegistryNumber);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        if (niss is not null &&
            await _context.Patients.AnyAsync(p => p.NationalRegistryNumber == niss, cancellationToken))
        {
            return BusinessFailures.Conflict<Guid>(
                "Un patient avec ce numéro de registre national existe déjà.");
        }

        var patient = Patient.Register(
            PersonName.Create(request.FirstName, request.LastName),
            niss,
            request.BirthDate,
            ContactDetails.Create(request.Email, request.Phone, request.AddressLine, request.PostalCode, request.City),
            string.IsNullOrWhiteSpace(request.MutualityCode)
                ? null
                : MutualityAffiliation.Create(request.MutualityCode, request.MutualityMemberNumber, request.HasPreferentialRate),
            request.TreatingPhysicianName,
            request.EmergencyContact,
            request.ReferentPractitionerId);

        // Consentement de base : sans lui, aucun traitement de données de santé n'est licite.
        patient.GrantConsent(ConsentType.HealthDataProcessing);

        _context.Patients.Add(patient);

        return Result.Success(patient.Id);
    }
}
