using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.SharedKernel.Application.Behaviors;

/// <summary>
/// Behavior custom du pipeline Mediarq (cf. architecture_technique_saas.md §5) :
/// journalise toute requête marquée <see cref="IAuditableRequest"/> dans le store
/// d'audit, succès comme échec. Ordre 10 : s'exécute après logging et validation,
/// autour du handler et de la transaction.
/// </summary>
public sealed class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditTrailStore _auditTrail;

    public AuditBehavior(ICurrentUserService currentUser, IAuditTrailStore auditTrail)
    {
        _currentUser = currentUser;
        _auditTrail = auditTrail;
    }

    public int Order => 10;

    public async Task<TResponse> Handle(
        IMutableRequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handle,
        CancellationToken cancellationToken = default)
    {
        if (context.Request is not IAuditableRequest auditable)
            return await handle();

        var occurredOn = DateTimeOffset.UtcNow;
        try
        {
            var response = await handle();
            var (succeeded, failureReason) = Describe(response);
            await Append(auditable, occurredOn, succeeded, failureReason, cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            await Append(auditable, occurredOn, succeeded: false, ex.GetType().Name, cancellationToken);
            throw;
        }
    }

    private Task Append(
        IAuditableRequest auditable,
        DateTimeOffset occurredOn,
        bool succeeded,
        string? failureReason,
        CancellationToken cancellationToken) =>
        _auditTrail.AppendAsync(new AuditRecord
        {
            Action = auditable.AuditAction,
            RequestType = typeof(TRequest).Name,
            TargetId = auditable.AuditTargetId,
            UserId = _currentUser.UserId,
            OccurredOn = occurredOn,
            Succeeded = succeeded,
            FailureReason = failureReason,
        }, cancellationToken);

    private static (bool Succeeded, string? FailureReason) Describe(TResponse response) =>
        response is Result { IsFailure: true } result
            ? (false, $"{result.Error.Code}: {result.Error.Message}")
            : (true, null);
}
