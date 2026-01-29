using Served.SDK.Client.Core;
using Served.SDK.Diagnostics;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for system health verification.
/// Use this to verify services are running before claiming success.
/// </summary>
/// <example>
/// // Quick health check
/// var isHealthy = await client.Health.IsHealthyAsync();
///
/// // Full system health with details
/// var result = await client.Health.GetSystemHealthAsync();
/// Console.WriteLine($"Status: {result.OverallStatus}");
///
/// // Check specific service
/// var apiHealth = await client.Health.CheckServiceAsync("api");
/// </example>
public class HealthApi : ApiModuleBase
{
    protected override string ModulePath => "system";

    private HealthcheckOptions? _cachedOptions;

    public HealthApi(IHttpClient http) : base(http)
    {
    }

    /// <summary>
    /// Gets the full system health status with all component details.
    /// </summary>
    /// <param name="local">If true, checks local dev environment. If false, checks production.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Complete healthcheck result.</returns>
    public async Task<HealthcheckResult> GetSystemHealthAsync(
        bool local = false,
        CancellationToken cancellationToken = default)
    {
        var options = local
            ? HealthcheckOptions.Local()
            : HealthcheckOptions.Production();

        using var client = new HealthcheckClient(options);
        return await client.CheckAllAsync(cancellationToken);
    }

    /// <summary>
    /// Quick check if the system is healthy.
    /// </summary>
    /// <param name="local">If true, checks local dev environment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all services are healthy.</returns>
    public async Task<bool> IsHealthyAsync(
        bool local = false,
        CancellationToken cancellationToken = default)
    {
        var result = await GetSystemHealthAsync(local, cancellationToken);
        return result.IsHealthy;
    }

    /// <summary>
    /// Checks health of a specific service.
    /// </summary>
    /// <param name="serviceName">Service name (api, unifiedhq, webapp, etc.)</param>
    /// <param name="local">If true, uses local URLs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Health status for the service.</returns>
    public async Task<ComponentHealth> CheckServiceAsync(
        string serviceName,
        bool local = false,
        CancellationToken cancellationToken = default)
    {
        var endpoint = GetEndpointForService(serviceName, local);

        using var client = new HealthcheckClient(HealthcheckOptions.PreDeploy());
        return await client.CheckAsync(endpoint, cancellationToken);
    }

    /// <summary>
    /// Verifies the API is responding and returns version info.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API health check response.</returns>
    public async Task<ApiHealthInfo?> GetApiHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await Http.GetAsync<ApiHealthInfo>($"{BuildLegacyPath("system/health")}");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Atlas verification workflow - checks services after build/deploy.
    /// Used by Atlas to verify before claiming "it works".
    /// </summary>
    /// <param name="context">Verification context (local, pre-deploy, post-deploy).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Verification result with recommendations.</returns>
    public async Task<AtlasVerificationResult> AtlasVerifyAsync(
        AtlasVerifyContext context = AtlasVerifyContext.Local,
        CancellationToken cancellationToken = default)
    {
        var options = context switch
        {
            AtlasVerifyContext.Local => HealthcheckOptions.Local(),
            AtlasVerifyContext.PreDeploy => HealthcheckOptions.PreDeploy(),
            AtlasVerifyContext.PostDeploy => HealthcheckOptions.Production(),
            _ => HealthcheckOptions.Local()
        };

        using var client = new HealthcheckClient(options);
        var health = await client.CheckAllAsync(cancellationToken);

        var recommendations = new List<string>();

        if (!health.IsHealthy)
        {
            foreach (var service in health.Services.Where(s => s.Status != HealthStatus.Healthy))
            {
                var suggestion = GetSuggestionForService(service.Name, service.Error);
                recommendations.Add(suggestion);
            }
        }

        return new AtlasVerificationResult
        {
            Passed = health.IsHealthy,
            Context = context,
            Health = health,
            Recommendations = recommendations,
            Message = health.IsHealthy
                ? "All services verified running"
                : $"{health.UnhealthyCount} service(s) not responding"
        };
    }

    private static ServiceEndpoint GetEndpointForService(string serviceName, bool local)
    {
        return serviceName.ToLowerInvariant() switch
        {
            "api" => local
                ? new ServiceEndpoint("api", "http://localhost:5010/healthz/readiness")
                : new ServiceEndpoint("api", "https://app.served.dk/healthz/readiness"),

            "webapp" => local
                ? new ServiceEndpoint("webapp", "http://localhost:4200")
                : new ServiceEndpoint("webapp", "https://app.served.dk"),

            "unifiedhq" => local
                ? new ServiceEndpoint("unifiedhq", "http://localhost:4201")
                : new ServiceEndpoint("unifiedhq", "https://unifiedhq.ai"),

            "cloud" => local
                ? new ServiceEndpoint("cloud", "http://localhost:4202")
                : new ServiceEndpoint("cloud", "https://cloud.served.dk"),

            "website" => local
                ? new ServiceEndpoint("website", "http://localhost:4203")
                : new ServiceEndpoint("website", "https://served.dk"),

            _ => throw new ArgumentException($"Unknown service: {serviceName}")
        };
    }

    private static string GetSuggestionForService(string serviceName, string? error)
    {
        if (error?.Contains("Connection refused") == true)
        {
            return serviceName switch
            {
                "api" => "API not running. Start with: served dev api",
                "webapp" => "Webapp not running. Start with: served dev webapp",
                "unifiedhq" => "UnifiedHQ not running. Start with: served dev unifiedhq",
                "cloud" => "Cloud not running. Start with: served dev cloud",
                "website" => "Website not running. Start with: served dev website",
                _ => $"{serviceName} not running. Check service logs."
            };
        }

        return $"Check {serviceName} status with: served dev status";
    }
}

/// <summary>
/// API health response from /api/system/health
/// </summary>
public record ApiHealthInfo
{
    public string? Status { get; init; }
    public string? Version { get; init; }
    public string? GitCommit { get; init; }
    public string? BuildDate { get; init; }
    public string? Environment { get; init; }
    public string? Uptime { get; init; }
}

/// <summary>
/// Context for Atlas verification
/// </summary>
public enum AtlasVerifyContext
{
    /// <summary>Check local development environment.</summary>
    Local,

    /// <summary>Quick check before deployment.</summary>
    PreDeploy,

    /// <summary>Full check after deployment.</summary>
    PostDeploy
}

/// <summary>
/// Result of Atlas verification workflow.
/// </summary>
public record AtlasVerificationResult
{
    /// <summary>Whether verification passed.</summary>
    public required bool Passed { get; init; }

    /// <summary>Verification context used.</summary>
    public AtlasVerifyContext Context { get; init; }

    /// <summary>Full health check result.</summary>
    public required HealthcheckResult Health { get; init; }

    /// <summary>Recommendations for fixing issues.</summary>
    public List<string> Recommendations { get; init; } = new();

    /// <summary>Human-readable summary message.</summary>
    public required string Message { get; init; }
}
