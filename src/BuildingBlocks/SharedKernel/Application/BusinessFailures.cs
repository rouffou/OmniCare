using Mediarq.Core.Common.Results;

namespace OmniCare.SharedKernel.Application;

/// <summary>
/// Point unique de construction des échecs métier attendus (railway-oriented).
/// Les modules ne construisent jamais un Result d'échec directement : si l'API
/// Result/ResultError de Mediarq évolue, seul ce fichier change.
/// </summary>
public static class BusinessFailures
{
    public static Result<T> NotFound<T>(string message) =>
        Result.Failure<T>(ResultError.NotFound("not_found", message));

    public static Result<T> Conflict<T>(string message) =>
        Result.Failure<T>(ResultError.Conflict("conflict", message));

    // ErrorType.Validation → HTTP 400 : un échec de règle métier attendu est une
    // erreur du client, pas une erreur serveur (ErrorType.Failure serait mappé 500).
    public static Result<T> Rule<T>(string message) =>
        Result.Failure<T>(new ResultError("business_rule", message, ErrorType.Validation));
}
