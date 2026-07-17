using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Domain.Events;
using OmniCare.Modules.Patients.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using Xunit;

namespace OmniCare.UnitTests.Patients;

public class PatientTests
{
    private static Patient NewPatient() => Patient.Register(
        PersonName.Create("Marie", "Dupont"),
        nationalRegistryNumber: null,
        birthDate: new DateOnly(1985, 1, 7),
        ContactDetails.Create(email: "marie@example.be"));

    [Fact]
    public void Register_raises_PatientRegisteredEvent()
    {
        var patient = NewPatient();
        var domainEvent = Assert.Single(patient.DomainEvents);
        var registered = Assert.IsType<PatientRegisteredEvent>(domainEvent);
        Assert.Equal(patient.Id, registered.PatientId);
        Assert.Equal(PatientStatus.Active, patient.Status);
    }

    [Fact]
    public void GrantConsent_is_idempotent_while_active()
    {
        var patient = NewPatient();
        patient.GrantConsent(ConsentType.HealthDataProcessing);
        patient.GrantConsent(ConsentType.HealthDataProcessing);
        Assert.Single(patient.Consents);
    }

    [Fact]
    public void RevokeConsent_keeps_history_and_allows_regrant()
    {
        var patient = NewPatient();
        patient.GrantConsent(ConsentType.ElectronicCommunication);
        patient.RevokeConsent(ConsentType.ElectronicCommunication);
        patient.GrantConsent(ConsentType.ElectronicCommunication);

        Assert.Equal(2, patient.Consents.Count);
        Assert.Single(patient.Consents, c => c.IsActive);
    }

    [Fact]
    public void RevokeConsent_without_active_consent_fails()
    {
        var patient = NewPatient();
        Assert.Throws<DomainException>(() => patient.RevokeConsent(ConsentType.SharingWithCareCircle));
    }

    [Fact]
    public void UpdateMutuality_resets_insurability_status()
    {
        var patient = NewPatient();
        patient.RecordInsurabilityCheck(insured: true, new DateOnly(2026, 7, 1));
        Assert.Equal(InsurabilityState.Insured, patient.Insurability.State);

        patient.UpdateMutuality(MutualityAffiliation.Create("509"));
        Assert.Equal(InsurabilityState.Unknown, patient.Insurability.State);
    }

    [Fact]
    public void Archived_patient_rejects_modifications()
    {
        var patient = NewPatient();
        patient.Archive();

        Assert.Equal(PatientStatus.Archived, patient.Status);
        Assert.Throws<DomainException>(() => patient.UpdateContactDetails(ContactDetails.Empty));
        Assert.Throws<DomainException>(() => patient.GrantConsent(ConsentType.ElectronicCommunication));
    }

    [Fact]
    public void Archive_is_idempotent()
    {
        var patient = NewPatient();
        patient.Archive();
        patient.Archive();
        Assert.Equal(PatientStatus.Archived, patient.Status);
    }
}
