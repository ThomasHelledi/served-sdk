using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Served.SDK.Http;

/// <summary>
/// Middleware that adds X-Application-Name and X-Application-Version headers to all responses.
/// This enables infrastructure validation to identify services and their versions.
/// </summary>
public class ApplicationInfoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _applicationName;
    private readonly string _applicationVersion;
    private readonly ApplicationInfoOptions _options;

    public ApplicationInfoMiddleware(
        RequestDelegate next,
        ApplicationInfoOptions? options = null)
    {
        _next = next;
        _options = options ?? new ApplicationInfoOptions();

        // Resolve application name
        _applicationName = _options.ApplicationName
            ?? Assembly.GetEntryAssembly()?.GetName().Name
            ?? "Unknown";

        // Resolve application version
        _applicationVersion = ResolveVersion();
    }

    private string ResolveVersion()
    {
        // 1. Use explicit version if provided
        if (!string.IsNullOrEmpty(_options.ApplicationVersion))
            return _options.ApplicationVersion;

        // 2. Try environment variable (set by CI/CD)
        var envVersion = Environment.GetEnvironmentVariable("APP_VERSION")
            ?? Environment.GetEnvironmentVariable("APPLICATION_VERSION");
        if (!string.IsNullOrEmpty(envVersion))
            return envVersion;

        // 3. Try assembly informational version (includes git hash)
        var assembly = Assembly.GetEntryAssembly();
        var infoVersion = assembly?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        if (!string.IsNullOrEmpty(infoVersion))
        {
            // Strip the +commitHash suffix if present
            var plusIndex = infoVersion.IndexOf('+');
            return plusIndex > 0 ? infoVersion[..plusIndex] : infoVersion;
        }

        // 4. Fall back to assembly version
        var version = assembly?.GetName().Version;
        if (version != null)
            return $"{version.Major}.{version.Minor}.{version.Build}";

        // 5. Use CalVer format as last resort
        return DateTime.UtcNow.ToString("yyyy.MM.dd");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add headers before response starts
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            // Always add these headers
            if (!headers.ContainsKey("X-Application-Name"))
                headers["X-Application-Name"] = _applicationName;

            if (!headers.ContainsKey("X-Application-Version"))
                headers["X-Application-Version"] = _applicationVersion;

            // Optional: Add build timestamp
            if (_options.IncludeBuildTime && !string.IsNullOrEmpty(_options.BuildTime))
                headers["X-Build-Time"] = _options.BuildTime;

            // Optional: Add environment
            if (_options.IncludeEnvironment)
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                    ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                    ?? "Production";
                headers["X-Environment"] = env;
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

/// <summary>
/// Options for configuring ApplicationInfoMiddleware
/// </summary>
public class ApplicationInfoOptions
{
    /// <summary>
    /// Application name. If not set, uses entry assembly name.
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// Application version. If not set, uses assembly version or APP_VERSION env var.
    /// </summary>
    public string? ApplicationVersion { get; set; }

    /// <summary>
    /// Build timestamp (ISO 8601 format)
    /// </summary>
    public string? BuildTime { get; set; }

    /// <summary>
    /// Include X-Build-Time header
    /// </summary>
    public bool IncludeBuildTime { get; set; }

    /// <summary>
    /// Include X-Environment header
    /// </summary>
    public bool IncludeEnvironment { get; set; }
}
