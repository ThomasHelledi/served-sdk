using System.Threading.Tasks;
using Served.SDK.Models.Bootstrap;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for bootstrap data operations.
/// Returns initial data needed when users authenticate or switch context.
/// </summary>
public interface IBootstrapClient
{
    /// <summary>
    /// Gets user bootstrap data including preferences, permissions, and accessible tenants.
    /// </summary>
    /// <returns>User bootstrap data.</returns>
    Task<UserBootstrap> GetUserAsync();

    /// <summary>
    /// Gets tenant bootstrap data including settings, features, employees, and categories.
    /// </summary>
    /// <param name="tenantSlug">The tenant slug.</param>
    /// <returns>Tenant bootstrap data.</returns>
    Task<TenantBootstrap> GetTenantAsync(string tenantSlug);

    /// <summary>
    /// Gets workspace bootstrap data including tenant, workspace, and pipeline information.
    /// </summary>
    /// <param name="tenantSlug">The tenant slug.</param>
    /// <param name="workspaceSlug">The workspace slug.</param>
    /// <returns>Workspace bootstrap data.</returns>
    Task<WorkspaceBootstrap> GetWorkspaceAsync(string tenantSlug, string workspaceSlug);
}
