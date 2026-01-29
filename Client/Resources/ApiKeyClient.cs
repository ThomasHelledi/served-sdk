using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.ApiKeys;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for API Key management operations.
/// </summary>
public class ApiKeyClient : IApiKeyClient
{
    private readonly IServedClient _client;

    public ApiKeyClient(IServedClient client)
    {
        _client = client;
    }

    /// <inheritdoc/>
    public Task<List<ApiKeyViewModel>> ListAsync()
    {
        return _client.GetAsync<List<ApiKeyViewModel>>("api/administration/apikey");
    }

    /// <inheritdoc/>
    public Task<ApiKeyCreatedViewModel> CreateAsync(string name, List<string> scopes, DateTime? expiresAt = null)
    {
        var request = new CreateApiKeyRequest
        {
            Name = name,
            Scopes = scopes,
            ExpiresAt = expiresAt
        };
        return _client.PostAsync<ApiKeyCreatedViewModel>("api/administration/apikey", request);
    }

    /// <inheritdoc/>
    public Task DeactivateAsync(int id)
    {
        return _client.DeleteAsync($"api/administration/apikey/{id}");
    }

    /// <inheritdoc/>
    public Task<List<ApiKeyScopeInfo>> GetScopesAsync()
    {
        return _client.GetAsync<List<ApiKeyScopeInfo>>("api/administration/apikey/scopes");
    }
}
