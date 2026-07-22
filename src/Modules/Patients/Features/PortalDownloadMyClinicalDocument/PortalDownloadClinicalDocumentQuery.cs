using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using OmniCare.Modules.Patients.Features.DownloadClinicalDocument;

namespace OmniCare.Modules.Patients.Features.PortalDownloadMyClinicalDocument;

/// <summary>
/// Téléchargement d'un document par le patient lui-même (portail, ticket #39) — contrairement
/// à <c>DownloadClinicalDocumentQuery</c> (accès praticien/secrétariat, aucun scoping), vérifie
/// que le document appartient bien au dossier du patient authentifié avant de le déchiffrer.
/// </summary>
public record PortalDownloadClinicalDocumentQuery(Guid DocumentId, Guid PatientId)
    : IQuery<Result<DownloadedDocument>>;
