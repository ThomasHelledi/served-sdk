namespace Served.SDK.Diagnostics;

/// <summary>
/// Health status levels for services and components.
/// </summary>
public enum HealthStatus
{
    /// <summary>Status not yet determined.</summary>
    Unknown = 0,

    /// <summary>Service is fully operational.</summary>
    Healthy = 1,

    /// <summary>Service is operational but with warnings.</summary>
    Degraded = 2,

    /// <summary>Service is not operational.</summary>
    Unhealthy = 3
}

/// <summary>
/// Health information for a single service or component.
/// </summary>
public record ComponentHealth
{
    /// <summary>
    /// Service name (e.g., "api", "unifiedhq", "redis-cache").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The endpoint URL that was checked.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Current health status.
    /// </summary>
    public required HealthStatus Status { get; init; }

    /// <summary>
    /// HTTP status code returned (if applicable).
    /// </summary>
    public int? HttpStatusCode { get; init; }

    /// <summary>
    /// Response time in milliseconds.
    /// </summary>
    public long ResponseTimeMs { get; init; }

    /// <summary>
    /// Error message if the check failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Additional metadata about the service.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Timestamp when the check was performed.
    /// </summary>
    public DateTime CheckedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Complete healthcheck result for system verification.
/// </summary>
public record HealthcheckResult
{
    /// <summary>
    /// Overall system health status.
    /// </summary>
    public required HealthStatus OverallStatus { get; init; }

    /// <summary>
    /// Individual service health results.
    /// </summary>
    public required IReadOnlyList<ComponentHealth> Services { get; init; }

    /// <summary>
    /// Trace ID for correlation (if tracing enabled).
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// Span ID for correlation (if tracing enabled).
    /// </summary>
    public string? SpanId { get; init; }

    /// <summary>
    /// Total time to perform all checks.
    /// </summary>
    public long TotalDurationMs { get; init; }

    /// <summary>
    /// When the healthcheck was performed.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Number of healthy services.
    /// </summary>
    public int HealthyCount => Services.Count(s => s.Status == HealthStatus.Healthy);

    /// <summary>
    /// Number of unhealthy services.
    /// </summary>
    public int UnhealthyCount => Services.Count(s => s.Status == HealthStatus.Unhealthy);

    /// <summary>
    /// Number of degraded services.
    /// </summary>
    public int DegradedCount => Services.Count(s => s.Status == HealthStatus.Degraded);

    /// <summary>
    /// Whether all services are healthy.
    /// </summary>
    public bool IsHealthy => OverallStatus == HealthStatus.Healthy;

    /// <summary>
    /// Creates a healthy result from component health list.
    /// </summary>
    public static HealthcheckResult FromComponents(
        IEnumerable<ComponentHealth> components,
        long totalDurationMs,
        string? traceId = null,
        string? spanId = null)
    {
        var services = components.ToList();
        var overallStatus = CalculateOverallStatus(services);

        return new HealthcheckResult
        {
            OverallStatus = overallStatus,
            Services = services,
            TotalDurationMs = totalDurationMs,
            TraceId = traceId,
            SpanId = spanId
        };
    }

    private static HealthStatus CalculateOverallStatus(IReadOnlyList<ComponentHealth> services)
    {
        if (!services.Any())
            return HealthStatus.Unknown;

        if (services.All(s => s.Status == HealthStatus.Healthy))
            return HealthStatus.Healthy;

        if (services.Any(s => s.Status == HealthStatus.Unhealthy))
            return HealthStatus.Unhealthy;

        if (services.Any(s => s.Status == HealthStatus.Degraded || s.Status == HealthStatus.Unknown))
            return HealthStatus.Degraded;

        return HealthStatus.Healthy;
    }
}
