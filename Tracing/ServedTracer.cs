using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Served.SDK.Tracing;

/// <summary>
/// Default implementation of IServedTracer with OpenTelemetry and Forge support.
/// </summary>
public class ServedTracer : IServedTracer
{
    private readonly TracingOptions _options;
    private readonly ActivitySource _activitySource;
    private readonly ConcurrentQueue<TelemetryEvent> _eventBuffer;
    private readonly ConcurrentDictionary<string, string> _context;
    private readonly ConcurrentDictionary<string, double> _metrics;
    private readonly HttpClient _httpClient;
    private readonly Timer _flushTimer;
    private readonly Random _random;
    private bool _disposed;

    /// <inheritdoc/>
    public bool IsEnabled { get; }

    /// <inheritdoc/>
    public TracingOptions Options => _options;

    /// <summary>
    /// Creates a new ServedTracer with the specified options.
    /// </summary>
    public ServedTracer(TracingOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _activitySource = new ActivitySource(_options.ServiceName, _options.ServiceVersion);
        _eventBuffer = new ConcurrentQueue<TelemetryEvent>();
        _context = new ConcurrentDictionary<string, string>();
        _metrics = new ConcurrentDictionary<string, double>();
        _httpClient = new HttpClient();
        _random = new Random();

        // Auto-configure from environment variables
        ConfigureFromEnvironment();

        IsEnabled = _options.EnableForge || _options.EnableOtlp;

        if (IsEnabled)
        {
            _flushTimer = new Timer(
                _ => _ = FlushAsync(),
                null,
                _options.FlushIntervalMs,
                _options.FlushIntervalMs);
        }
        else
        {
            _flushTimer = new Timer(_ => { }, null, Timeout.Infinite, Timeout.Infinite);
        }

        // Record initialization event
        RecordEvent(new TelemetryEvent
        {
            Type = TelemetryEventType.ClientInitialized,
            Severity = TelemetrySeverity.Info,
            Message = $"ServedTracer initialized (Forge: {_options.EnableForge}, OTLP: {_options.EnableOtlp})"
        });
    }

