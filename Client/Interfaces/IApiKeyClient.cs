using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.ApiKeys;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for API Key management operations.
/// </summary>
public interface IApiKeyClient
{
    /// <summary>
    /// Gets all API keys for the current organization.
    /// </summary>
    Task<List<ApiKeyViewModel>> ListAsync();

    /// <summary>
    /// Creates a new API key.
    /// </summary>
    /// <param name="name">Name for the API key.</param>
    /// <param name="scopes">List of scopes to grant.</param>
    /// <param name="expiresAt">Optional expiration date.</param>
    /// <returns>The created API key with plaintext key (only shown once!).</returns>
    Task<ApiKeyCreatedViewModel> CreateAsync(string name, List<string> scopes, DateTime? expiresAt = null);

    /// <summary>
    /// Deactivates an API key.
    /// </summary>
    /// <param name="id">API key ID to deactivate.</param>
    Task DeactivateAsync(int id);

    /// <summary>
    /// Gets available API key scopes.
    /// </summary>
    Task<List<ApiKeyScopeInfo>> GetScopesAsync();
}
