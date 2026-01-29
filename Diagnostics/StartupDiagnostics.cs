using System.Text.Json;

namespace Served.SDK.Diagnostics;

/// <summary>
/// Startup diagnostics for Served.SDK to report issues to CLI
/// </summary>
public class StartupDiagnostics
{
    private static readonly string DiagnosticsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".served", "sdk-diagnostics.json");

    /// <summary>
    /// Whether the SDK started successfully
    /// </summary>
    public bool IsHealthy { get; set; } = true;

    /// <summary>
    /// List of issues encountered during startup
    /// </summary>
    public List<DiagnosticIssue> Issues { get; set; } = new();

    /// <summary>
    /// Service statuses
    /// </summary>
    public Dictionary<string, ServiceStatus> Services { get; set; } = new();

    /// <summary>
    /// Time taken to start up
    /// </summary>
    public TimeSpan StartupTime { get; set; }

    /// <summary>
    /// Timestamp when diagnostics were captured
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Report an issue encountered during startup
    /// </summary>
    public void ReportIssue(string code, string message, IssueSeverity severity, string? suggestedFix = null)
    {
        Issues.Add(new DiagnosticIssue
        {
            Code = code,
            Message = message,
            Severity = severity,
            SuggestedFix = suggestedFix
        });

        if (severity == IssueSeverity.Error)
            IsHealthy = false;
    }

    /// <summary>
    /// Report service status
    /// </summary>
    public void ReportServiceStatus(string serviceName, bool isHealthy, string? error = null)
    {
        Services[serviceName] = new ServiceStatus
        {
            Name = serviceName,
            IsHealthy = isHealthy,
            Error = error,
            CheckedAt = DateTime.UtcNow
        };

        if (!isHealthy)
        {
            ReportIssue(
                $"SERVICE_{serviceName.ToUpperInvariant()}_UNHEALTHY",
                error ?? $"Service {serviceName} is not healthy",
                IssueSeverity.Error,
                $"Check {serviceName} logs and configuration");
        }
    }

    /// <summary>
    /// Save diagnostics to file for CLI to read
    /// </summary>
    public async Task SaveAsync()
    {
        try
        {
            var dir = Path.GetDirectoryName(DiagnosticsPath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await File.WriteAllTextAsync(DiagnosticsPath, json);
        }
        catch
        {
            // Silently fail - diagnostics are optional
        }
    }

    /// <summary>
    /// Load diagnostics from file
    /// </summary>
    public static async Task<StartupDiagnostics?> LoadAsync()
    {
        try
        {
            if (!File.Exists(DiagnosticsPath))
                return null;

            var json = await File.ReadAllTextAsync(DiagnosticsPath);
            return JsonSerializer.Deserialize<StartupDiagnostics>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Clear diagnostics file
    /// </summary>
    public static void Clear()
    {
        try
        {
            if (File.Exists(DiagnosticsPath))
                File.Delete(DiagnosticsPath);
        }
        catch
        {
            // Silently fail
        }
    }
}

/// <summary>
/// A diagnostic issue reported by the SDK
/// </summary>
public class DiagnosticIssue
{
    /// <summary>
    /// Error code (e.g., "DB_CONNECTION_FAILED")
    /// </summary>
    public string Code { get; set; } = "";

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Severity of the issue
    /// </summary>
    public IssueSeverity Severity { get; set; }

    /// <summary>
    /// Suggested fix for the issue
    /// </summary>
    public string? SuggestedFix { get; set; }
}

/// <summary>
/// Status of a service
/// </summary>
public class ServiceStatus
{
    /// <summary>
    /// Service name
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Whether the service is healthy
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Error message if unhealthy
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// When the status was checked
    /// </summary>
    public DateTime CheckedAt { get; set; }
}

/// <summary>
/// Severity of a diagnostic issue
/// </summary>
public enum IssueSeverity
{
    Info,
    Warning,
    Error
}
