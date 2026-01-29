using System.Reflection;
using System.Runtime.InteropServices;

namespace Served.SDK.Diagnostics;

/// <summary>
/// Provides SDK version and capability information for CLI service discovery
/// </summary>
public static class SdkInfo
{
    /// <summary>
    /// SDK package name
    /// </summary>
    public static string Name => "Served.SDK";

    /// <summary>
    /// SDK version from assembly
    /// </summary>
    public static string Version
    {
        get
        {
            var assembly = typeof(SdkInfo).Assembly;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString()
                ?? "1.0.0";
            // Remove +commit hash if present
            var plusIndex = version.IndexOf('+');
            return plusIndex > 0 ? version[..plusIndex] : version;
        }
    }

    /// <summary>
    /// .NET runtime information
    /// </summary>
    public static string Runtime => RuntimeInformation.FrameworkDescription;

    /// <summary>
    /// SDK features/capabilities for CLI detection
    /// </summary>
    public static string[] Features => new[]
    {
        "startup-probe",      // Kubernetes startup/readiness/liveness probes
        "tracing",            // OpenTelemetry tracing
        "error-categories",   // Categorized error reporting
        "telemetry-events",   // Structured telemetry
        "health-endpoints",   // /health/* endpoints
        "sdk-discovery"       // This SDK info endpoint
    };

    /// <summary>
    /// Check if a specific feature is available
    /// </summary>
    public static bool HasFeature(string feature) => Features.Contains(feature, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Header name used for SDK detection in HTTP responses
    /// </summary>
    public const string HeaderName = "X-Served-SDK";

    /// <summary>
    /// Service ID header name
    /// </summary>
    public const string ServiceIdHeader = "X-Service-Id";

    /// <summary>
    /// Service profile header name
    /// </summary>
    public const string ServiceProfileHeader = "X-Service-Profile";
}
