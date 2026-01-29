namespace Served.SDK.Diagnostics;

/// <summary>
/// Configuration options for healthcheck operations.
/// </summary>
public class HealthcheckOptions
{
    /// <summary>
    /// Timeout for individual health checks.
    /// Default: 10 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Whether to run checks in parallel.
    /// Default: true (faster).
    /// </summary>
    public bool Parallel { get; set; } = true;

    /// <summary>
    /// Number of retry attempts for failed checks.
    /// Default: 1 (one retry).
    /// </summary>
    public int RetryCount { get; set; } = 1;

    /// <summary>
    /// Delay between retry attempts.
    /// Default: 500ms.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Custom endpoints to check in addition to defaults.
    /// </summary>
    public List<ServiceEndpoint> CustomEndpoints { get; set; } = new();

    /// <summary>
    /// Whether to include detailed metadata in results.
    /// Default: false (smaller payloads).
    /// </summary>
    public bool IncludeMetadata { get; set; } = false;

    /// <summary>
    /// Whether to create tracing spans for healthchecks.
    /// Default: true.
    /// </summary>
    public bool EnableTracing { get; set; } = true;

    /// <summary>
    /// Response time threshold for degraded status (milliseconds).
    /// Default: 5000ms.
    /// </summary>
    public long DegradedThresholdMs { get; set; } = 5000;

    /// <summary>
    /// Creates default options for local development.
    /// </summary>
    public static HealthcheckOptions Local()
    {
        return new HealthcheckOptions
        {
            Timeout = TimeSpan.FromSeconds(5),
            RetryCount = 0,
            Parallel = true,
            CustomEndpoints = new List<ServiceEndpoint>
            {
                new("api", "http://localhost:5010/healthz/readiness"),
                new("webapp", "http://localhost:4200"),
                new("unifiedhq", "http://localhost:4201"),
                new("cloud", "http://localhost:4202"),
                new("website", "http://localhost:4203")
            }
        };
    }

    /// <summary>
    /// Creates default options for production environment.
    /// </summary>
    public static HealthcheckOptions Production()
    {
        return new HealthcheckOptions
        {
            Timeout = TimeSpan.FromSeconds(15),
            RetryCount = 2,
            Parallel = true,
            CustomEndpoints = new List<ServiceEndpoint>
            {
                new("api", "https://app.served.dk/healthz/readiness"),
                new("unifiedhq", "https://unifiedhq.ai/"),
                new("cloud", "https://cloud.served.dk/"),
                new("website", "https://served.dk/")
            }
        };
    }

    /// <summary>
    /// Creates options for pre-deploy verification.
    /// </summary>
    public static HealthcheckOptions PreDeploy()
    {
        return new HealthcheckOptions
        {
            Timeout = TimeSpan.FromSeconds(3),
            RetryCount = 0,
            Parallel = true,
            EnableTracing = false // Don't trace pre-deploy checks
        };
    }
}

/// <summary>
/// Represents a service endpoint to check.
/// </summary>
public record ServiceEndpoint(string Name, string Url)
{
    /// <summary>
    /// Expected HTTP status codes for healthy state.
    /// Default: 200-299.
    /// </summary>
    public int[] ExpectedStatusCodes { get; init; } = new[] { 200, 201, 204 };

    /// <summary>
    /// Whether this is a critical service.
    /// Critical services being down results in Unhealthy status.
    /// </summary>
    public bool IsCritical { get; init; } = true;

    /// <summary>
    /// Optional healthcheck path to append to base URL.
    /// </summary>
    public string? HealthPath { get; init; }

    /// <summary>
    /// Gets the full URL to check (Url + HealthPath if present).
    /// </summary>
    public string GetCheckUrl()
    {
        if (string.IsNullOrEmpty(HealthPath))
            return Url;

        var baseUrl = Url.TrimEnd('/');
        var path = HealthPath.TrimStart('/');
        return $"{baseUrl}/{path}";
    }
}
