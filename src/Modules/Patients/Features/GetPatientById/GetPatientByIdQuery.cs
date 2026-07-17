using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Patients.Features.GetPatientById;

/// <summary>Lecture de la fiche administrative d'un patient (vue secrétariat).</summary>
public record GetPatientByIdQuery(Guid PatientId) : IQuery<Result<PatientDto>>;
