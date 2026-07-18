using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Agenda.Features.GetWaitlist;

/// <summary>Liste d'attente d'un praticien, triée par ordre d'inscription (FIFO — §4.2).</summary>
public record GetWaitlistQuery(Guid PractitionerId)
    : IQuery<Result<IReadOnlyList<WaitlistEntryDto>>>;

public record WaitlistEntryDto(
    Guid Id,
    Guid PatientId,
    Guid AppointmentTypeId,
    DateTimeOffset RequestedFrom,
    DateTimeOffset RequestedTo,
    string? Notes,
    DateTimeOffset JoinedOn);
