using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Tenants;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for tenant/organization management operations.
/// </summary>
public interface ITenantClient
{
    /// <summary>
    /// Gets all tenants accessible to the current user.
    /// </summary>
    /// <returns>List of tenant summaries.</returns>
    Task<List<TenantSummary>> GetAllAsync();

    /// <summary>
    /// Gets a tenant by its slug.
    /// </summary>
    /// <param name="slug">The tenant slug.</param>
    /// <returns>Tenant details.</returns>
    Task<TenantDetail> GetBySlugAsync(string slug);

    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    /// <param name="request">The tenant creation request.</param>
    /// <returns>The created tenant ID.</returns>
    Task<int> CreateAsync(CreateTenantRequest request);

    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    /// <param name="request">The tenant update request.</param>
    Task UpdateAsync(UpdateTenantRequest request);
}
