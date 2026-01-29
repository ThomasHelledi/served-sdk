namespace Served.SDK.Tracing;

/// <summary>
/// Configuration options for SDK tracing and observability.
/// </summary>
public class TracingOptions
{
    /// <summary>
    /// Service name for tracing identification.
    /// </summary>
    public string ServiceName { get; set; } = "served-sdk-client";

    /// <summary>
    /// Service version for tracing.
    /// </summary>
    public string ServiceVersion { get; set; } = typeof(TracingOptions).Assembly.GetName().Version?.ToString() ?? "unknown";

    /// <summary>
    /// Environment name (development, staging, production).
    /// </summary>
    public string Environment { get; set; } = "development";

    /// <summary>
    /// Enable Forge platform integration.
    /// </summary>
    public bool EnableForge { get; set; }

    /// <summary>
    /// Forge API key for telemetry export.
    /// </summary>
    public string? ForgeApiKey { get; set; }

    /// <summary>
    /// Forge telemetry endpoint.
    /// Defaults to UnifiedHQ infrastructure.
    /// </summary>
    public string ForgeEndpoint { get; set; } = "https://apis.unifiedhq.ai/v1/telemetry";

    /// <summary>
    /// Enable OpenTelemetry Protocol export.
    /// </summary>
    public bool EnableOtlp { get; set; }

    /// <summary>
    /// OTLP collector endpoint.
    /// </summary>
    public string? OtlpEndpoint { get; set; }

    /// <summary>
    /// Sampling rate for requests (0.0-1.0). Default is 1.0 (100%).
    /// </summary>
    public double SamplingRate { get; set; } = 1.0;

    /// <summary>
    /// Always sample errors regardless of sampling rate.
    /// </summary>
    public bool AlwaysSampleErrors { get; set; } = true;

    /// <summary>
    /// Error detection options.
    /// </summary>
    public ErrorDetectionOptions ErrorDetection { get; set; } = new();

    /// <summary>
    /// Metrics collection options.
    /// </summary>
    public MetricsOptions Metrics { get; set; } = new();

    /// <summary>
    /// Forge-specific enrichment options.
    /// </summary>
    public ForgeEnrichmentOptions ForgeEnrichment { get; set; } = new();

    /// <summary>
    /// Buffer size for batching telemetry before export.
    /// </summary>
    public int BufferSize { get; set; } = 100;

    /// <summary>
    /// Flush interval in milliseconds.
    /// </summary>
    public int FlushIntervalMs { get; set; } = 5000;

    /// <summary>
    /// Maximum retry attempts for failed exports.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}

/// <summary>
/// Error detection configuration.
/// </summary>
public class ErrorDetectionOptions
{
    /// <summary>
    /// Capture unhandled exceptions.
    /// </summary>
    public bool CaptureExceptions { get; set; } = true;

    /// <summary>
    /// Capture HTTP 4xx and 5xx errors.
    /// </summary>
    public bool CaptureHttpErrors { get; set; } = true;

    /// <summary>
    /// Capture slow requests.
    /// </summary>
    public bool CaptureSlowRequests { get; set; } = true;

    /// <summary>
    /// Threshold in milliseconds for slow request detection.
    /// </summary>
    public int SlowRequestThresholdMs { get; set; } = 5000;

    /// <summary>
    /// HTTP status codes to ignore as errors.
    /// </summary>
    public int[] IgnoreStatusCodes { get; set; } = Array.Empty<int>();
}

/// <summary>
/// Metrics collection configuration.
/// </summary>
public class MetricsOptions
{
    /// <summary>
    /// Enable request duration histogram.
    /// </summary>
    public bool EnableRequestDuration { get; set; } = true;

    /// <summary>
    /// Enable request count counter.
    /// </summary>
    public bool EnableRequestCount { get; set; } = true;

    /// <summary>
    /// Enable error rate tracking.
    /// </summary>
    public bool EnableErrorRate { get; set; } = true;

    /// <summary>
    /// Enable throughput tracking.
    /// </summary>
    public bool EnableThroughput { get; set; } = true;

    /// <summary>
    /// Histogram bucket boundaries in milliseconds.
    /// </summary>
    public double[] HistogramBuckets { get; set; } = new[] { 10.0, 50, 100, 250, 500, 1000, 2500, 5000 };
}

/// <summary>
/// Forge-specific enrichment options.
/// </summary>
public class ForgeEnrichmentOptions
{
    /// <summary>
    /// Include entity context (projectId, taskId, etc.).
    /// </summary>
    public bool IncludeEntityContext { get; set; } = true;

    /// <summary>
    /// Include user context from bootstrap.
    /// </summary>
    public bool IncludeUserContext { get; set; } = true;

    /// <summary>
    /// Include tenant context.
    /// </summary>
    public bool IncludeTenantContext { get; set; } = true;

    /// <summary>
    /// Custom tags to include with all telemetry.
    /// </summary>
    public Dictionary<string, string> CustomTags { get; set; } = new();
}
