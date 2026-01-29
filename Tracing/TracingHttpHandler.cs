using System.Diagnostics;
using System.Net;

namespace Served.SDK.Tracing;

/// <summary>
/// HTTP message handler that instruments outgoing requests with tracing.
/// </summary>
public class TracingHttpHandler : DelegatingHandler
{
    private readonly IServedTracer _tracer;
    private readonly TracingOptions _options;

    /// <summary>
    /// Creates a new TracingHttpHandler.
    /// </summary>
    public TracingHttpHandler(IServedTracer tracer, HttpMessageHandler? innerHandler = null)
    {
        _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        _options = tracer.Options;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!_tracer.IsEnabled)
            return await base.SendAsync(request, cancellationToken);

        var operationName = GetOperationName(request);
        using var span = _tracer.StartSpan(operationName);

        try
        {
            // Add trace context headers for distributed tracing
            InjectTraceContext(request, span);

            // Record request attributes
            span.SetAttribute("http.method", request.Method.ToString());
            span.SetAttribute("http.url", SanitizeUrl(request.RequestUri?.ToString() ?? ""));
            span.SetAttribute("http.host", request.RequestUri?.Host ?? "");
            span.SetAttribute("http.scheme", request.RequestUri?.Scheme ?? "");

            if (request.Content?.Headers.ContentLength > 0)
            {
                span.SetAttribute("http.request_content_length", request.Content.Headers.ContentLength.Value);
            }

            // Extract Served-specific context from headers
            if (request.Headers.TryGetValues("Served-Tenant", out var tenantValues))
            {
                span.SetAttribute("served.tenant", tenantValues.First());
            }

            // Record request start event
            span.RecordEvent(TelemetryEventType.RequestStart);

            var stopwatch = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            // Record response attributes
            span.SetAttribute("http.status_code", (int)response.StatusCode);

            if (response.Content?.Headers.ContentLength > 0)
            {
                span.SetAttribute("http.response_content_length", response.Content.Headers.ContentLength.Value);
            }

            // Extract API module from URL path
            var apiModule = ExtractApiModule(request.RequestUri);
            if (!string.IsNullOrEmpty(apiModule))
            {
                span.SetAttribute("served.api.module", apiModule);
            }

            // Record duration metric
            if (_options.Metrics.EnableRequestDuration)
            {
                _tracer.RecordMetric("served.sdk.requests.duration", stopwatch.ElapsedMilliseconds, new Dictionary<string, string>
                {
                    ["http_method"] = request.Method.ToString(),
                    ["http_status_code"] = ((int)response.StatusCode).ToString(),
                    ["served_api_module"] = apiModule ?? "unknown"
                });
            }

            // Record request count metric
            if (_options.Metrics.EnableRequestCount)
            {
                _tracer.RecordMetric("served.sdk.requests.total", 1, new Dictionary<string, string>
                {
                    ["http_method"] = request.Method.ToString(),
                    ["served_api_module"] = apiModule ?? "unknown"
                });
            }

            // Check for HTTP errors
            if (!response.IsSuccessStatusCode && ShouldCaptureHttpError(response.StatusCode))
            {
                var errorCategory = ErrorCategoryHelper.CategorizeHttpStatus((int)response.StatusCode);
                span.SetError(true);
                span.SetAttribute("error.category", errorCategory.ToString());
                span.RecordEvent(TelemetryEventType.HttpError);

                // Record error metric
                if (_options.Metrics.EnableErrorRate)
                {
                    _tracer.RecordMetric("served.sdk.requests.errors", 1, new Dictionary<string, string>
                    {
                        ["http_status_code"] = ((int)response.StatusCode).ToString(),
                        ["error_category"] = errorCategory.ToString()
                    });
                }

                // Log error event
                _tracer.RecordEvent(new TelemetryEvent
                {
                    Type = TelemetryEventType.HttpError,
                    ErrorCategory = errorCategory,
                    Severity = ErrorCategoryHelper.GetDefaultSeverity(errorCategory),
                    Message = $"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}",
                    Attributes = new Dictionary<string, object>
                    {
                        ["http.status_code"] = (int)response.StatusCode,
                        ["http.url"] = SanitizeUrl(request.RequestUri?.ToString() ?? ""),
                        ["http.method"] = request.Method.ToString()
                    },
                    DurationMs = stopwatch.ElapsedMilliseconds
                });
            }

            // Check for rate limiting
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                span.RecordEvent(TelemetryEventType.RateLimited);

                _tracer.RecordEvent(new TelemetryEvent
                {
                    Type = TelemetryEventType.RateLimited,
                    Severity = TelemetrySeverity.Warning,
                    Message = "Rate limited by API",
                    Attributes = new Dictionary<string, object>
                    {
                        ["http.url"] = SanitizeUrl(request.RequestUri?.ToString() ?? "")
                    }
                });
            }

