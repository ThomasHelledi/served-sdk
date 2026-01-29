using System;
using System.Collections.Generic;

namespace Served.SDK.Models.ApiKeys;

/// <summary>
/// API Key view model returned from list/get operations.
/// </summary>
public class ApiKeyViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyHint { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? LastUsedIp { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }
}

/// <summary>
/// Response when creating a new API key - includes the plaintext key.
/// </summary>
public class ApiKeyCreatedViewModel
{
    public ApiKeyViewModel ApiKey { get; set; } = new();
    public string PlainKey { get; set; } = string.Empty;
}

/// <summary>
/// Request to create a new API key.
/// </summary>
public class CreateApiKeyRequest
{
    public string Name { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Request to update an API key.
/// </summary>
public class UpdateApiKeyRequest
{
    public string? Name { get; set; }
    public List<string>? Scopes { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Available API key scope definition.
/// </summary>
public class ApiKeyScopeInfo
{
    public string Scope { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
