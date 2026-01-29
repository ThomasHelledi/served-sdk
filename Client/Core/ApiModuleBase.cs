namespace Served.SDK.Client.Core;

/// <summary>
/// Base class for API module groupings.
/// Each module represents a logical grouping of related API resources.
/// </summary>
public abstract class ApiModuleBase
{
    protected readonly IHttpClient Http;

    /// <summary>
    /// The module path prefix (e.g., "project-management", "finance").
    /// </summary>
    protected abstract string ModulePath { get; }

    /// <summary>
    /// The API version (e.g., "v1", "v2").
    /// </summary>
    protected virtual string Version => "v1";

    /// <summary>
    /// Builds the full base path for a resource within this module.
    /// </summary>
    protected string BuildResourcePath(string resource)
    {
        return $"api/{Version}/{ModulePath}/{resource}";
    }

    /// <summary>
    /// Builds the full base path for a resource without module prefix (for legacy endpoints).
    /// </summary>
    protected string BuildLegacyPath(string resource)
    {
        return $"api/{resource}";
    }

    protected ApiModuleBase(IHttpClient http)
    {
        Http = http;
    }
}
