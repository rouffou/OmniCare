namespace OmniCare.SharedKernel.Application;

/// <summary>
/// Résout le <c>PatientId</c> du patient authentifié courant (portail patient, ticket
/// #39) à partir de son identité OIDC — utilisé par les endpoints portail des modules
/// Patients/Agenda/Billing pour ne jamais faire confiance à un PatientId fourni par le
/// client. <see langword="null"/> si non authentifié ou si aucune fiche patient n'est
/// liée à ce compte (cf. <c>LinkPatientPortalAccount</c>).
/// </summary>
public static class CurrentPatientResolver
{
    public static async Task<Guid?> ResolveAsync(
        this IPatientDirectory patientDirectory,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated)
            return null;
        return await patientDirectory.ResolvePatientIdByPortalUserIdAsync(currentUser.UserId, cancellationToken);
    }
}
