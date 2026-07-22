using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.Modules.Patients.Infrastructure.Storage;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.RegisterClinicalDocument;

public class RegisterClinicalDocumentHandler : ICommandHandler<RegisterClinicalDocumentCommand, Result<Guid>>
{
    private readonly IPatientsDbContext _context;
    private readonly IDocumentStorage _storage;

    public RegisterClinicalDocumentHandler(IPatientsDbContext context, IDocumentStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<Result<Guid>> Handle(RegisterClinicalDocumentCommand request, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient is null)
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");
        if (patient.Status != PatientStatus.Active)
            return BusinessFailures.Rule<Guid>("Impossible d'ajouter un document au dossier d'un patient archivé.");

        byte[] content;
        try
        {
            content = Convert.FromBase64String(request.ContentBase64);
        }
        catch (FormatException)
        {
            return BusinessFailures.Rule<Guid>("Le contenu du document n'est pas encodé en base64 valide.");
        }
        if (content.Length == 0)
            return BusinessFailures.Rule<Guid>("Un document joint ne peut pas être vide.");

        var profession = HealthProfession.FromCode(request.ProfessionCode);
        var documentType = Enum.Parse<ClinicalDocumentType>(request.DocumentType, ignoreCase: true);

        var record = await _context.ClinicalRecords
            .Include(r => r.Documents)
            .FirstOrDefaultAsync(
                r => r.PatientId == request.PatientId && r.Profession == profession,
                cancellationToken);
        if (record is null)
        {
            record = ClinicalRecord.Open(request.PatientId, profession);
            _context.ClinicalRecords.Add(record);
        }

        var storageKey = await _storage.SaveAsync(content, cancellationToken);

        var document = record.AddDocument(
            documentType, request.FileName, request.ContentType, content.Length, storageKey,
            request.UploadedByPractitionerId);

        return Result.Success(document.Id);
    }
}