    private void ConfigureFromEnvironment()
    {
        // Service configuration
        var envServiceName = Environment.GetEnvironmentVariable("SERVED_SERVICE_NAME");
        if (!string.IsNullOrEmpty(envServiceName))
            _options.ServiceName = envServiceName;

        var envServiceVersion = Environment.GetEnvironmentVariable("SERVED_SERVICE_VERSION");
        if (!string.IsNullOrEmpty(envServiceVersion))
            _options.ServiceVersion = envServiceVersion;

        var envEnvironment = Environment.GetEnvironmentVariable("SERVED_ENVIRONMENT");
        if (!string.IsNullOrEmpty(envEnvironment))
            _options.Environment = envEnvironment;

        // Forge configuration
        var forgeApiKey = Environment.GetEnvironmentVariable("FORGE_API_KEY");
        if (!string.IsNullOrEmpty(forgeApiKey))
        {
            _options.ForgeApiKey = forgeApiKey;
            _options.EnableForge = true;
        }

        // Check both FORGE_ENDPOINT and SERVED_TELEMETRY_ENDPOINT for backwards compatibility
        var forgeEndpoint = Environment.GetEnvironmentVariable("FORGE_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("SERVED_TELEMETRY_ENDPOINT");
        if (!string.IsNullOrEmpty(forgeEndpoint))
            _options.ForgeEndpoint = forgeEndpoint;

        // OTLP configuration
        var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        if (!string.IsNullOrEmpty(otlpEndpoint))
        {
            _options.OtlpEndpoint = otlpEndpoint;
            _options.EnableOtlp = true;
        }

        // Sampling configuration
        var samplingRate = Environment.GetEnvironmentVariable("SERVED_SAMPLING_RATE");
        if (!string.IsNullOrEmpty(samplingRate) && double.TryParse(samplingRate, out var rate))
            _options.SamplingRate = Math.Clamp(rate, 0.0, 1.0);

        // Enable from environment
        var tracingEnabled = Environment.GetEnvironmentVariable("SERVED_TRACING_ENABLED");
        if (tracingEnabled?.Equals("true", StringComparison.OrdinalIgnoreCase) == true)
        {
            if (!_options.EnableForge && !_options.EnableOtlp)
            {
                // Default to Forge if no specific exporter configured
                _options.EnableForge = true;
            }
        }
    }

    /// <inheritdoc/>
    public ISpan StartSpan(string operationName, SpanKind kind = SpanKind.Client)
    {
        if (!IsEnabled)
            return new NoOpSpan();

        // Apply sampling
        if (!ShouldSample())
            return new NoOpSpan();

        var activityKind = kind switch
        {
            SpanKind.Server => ActivityKind.Server,
            SpanKind.Producer => ActivityKind.Producer,
            SpanKind.Consumer => ActivityKind.Consumer,
            SpanKind.Internal => ActivityKind.Internal,
            _ => ActivityKind.Client
        };

        var activity = _activitySource.StartActivity(operationName, activityKind);
        if (activity == null)
            return new NoOpSpan();

        // Add standard attributes
        activity.SetTag("service.name", _options.ServiceName);
        activity.SetTag("service.version", _options.ServiceVersion);
        activity.SetTag("deployment.environment", _options.Environment);

        // Add context attributes
        foreach (var ctx in _context)
        {
            activity.SetTag(ctx.Key, ctx.Value);
        }

        // Add custom tags from enrichment options
        foreach (var tag in _options.ForgeEnrichment.CustomTags)
        {
            activity.SetTag(tag.Key, tag.Value);
        }

        return new ActivitySpan(activity, this);
    }

    private bool ShouldSample()
    {
        return _random.NextDouble() <= _options.SamplingRate;
    }

    /// <inheritdoc/>
    public void RecordEvent(TelemetryEvent evt)
    {
        if (!IsEnabled)
            return;

        // Always sample errors if configured
        if (evt.Severity >= TelemetrySeverity.Error && _options.AlwaysSampleErrors)
        {
            EnqueueEvent(evt);
            return;
        }

        // Apply sampling
        if (!ShouldSample())
            return;

        EnqueueEvent(evt);
    }

    private void EnqueueEvent(TelemetryEvent evt)
    {
        // Set trace context if available
        var activity = Activity.Current;
        if (activity != null)
        {
            evt.TraceId = activity.TraceId.ToString();
            evt.SpanId = activity.SpanId.ToString();
        }

        _eventBuffer.Enqueue(evt);

        // Flush if buffer is full
        if (_eventBuffer.Count >= _options.BufferSize)
        {
            _ = FlushAsync();
        }
    }

    /// <inheritdoc/>
    public void RecordMetric(string name, double value, IDictionary<string, string>? tags = null)
    {
        if (!IsEnabled)
            return;

        // Simple aggregation - store latest value
        var key = BuildMetricKey(name, tags);
        _metrics[key] = value;
    }

    private static string BuildMetricKey(string name, IDictionary<string, string>? tags)
    {
        if (tags == null || tags.Count == 0)
            return name;

        var sb = new StringBuilder(name);
        foreach (var tag in tags.OrderBy(t => t.Key))
        {
            sb.Append('|').Append(tag.Key).Append('=').Append(tag.Value);
        }
        return sb.ToString();
    }

    /// <inheritdoc/>
    public async Task FlushAsync()
    {
        if (!IsEnabled || _disposed)
            return;

        var events = new List<TelemetryEvent>();
        while (_eventBuffer.TryDequeue(out var evt))
        {
            events.Add(evt);
        }

        if (events.Count == 0)
            return;

        var tasks = new List<Task>();

        if (_options.EnableForge)
        {
            tasks.Add(ExportToForgeAsync(events));
        }

        // OTLP export handled by OpenTelemetry SDK directly via Activity

        await Task.WhenAll(tasks);
    }

    private async Task ExportToForgeAsync(List<TelemetryEvent> events)
    {
        if (string.IsNullOrEmpty(_options.ForgeApiKey))
            return;

        var batch = new
        {
            serviceName = _options.ServiceName,
            serviceVersion = _options.ServiceVersion,
            environment = _options.Environment,
            sdkVersion = typeof(ServedTracer).Assembly.GetName().Version?.ToString() ?? "unknown",
            batchTimestamp = DateTime.UtcNow,
            events = events.Select(e => new
            {
                timestamp = e.Timestamp,
                traceId = e.TraceId,
                spanId = e.SpanId,
                eventType = e.Type.ToString(),
                name = e.Name,
                errorCategory = e.ErrorCategory?.ToString(),
                severity = e.Severity.ToString().ToLowerInvariant(),
                message = e.Message,
                attributes = e.Attributes,
                stackTrace = e.StackTrace,
                durationMs = e.DurationMs
            }).ToList()
        };

        for (int retry = 0; retry < _options.MaxRetries; retry++)
        {
            try
            {
                var json = JsonSerializer.Serialize(batch);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, _options.ForgeEndpoint + "/events")
                {
                    Content = content
                };
                request.Headers.Add("X-Forge-Api-Key", _options.ForgeApiKey);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return;

                // Don't retry 4xx errors (except 429)
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500 && response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                    return;
            }
            catch
            {
                // Silently fail telemetry export - don't affect main application
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, retry)));
        }
    }

    /// <inheritdoc/>
    public void SetContext(string key, string value)
    {
        _context[key] = value;
    }

    /// <inheritdoc/>
    public string? GetContext(string key)
    {
        return _context.TryGetValue(key, out var value) ? value : null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        RecordEvent(new TelemetryEvent
        {
            Type = TelemetryEventType.ClientDisposed,
            Severity = TelemetrySeverity.Info,
            Message = "ServedTracer disposed"
        });

        FlushAsync().Wait(TimeSpan.FromSeconds(5));

        _flushTimer.Dispose();
        _activitySource.Dispose();
        _httpClient.Dispose();
    }
}

