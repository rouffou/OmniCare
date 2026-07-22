using System.Security.Claims;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Api.Infrastructure;

/// <summary>
/// Utilisateur courant depuis les claims du token JWT (ticket #26). Tant qu'aucun
/// fournisseur d'identité réel n'est configuré (Authority vide) ou qu'aucun token
/// n'est présenté, retombe sur une identité « system » anonyme sans rôle.
/// </summary>
public sealed class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string UserId =>
        (IsAuthenticated ? User!.FindFirstValue(ClaimTypes.NameIdentifier) : null) ?? "system";

    public string DisplayName =>
        (IsAuthenticated ? User!.FindFirstValue(ClaimTypes.Name) : null) ?? UserId;

    public IReadOnlyCollection<string> Roles =>
        IsAuthenticated
            ? User!.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
            : [];
}
