namespace Served.SDK.Tracing;

/// <summary>
/// Types of telemetry events captured by the SDK.
/// </summary>
public enum TelemetryEventType
{
    // HTTP Events
    RequestStart,
    RequestComplete,
    RequestError,

    // Error Events
    Exception,
    HttpError,
    SlowRequest,
    RateLimited,

    // Authentication Events
    TokenRefresh,
    AuthenticationFailure,

    // SDK Events
    ClientInitialized,
    ClientDisposed,
    CacheHit,
    CacheMiss,

    // Custom Events
    Custom
}

/// <summary>
/// Represents a telemetry event captured by the SDK.
/// </summary>
public class TelemetryEvent
{
    /// <summary>
    /// Timestamp when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Trace ID for correlation.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Span ID for correlation.
    /// </summary>
    public string? SpanId { get; set; }

    /// <summary>
    /// Type of the telemetry event.
    /// </summary>
    public TelemetryEventType Type { get; set; }

    /// <summary>
    /// Custom event name (for Custom type).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Error category if this is an error event.
    /// </summary>
    public ErrorCategory? ErrorCategory { get; set; }

    /// <summary>
    /// Severity level of the event.
    /// </summary>
    public TelemetrySeverity Severity { get; set; } = TelemetrySeverity.Info;

    /// <summary>
    /// Human-readable message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Additional attributes for the event.
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; } = new();

    /// <summary>
    /// Stack trace for exception events.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Duration in milliseconds for timed events.
    /// </summary>
    public long? DurationMs { get; set; }
}

/// <summary>
/// Severity levels for telemetry events.
/// </summary>
public enum TelemetrySeverity
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Represents a span in distributed tracing.
/// </summary>
public interface ISpan : IDisposable
{
    /// <summary>
    /// Unique identifier for the span.
    /// </summary>
    string SpanId { get; }

    /// <summary>
    /// Trace ID this span belongs to.
    /// </summary>
    string TraceId { get; }

    /// <summary>
    /// Set an attribute on the span.
    /// </summary>
    void SetAttribute(string key, object value);

    /// <summary>
    /// Mark the span as an error.
    /// </summary>
    void SetError(bool isError);

    /// <summary>
    /// Record an event on the span.
    /// </summary>
    void RecordEvent(TelemetryEventType eventType);

    /// <summary>
    /// Record an exception on the span.
    /// </summary>
    void RecordException(Exception exception);
}

/// <summary>
/// Kind of span in the trace.
/// </summary>
public enum SpanKind
{
    Internal,
    Client,
    Server,
    Producer,
    Consumer
}
