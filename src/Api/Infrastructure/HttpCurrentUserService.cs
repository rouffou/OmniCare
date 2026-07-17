using OmniCare.SharedKernel.Application;

namespace OmniCare.Api.Infrastructure;

/// <summary>
/// Utilisateur courant depuis le HttpContext. Tant que l'authentification forte
/// (MFA, exigence §5.1) n'est pas branchée, retombe sur un utilisateur « system »
/// — à remplacer lors de l'intégration de l'identité (eHealth IAM / OIDC).
/// </summary>
public sealed class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

    public string DisplayName => UserId;
}