            // Check for slow requests
            if (_options.ErrorDetection.CaptureSlowRequests &&
                stopwatch.ElapsedMilliseconds > _options.ErrorDetection.SlowRequestThresholdMs)
            {
                span.RecordEvent(TelemetryEventType.SlowRequest);
                span.SetAttribute("served.slow_request", true);

                _tracer.RecordEvent(new TelemetryEvent
                {
                    Type = TelemetryEventType.SlowRequest,
                    Severity = TelemetrySeverity.Warning,
                    Message = $"Slow request: {stopwatch.ElapsedMilliseconds}ms",
                    Attributes = new Dictionary<string, object>
                    {
                        ["http.url"] = SanitizeUrl(request.RequestUri?.ToString() ?? ""),
                        ["http.method"] = request.Method.ToString(),
                        ["duration_ms"] = stopwatch.ElapsedMilliseconds,
                        ["threshold_ms"] = _options.ErrorDetection.SlowRequestThresholdMs
                    },
                    DurationMs = stopwatch.ElapsedMilliseconds
                });
            }

            span.RecordEvent(TelemetryEventType.RequestComplete);
            return response;
        }
        catch (Exception ex)
        {
            var errorCategory = ErrorCategoryHelper.CategorizeException(ex);
            span.SetError(true);
            span.SetAttribute("error.category", errorCategory.ToString());
            span.RecordException(ex);

            // Record error metric
            if (_options.Metrics.EnableErrorRate)
            {
                _tracer.RecordMetric("served.sdk.requests.errors", 1, new Dictionary<string, string>
                {
                    ["error_category"] = errorCategory.ToString(),
                    ["exception_type"] = ex.GetType().Name
                });
            }

            throw;
        }
    }

    private static string GetOperationName(HttpRequestMessage request)
    {
        var path = request.RequestUri?.AbsolutePath ?? "/";
        var method = request.Method.ToString();

        // Extract meaningful operation name from path
        // e.g., /api/v1/project-management/projects -> ServedClient.ProjectManagement.Projects
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length >= 3 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
        {
            // Skip 'api' and optional version
            var startIndex = segments[1].StartsWith("v") ? 2 : 1;
            if (startIndex < segments.Length)
            {
                var module = ToPascalCase(segments[startIndex]);
                var resource = startIndex + 1 < segments.Length ? ToPascalCase(segments[startIndex + 1]) : "";
                return $"ServedClient.{module}.{resource}.{method}";
            }
        }

        return $"ServedClient.{method} {path}";
    }

    private static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Handle kebab-case
        var parts = input.Split('-');
        return string.Join("", parts.Select(p =>
            string.IsNullOrEmpty(p) ? "" : char.ToUpperInvariant(p[0]) + p.Substring(1).ToLowerInvariant()));
    }

    private static string? ExtractApiModule(Uri? uri)
    {
        if (uri == null)
            return null;

        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length >= 2 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
        {
            var startIndex = segments[1].StartsWith("v") ? 2 : 1;
            if (startIndex < segments.Length)
            {
                return segments[startIndex];
            }
        }

        return null;
    }

    private void InjectTraceContext(HttpRequestMessage request, ISpan span)
    {
        if (string.IsNullOrEmpty(span.TraceId))
            return;

        // W3C Trace Context headers
        request.Headers.TryAddWithoutValidation("traceparent", $"00-{span.TraceId}-{span.SpanId}-01");

        // Served-specific correlation header
        request.Headers.TryAddWithoutValidation("X-Served-Trace-Id", span.TraceId);
    }

    private bool ShouldCaptureHttpError(HttpStatusCode statusCode)
    {
        if (!_options.ErrorDetection.CaptureHttpErrors)
            return false;

        var code = (int)statusCode;

        // Check if this status code should be ignored
        if (_options.ErrorDetection.IgnoreStatusCodes.Contains(code))
            return false;

        return code >= 400;
    }

    private static string SanitizeUrl(string url)
    {
        // Remove sensitive query parameters
        try
        {
            var uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.Query))
                return url;

            // Parse and filter query parameters
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var sensitiveParams = new[] { "token", "key", "password", "secret", "auth", "apikey", "api_key" };

            foreach (var param in sensitiveParams)
            {
                if (queryParams[param] != null)
                {
                    queryParams[param] = "[REDACTED]";
                }
            }

            var builder = new UriBuilder(uri)
            {
                Query = queryParams.ToString()
            };

            return builder.ToString();
        }
        catch
        {
            return url;
        }
    }
}
