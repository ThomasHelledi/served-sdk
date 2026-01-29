using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Users;
using Served.SDK.Models.Bootstrap;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for bootstrap/initialization resources.
/// </summary>
public class BootstrapApi : ApiModuleBase
{
    protected override string ModulePath => "bootstrap";

    private readonly IHttpClient _http;

    public BootstrapApi(IHttpClient http) : base(http)
    {
        _http = http;
    }

    /// <summary>
    /// Gets the current user's bootstrap data including tenant and workspace information.
    /// </summary>
    public Task<UserBootstrapViewModel> GetUserAsync()
    {
        return _http.GetAsync<UserBootstrapViewModel>("api/core/bootstrap/user");
    }

    /// <summary>
    /// Gets bootstrap data for a specific tenant.
    /// </summary>
    public Task<TenantBootstrapViewModel> GetTenantAsync(string tenantSlug)
    {
        return _http.GetAsync<TenantBootstrapViewModel>($"api/core/bootstrap/tenant/{tenantSlug}");
    }

    /// <summary>
    /// Gets bootstrap data for a specific workspace.
    /// </summary>
    public Task<WorkspaceBootstrapViewModel> GetWorkspaceAsync(string tenantSlug, string workspaceSlug)
    {
        return _http.GetAsync<WorkspaceBootstrapViewModel>($"api/core/bootstrap/tenant/{tenantSlug}/workspace/{workspaceSlug}");
    }

    /// <summary>
    /// Gets the current user's permissions.
    /// </summary>
    public Task<UserPermissionsViewModel> GetPermissionsAsync()
    {
        return _http.GetAsync<UserPermissionsViewModel>("api/core/bootstrap/permissions");
    }
}
