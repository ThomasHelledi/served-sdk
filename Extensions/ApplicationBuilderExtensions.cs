using Microsoft.AspNetCore.Builder;
using Served.SDK.Http;

namespace Served.SDK.Extensions;

/// <summary>
/// Extension methods for IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds middleware that includes X-Application-Name and X-Application-Version headers
    /// on all responses. This enables infrastructure validation to identify services.
    ///
    /// Usage:
    ///   app.UseApplicationInfo();
    ///
    /// Or with options:
    ///   app.UseApplicationInfo(options => {
    ///       options.ApplicationName = "MyService";
    ///       options.ApplicationVersion = "1.0.0";
    ///       options.IncludeEnvironment = true;
    ///   });
    /// </summary>
    public static IApplicationBuilder UseApplicationInfo(
        this IApplicationBuilder app,
        Action<ApplicationInfoOptions>? configure = null)
    {
        var options = new ApplicationInfoOptions();
        configure?.Invoke(options);

        return app.UseMiddleware<ApplicationInfoMiddleware>(options);
    }

    /// <summary>
    /// Adds middleware with explicit name and version.
    ///
    /// Usage:
    ///   app.UseApplicationInfo("ServedApi", "2026.1.306");
    /// </summary>
    public static IApplicationBuilder UseApplicationInfo(
        this IApplicationBuilder app,
        string applicationName,
        string? applicationVersion = null)
    {
        var options = new ApplicationInfoOptions
        {
            ApplicationName = applicationName,
            ApplicationVersion = applicationVersion
        };

        return app.UseMiddleware<ApplicationInfoMiddleware>(options);
    }
}