/// <summary>
/// Span implementation backed by System.Diagnostics.Activity.
/// </summary>
internal class ActivitySpan : ISpan
{
    private readonly Activity _activity;
    private readonly ServedTracer _tracer;

    public string SpanId => _activity.SpanId.ToString();
    public string TraceId => _activity.TraceId.ToString();

    public ActivitySpan(Activity activity, ServedTracer tracer)
    {
        _activity = activity;
        _tracer = tracer;
    }

    public void SetAttribute(string key, object value)
    {
        _activity.SetTag(key, value);
    }

    public void SetError(bool isError)
    {
        _activity.SetTag("error", isError);
        if (isError)
        {
            _activity.SetStatus(ActivityStatusCode.Error);
        }
    }

    public void RecordEvent(TelemetryEventType eventType)
    {
        _activity.AddEvent(new ActivityEvent(eventType.ToString()));
    }

    public void RecordException(Exception exception)
    {
        _activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        _activity.SetTag("exception.type", exception.GetType().FullName);
        _activity.SetTag("exception.message", exception.Message);
        _activity.SetTag("exception.stacktrace", exception.StackTrace);

        _tracer.RecordEvent(new TelemetryEvent
        {
            Type = TelemetryEventType.Exception,
            TraceId = TraceId,
            SpanId = SpanId,
            ErrorCategory = ErrorCategoryHelper.CategorizeException(exception),
            Severity = TelemetrySeverity.Error,
            Message = exception.Message,
            StackTrace = exception.StackTrace
        });
    }

    public void Dispose()
    {
        _activity.Dispose();
    }
}

/// <summary>
/// No-op span for when tracing is disabled or not sampled.
/// </summary>
internal class NoOpSpan : ISpan
{
    public string SpanId => string.Empty;
    public string TraceId => string.Empty;

    public void SetAttribute(string key, object value) { }
    public void SetError(bool isError) { }
    public void RecordEvent(TelemetryEventType eventType) { }
    public void RecordException(Exception exception) { }
    public void Dispose() { }
}
