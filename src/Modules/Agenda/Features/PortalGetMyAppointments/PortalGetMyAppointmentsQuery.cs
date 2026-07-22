using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Agenda.Features.PortalGetMyAppointments;

/// <summary>Portail patient (ticket #39) : liste de ses propres rendez-vous, passés et à
/// venir. Le PatientId est toujours résolu côté serveur depuis le compte authentifié.</summary>
public record PortalGetMyAppointmentsQuery(Guid PatientId)
    : IQuery<Result<IReadOnlyList<PortalAppointmentDto>>>;

public record PortalAppointmentDto(
    Guid Id,
    Guid PractitionerId,
    string PractitionerName,
    string AppointmentTypeName,
    DateTimeOffset Start,
    DateTimeOffset End,
    string Status);
