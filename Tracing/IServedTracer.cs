namespace Served.SDK.Tracing;

/// <summary>
/// Main interface for SDK tracing and observability.
/// </summary>
public interface IServedTracer : IDisposable
{
    /// <summary>
    /// Check if tracing is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Current tracing options.
    /// </summary>
    TracingOptions Options { get; }

    /// <summary>
    /// Start a new span for an operation.
    /// </summary>
    /// <param name="operationName">Name of the operation being traced.</param>
    /// <param name="kind">Kind of span (client, server, etc.).</param>
    /// <returns>A span that should be disposed when the operation completes.</returns>
    ISpan StartSpan(string operationName, SpanKind kind = SpanKind.Client);

    /// <summary>
    /// Record a telemetry event.
    /// </summary>
    /// <param name="evt">The event to record.</param>
    void RecordEvent(TelemetryEvent evt);

    /// <summary>
    /// Record a metric value.
    /// </summary>
    /// <param name="name">Name of the metric.</param>
    /// <param name="value">Value to record.</param>
    /// <param name="tags">Optional tags/labels for the metric.</param>
    void RecordMetric(string name, double value, IDictionary<string, string>? tags = null);

    /// <summary>
    /// Flush buffered telemetry to exporters.
    /// </summary>
    /// <returns>Task that completes when flush is done.</returns>
    Task FlushAsync();

    /// <summary>
    /// Set context for subsequent operations.
    /// </summary>
    /// <param name="key">Context key.</param>
    /// <param name="value">Context value.</param>
    void SetContext(string key, string value);

    /// <summary>
    /// Get a context value.
    /// </summary>
    /// <param name="key">Context key.</param>
    /// <returns>Context value or null if not set.</returns>
    string? GetContext(string key);
}
