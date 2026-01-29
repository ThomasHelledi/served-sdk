using System.Diagnostics;
using System.Net;
using Served.SDK.Tracing;

namespace Served.SDK.Diagnostics;

/// <summary>
/// Unified healthcheck client for verifying service availability.
/// Integrates with ServedTracer for observability.
/// </summary>
public class HealthcheckClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly HealthcheckOptions _options;
    private readonly IServedTracer? _tracer;
    private readonly bool _ownsHttpClient;
    private bool _disposed;

    /// <summary>
    /// Creates a new HealthcheckClient with specified options.
    /// </summary>
    /// <param name="options">Healthcheck options (null uses defaults).</param>
    /// <param name="tracer">Optional tracer for observability.</param>
    /// <param name="httpClient">Optional HttpClient to use.</param>
    public HealthcheckClient(
        HealthcheckOptions? options = null,
        IServedTracer? tracer = null,
        HttpClient? httpClient = null)
    {
        _options = options ?? new HealthcheckOptions();
        _tracer = tracer;

        if (httpClient != null)
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _httpClient = CreateHttpClient();
            _ownsHttpClient = true;
        }
    }

    private HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 3
        };

        // Allow self-signed certs in development
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        return new HttpClient(handler)
        {
            Timeout = _options.Timeout
        };
    }

    /// <summary>
    /// Checks all configured endpoints and returns aggregate health status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Complete healthcheck result.</returns>
    public async Task<HealthcheckResult> CheckAllAsync(CancellationToken cancellationToken = default)
    {
        using var span = _options.EnableTracing && _tracer != null
            ? _tracer.StartSpan("healthcheck.all", SpanKind.Client)
            : null;

        var overallStopwatch = Stopwatch.StartNew();
        var endpoints = _options.CustomEndpoints;

        span?.SetAttribute("healthcheck.endpoint_count", endpoints.Count);
        span?.SetAttribute("healthcheck.parallel", _options.Parallel);

        IReadOnlyList<ComponentHealth> results;

        if (_options.Parallel && endpoints.Count > 1)
        {
            results = await CheckParallelAsync(endpoints, cancellationToken);
        }
        else
        {
            results = await CheckSequentialAsync(endpoints, cancellationToken);
        }

        overallStopwatch.Stop();

        var result = HealthcheckResult.FromComponents(
            results,
            overallStopwatch.ElapsedMilliseconds,
            span?.TraceId,
            span?.SpanId);

        // Set span attributes
        span?.SetAttribute("healthcheck.status", result.OverallStatus.ToString());
        span?.SetAttribute("healthcheck.healthy_count", result.HealthyCount);
        span?.SetAttribute("healthcheck.unhealthy_count", result.UnhealthyCount);
        span?.SetAttribute("healthcheck.total_duration_ms", result.TotalDurationMs);

        if (result.OverallStatus == HealthStatus.Unhealthy)
        {
            span?.SetError(true);
        }

        return result;
    }

    /// <summary>
    /// Checks a single endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Health status for the endpoint.</returns>
    public async Task<ComponentHealth> CheckAsync(
        ServiceEndpoint endpoint,
        CancellationToken cancellationToken = default)
    {
        using var span = _options.EnableTracing && _tracer != null
            ? _tracer.StartSpan($"healthcheck.{endpoint.Name}", SpanKind.Client)
            : null;

        span?.SetAttribute("healthcheck.service", endpoint.Name);
        span?.SetAttribute("healthcheck.url", endpoint.GetCheckUrl());

        var result = await CheckEndpointWithRetryAsync(endpoint, cancellationToken);

        span?.SetAttribute("healthcheck.status", result.Status.ToString());
        span?.SetAttribute("healthcheck.response_time_ms", result.ResponseTimeMs);

        if (result.HttpStatusCode.HasValue)
        {
            span?.SetAttribute("http.status_code", result.HttpStatusCode.Value);
        }

        if (result.Status == HealthStatus.Unhealthy)
        {
            span?.SetError(true);
            if (result.Error != null)
            {
                span?.SetAttribute("error.message", result.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Quick check if a single service is healthy.
    /// </summary>
    /// <param name="endpoint">The endpoint to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if healthy, false otherwise.</returns>
    public async Task<bool> IsHealthyAsync(
        ServiceEndpoint endpoint,
        CancellationToken cancellationToken = default)
    {
        var result = await CheckAsync(endpoint, cancellationToken);
        return result.Status == HealthStatus.Healthy;
    }

    /// <summary>
    /// Quick check if the local development environment is healthy.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all local services are healthy.</returns>
    public static async Task<HealthcheckResult> CheckLocalAsync(CancellationToken cancellationToken = default)
    {
        var options = HealthcheckOptions.Local();
        using var client = new HealthcheckClient(options);
        return await client.CheckAllAsync(cancellationToken);
    }

    /// <summary>
    /// Quick check if production environment is healthy.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all production services are healthy.</returns>
    public static async Task<HealthcheckResult> CheckProductionAsync(CancellationToken cancellationToken = default)
    {
        var options = HealthcheckOptions.Production();
        using var client = new HealthcheckClient(options);
        return await client.CheckAllAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<ComponentHealth>> CheckParallelAsync(
        List<ServiceEndpoint> endpoints,
        CancellationToken cancellationToken)
    {
        var tasks = endpoints.Select(e => CheckEndpointWithRetryAsync(e, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    private async Task<IReadOnlyList<ComponentHealth>> CheckSequentialAsync(
        List<ServiceEndpoint> endpoints,
        CancellationToken cancellationToken)
    {
        var results = new List<ComponentHealth>();
        foreach (var endpoint in endpoints)
        {
            var result = await CheckEndpointWithRetryAsync(endpoint, cancellationToken);
            results.Add(result);
        }
        return results;
    }

    private async Task<ComponentHealth> CheckEndpointWithRetryAsync(
        ServiceEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        var attempts = _options.RetryCount + 1;
        ComponentHealth? lastResult = null;

        for (int attempt = 0; attempt < attempts; attempt++)
        {
            if (attempt > 0)
            {
                await Task.Delay(_options.RetryDelay, cancellationToken);
            }

            lastResult = await CheckEndpointAsync(endpoint, cancellationToken);

            if (lastResult.Status == HealthStatus.Healthy)
            {
                return lastResult;
            }
        }

        return lastResult!;
    }

    private async Task<ComponentHealth> CheckEndpointAsync(
        ServiceEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var url = endpoint.GetCheckUrl();

        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            stopwatch.Stop();

            var statusCode = (int)response.StatusCode;
            var isExpectedStatus = endpoint.ExpectedStatusCodes.Contains(statusCode);

            var status = isExpectedStatus
                ? (stopwatch.ElapsedMilliseconds > _options.DegradedThresholdMs
                    ? HealthStatus.Degraded
                    : HealthStatus.Healthy)
                : HealthStatus.Unhealthy;

            return new ComponentHealth
            {
                Name = endpoint.Name,
                Url = url,
                Status = status,
                HttpStatusCode = statusCode,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                Error = isExpectedStatus ? null : $"Unexpected status: {statusCode}",
                Metadata = _options.IncludeMetadata ? new Dictionary<string, object>
                {
                    ["ContentLength"] = response.Content.Headers.ContentLength ?? 0,
                    ["ContentType"] = response.Content.Headers.ContentType?.MediaType ?? "unknown"
                } : null
            };
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            return new ComponentHealth
            {
                Name = endpoint.Name,
                Url = url,
                Status = HealthStatus.Unhealthy,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                Error = "Request timed out"
            };
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            return new ComponentHealth
            {
                Name = endpoint.Name,
                Url = url,
                Status = HealthStatus.Unhealthy,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                Error = SimplifyError(ex)
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ComponentHealth
            {
                Name = endpoint.Name,
                Url = url,
                Status = HealthStatus.Unhealthy,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                Error = ex.Message
            };
        }
    }

    private static string SimplifyError(HttpRequestException ex)
    {
        var message = ex.Message.ToLowerInvariant();

        if (message.Contains("connection refused"))
            return "Connection refused - service not running";

        if (message.Contains("no such host"))
            return "Host not found - DNS resolution failed";

        if (message.Contains("ssl") || message.Contains("certificate"))
            return "SSL/TLS error";

        if (message.Contains("network unreachable"))
            return "Network unreachable";

        return ex.Message.Split('\n')[0];
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
    }
}
