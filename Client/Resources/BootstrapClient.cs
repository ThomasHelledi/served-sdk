using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Bootstrap;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for bootstrap data operations.
/// </summary>
public class BootstrapClient : IBootstrapClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="BootstrapClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public BootstrapClient(IServedClient client)
    {
        _client = client;
    }

    /// <inheritdoc/>
    public Task<UserBootstrap> GetUserAsync()
    {
        return _client.GetAsync<UserBootstrap>("api/core/bootstrap/user");
    }

    /// <inheritdoc/>
    public Task<TenantBootstrap> GetTenantAsync(string tenantSlug)
    {
        return _client.GetAsync<TenantBootstrap>($"api/core/bootstrap/tenant/{tenantSlug}");
    }

    /// <inheritdoc/>
    public Task<WorkspaceBootstrap> GetWorkspaceAsync(string tenantSlug, string workspaceSlug)
    {
        return _client.GetAsync<WorkspaceBootstrap>($"api/core/bootstrap/workspace/{tenantSlug}/{workspaceSlug}");
    }
}
