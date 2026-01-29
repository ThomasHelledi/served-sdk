using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Tenants;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for tenant/organization management operations.
/// </summary>
public class TenantClient : ITenantClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public TenantClient(IServedClient client)
    {
        _client = client;
    }

    /// <inheritdoc/>
    public Task<List<TenantSummary>> GetAllAsync()
    {
        return _client.GetAsync<List<TenantSummary>>("api/tenants/Get");
    }

    /// <inheritdoc/>
    public Task<TenantDetail> GetBySlugAsync(string slug)
    {
        return _client.GetAsync<TenantDetail>($"api/tenants/GetBySlug?slug={slug}");
    }

    /// <inheritdoc/>
    public Task<int> CreateAsync(CreateTenantRequest request)
    {
        return _client.PostAsync<int>("api/tenants/Create", request);
    }

    /// <inheritdoc/>
    public Task UpdateAsync(UpdateTenantRequest request)
    {
        return _client.PostAsync("api/tenants/Update", request);
    }
}
