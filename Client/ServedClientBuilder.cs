using Microsoft.Extensions.Logging;
using Served.SDK.Tracing;

namespace Served.SDK.Client;

/// <summary>
/// Fluent builder for creating ServedClient instances with advanced configuration.
/// </summary>
/// <example>
/// var client = new ServedClientBuilder()
///     .WithToken(token)
///     .WithTenant("my-workspace")
///     .WithTracing(options =>
///     {
///         options.EnableForge = true;
///         options.ServiceName = "my-application";
///     })
///     .Build();
/// </example>
public class ServedClientBuilder
{
    private string _baseUrl = ServedClient.GetConfiguredApiUrl();
    private string? _token;
    private string? _tenant;
    private HttpClient? _httpClient;
    private TracingOptions? _tracingOptions;
    private ILogger? _logger;
    private TimeSpan? _timeout;
    private Dictionary<string, string> _defaultHeaders = new();

    /// <summary>
    /// Set the base URL for the Served API.
    /// </summary>
    /// <param name="baseUrl">Base URL (e.g., https://api.served.dk)</param>
    public ServedClientBuilder WithBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        return this;
    }

    /// <summary>
    /// Set the authentication token.
    /// </summary>
    /// <param name="token">Bearer token for authentication.</param>
    public ServedClientBuilder WithToken(string token)
    {
        _token = token;
        return this;
    }

    /// <summary>
    /// Set the tenant/workspace context.
    /// </summary>
    /// <param name="tenant">Tenant slug or ID.</param>
    public ServedClientBuilder WithTenant(string tenant)
    {
        _tenant = tenant;
        return this;
    }

    /// <summary>
    /// Use a custom HttpClient instead of creating one.
    /// </summary>
    /// <param name="httpClient">Pre-configured HttpClient.</param>
    public ServedClientBuilder WithHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        return this;
    }

    /// <summary>
    /// Enable tracing and observability.
    /// </summary>
    /// <param name="configure">Optional configuration action for tracing options.</param>
    public ServedClientBuilder WithTracing(Action<TracingOptions>? configure = null)
    {
        _tracingOptions = new TracingOptions();
        configure?.Invoke(_tracingOptions);
        return this;
    }

    /// <summary>
    /// Set a logger for SDK operations.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public ServedClientBuilder WithLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    /// <summary>
    /// Set the default request timeout.
    /// </summary>
    /// <param name="timeout">Request timeout.</param>
    public ServedClientBuilder WithTimeout(TimeSpan timeout)
    {
        _timeout = timeout;
        return this;
    }

    /// <summary>
    /// Add a default header to all requests.
    /// </summary>
    /// <param name="name">Header name.</param>
    /// <param name="value">Header value.</param>
    public ServedClientBuilder WithDefaultHeader(string name, string value)
    {
        _defaultHeaders[name] = value;
        return this;
    }

    /// <summary>
    /// Build the configured ServedClient instance.
    /// </summary>
    /// <returns>Configured ServedClient.</returns>
    /// <exception cref="ArgumentException">Thrown when base URL is not set.</exception>
    public ServedClient Build()
    {
        if (string.IsNullOrEmpty(_baseUrl))
            throw new ArgumentException("Base URL is required. Call WithBaseUrl() first.");

        HttpClient httpClient;

        if (_httpClient != null)
        {
            httpClient = _httpClient;
        }
        else if (_tracingOptions != null)
        {
            // Create traced HttpClient
            var tracer = new ServedTracer(_tracingOptions);
            var handler = new TracingHttpHandler(tracer);
            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }
        else
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        // Apply timeout
        if (_timeout.HasValue)
        {
            httpClient.Timeout = _timeout.Value;
        }

        // Apply default headers
        foreach (var header in _defaultHeaders)
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        var client = new ServedClient(_baseUrl, _token, _tenant, httpClient);

        // Enable tracing on the client if configured
        if (_tracingOptions != null)
        {
            client.EnableTracing(_tracingOptions);
        }

        return client;
    }
}

/// <summary>
/// Extension methods for ServedClient tracing.
/// </summary>
public static class ServedClientExtensions
{
    /// <summary>
    /// Enable tracing on an existing ServedClient.
    /// </summary>
    /// <param name="client">The client to enable tracing on.</param>
    /// <param name="options">Optional tracing options. If null, defaults are used with environment auto-detection.</param>
    /// <returns>The client with tracing enabled.</returns>
    public static ServedClient EnableTracing(this ServedClient client, TracingOptions? options = null)
    {
        options ??= new TracingOptions();

        // Auto-detect configuration from environment
        var forgeKey = Environment.GetEnvironmentVariable("FORGE_API_KEY");
        if (string.IsNullOrEmpty(options.ForgeApiKey) && !string.IsNullOrEmpty(forgeKey))
        {
            options.ForgeApiKey = forgeKey;
            options.EnableForge = true;
        }

        var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        if (string.IsNullOrEmpty(options.OtlpEndpoint) && !string.IsNullOrEmpty(otlpEndpoint))
        {
            options.OtlpEndpoint = otlpEndpoint;
            options.EnableOtlp = true;
        }

        // Enable based on available configuration
        if (!string.IsNullOrEmpty(options.ForgeApiKey))
            options.EnableForge = true;

        if (!string.IsNullOrEmpty(options.OtlpEndpoint))
            options.EnableOtlp = true;

        var tracer = new ServedTracer(options);
        client.SetTracer(tracer);

        return client;
    }
}
