using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Served.SDK.Diagnostics;

/// <summary>
/// Unified startup probe for Kubernetes readiness/liveness and CLI monitoring.
/// Tracks startup phases, reports errors, and integrates with diagnostics.
/// </summary>
public class UnifiedStartupProbe
{
    private static readonly Lazy<UnifiedStartupProbe> _instance = new(() => new UnifiedStartupProbe());
    public static UnifiedStartupProbe Instance => _instance.Value;

    private static readonly string ProbeFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".served", "startup-probe.json");

    private readonly Stopwatch _startupTimer = Stopwatch.StartNew();
    private readonly ConcurrentQueue<StartupEvent> _events = new();
    private readonly object _lock = new();

    /// <summary>
    /// Current startup phase
    /// </summary>
    public StartupPhase Phase { get; private set; } = StartupPhase.Initializing;

    /// <summary>
    /// Whether startup completed successfully
    /// </summary>
    public bool IsReady => Phase == StartupPhase.Ready;

    /// <summary>
    /// Whether the application is alive (not crashed)
    /// </summary>
    public bool IsAlive => Phase != StartupPhase.Failed;

    /// <summary>
    /// Current error if startup failed
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Detailed error information for CLI
    /// </summary>
    public StartupError? ErrorDetails { get; private set; }

    /// <summary>
    /// Time elapsed since startup began
    /// </summary>
    public TimeSpan Elapsed => _startupTimer.Elapsed;

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public int Progress => Phase switch
    {
        StartupPhase.Initializing => 5,
        StartupPhase.ConfiguringServices => 20,
        StartupPhase.BuildingApp => 40,
        StartupPhase.RunningMigrations => 60,
        StartupPhase.RegisteringEndpoints => 75,
        StartupPhase.StartingServices => 85,
        StartupPhase.WarmingUp => 95,
        StartupPhase.Ready => 100,
        StartupPhase.Failed => 0,
        _ => 0
    };

    /// <summary>
    /// Advance to the next startup phase
    /// </summary>
    public void SetPhase(StartupPhase phase, string? message = null)
    {
        lock (_lock)
        {
            var previousPhase = Phase;
            Phase = phase;

            _events.Enqueue(new StartupEvent
            {
                Phase = phase,
                Message = message ?? $"Entered {phase} phase",
                Timestamp = DateTime.UtcNow,
                ElapsedMs = _startupTimer.ElapsedMilliseconds
            });

            // Save state for CLI monitoring
            SaveStateAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Report startup failure with detailed error information
    /// </summary>
    public void ReportFailure(Exception ex, string context = "Startup")
    {
        lock (_lock)
        {
            Phase = StartupPhase.Failed;
            Error = ex.Message;

            ErrorDetails = new StartupError
            {
                Context = context,
                Message = ex.Message,
                ExceptionType = ex.GetType().FullName ?? ex.GetType().Name,
                StackTrace = ex.StackTrace,
                InnerError = ex.InnerException?.Message,
                Timestamp = DateTime.UtcNow,
                SuggestedFix = GetSuggestedFix(ex)
            };

            _events.Enqueue(new StartupEvent
            {
                Phase = StartupPhase.Failed,
                Message = $"FATAL: {ex.Message}",
                Timestamp = DateTime.UtcNow,
                ElapsedMs = _startupTimer.ElapsedMilliseconds,
                IsError = true
            });

            // Save immediately for CLI to detect
            SaveStateAsync().Wait();
        }
    }

    /// <summary>
    /// Report a non-fatal warning during startup
    /// </summary>
    public void ReportWarning(string message, string? suggestedFix = null)
    {
        _events.Enqueue(new StartupEvent
        {
            Phase = Phase,
            Message = $"WARNING: {message}",
            Timestamp = DateTime.UtcNow,
            ElapsedMs = _startupTimer.ElapsedMilliseconds,
            IsWarning = true,
            SuggestedFix = suggestedFix
        });

        SaveStateAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Mark startup as complete
    /// </summary>
    public void MarkReady()
    {
        lock (_lock)
        {
            _startupTimer.Stop();
            Phase = StartupPhase.Ready;

            _events.Enqueue(new StartupEvent
            {
                Phase = StartupPhase.Ready,
                Message = $"Startup complete in {_startupTimer.ElapsedMilliseconds}ms",
                Timestamp = DateTime.UtcNow,
                ElapsedMs = _startupTimer.ElapsedMilliseconds
            });

            SaveStateAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Get current probe state for API responses
    /// </summary>
    public StartupProbeState GetState()
    {
        return new StartupProbeState
        {
            Phase = Phase,
            IsReady = IsReady,
            IsAlive = IsAlive,
            Progress = Progress,
            ElapsedMs = _startupTimer.ElapsedMilliseconds,
            Error = Error,
            ErrorDetails = ErrorDetails,
            Events = _events.ToArray()
        };
    }

    /// <summary>
    /// Save state to file for CLI monitoring
    /// </summary>
    private async Task SaveStateAsync()
    {
        try
        {
            var dir = Path.GetDirectoryName(ProbeFilePath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var state = GetState();
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });

            await File.WriteAllTextAsync(ProbeFilePath, json);
        }
        catch
        {
            // Silently fail - probe file is optional
        }
    }

    /// <summary>
    /// Load probe state from file (for CLI)
    /// </summary>
    public static async Task<StartupProbeState?> LoadStateAsync()
    {
        try
        {
            if (!File.Exists(ProbeFilePath))
                return null;

            var json = await File.ReadAllTextAsync(ProbeFilePath);
            return JsonSerializer.Deserialize<StartupProbeState>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            });
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Clear probe state file
    /// </summary>
    public static void ClearState()
    {
        try
        {
            if (File.Exists(ProbeFilePath))
                File.Delete(ProbeFilePath);
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>
    /// Get suggested fix based on exception type
    /// </summary>
    private static string? GetSuggestedFix(Exception ex)
    {
        var message = ex.Message.ToLowerInvariant();
        var typeName = ex.GetType().Name;

        // Route configuration errors
        if (message.Contains("catch-all parameter") || message.Contains("route template"))
            return "Check controllers with [ServedApi] attribute. Add SkipRouteGeneration=true for controllers with explicit [Route] attributes or catch-all parameters.";

        // Database errors
        if (message.Contains("connection") && (message.Contains("mysql") || message.Contains("database")))
            return "Check database connection string and ensure MySQL is running. Run: served db status";

        // Redis errors
        if (message.Contains("redis") || message.Contains("connectionmultiplexer"))
            return "Check Redis connection. Run: served redis status";

        // Migration errors
        if (message.Contains("migration") || message.Contains("table") && message.Contains("exists"))
            return "Database migration issue. Try: served db migrate --force";

        // Assembly/type loading errors
        if (typeName.Contains("TypeLoad") || typeName.Contains("ReflectionTypeLoad"))
            return "Assembly loading error. Check for missing NuGet packages or version mismatches. Run: dotnet restore";

        // Cast/type errors
        if (typeName == "InvalidCastException")
            return "Type casting error. Check generic type constraints and use OfType<T>() instead of Cast<T>() where appropriate.";

        return null;
    }
}

/// <summary>
/// Startup phases for tracking progress
/// </summary>
public enum StartupPhase
{
    Initializing,
    ConfiguringServices,
    BuildingApp,
    RunningMigrations,
    RegisteringEndpoints,
    StartingServices,
    WarmingUp,
    Ready,
    Failed
}

/// <summary>
/// Startup event for tracking progress
/// </summary>
public class StartupEvent
{
    public StartupPhase Phase { get; set; }
    public string Message { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public long ElapsedMs { get; set; }
    public bool IsError { get; set; }
    public bool IsWarning { get; set; }
    public string? SuggestedFix { get; set; }
}

/// <summary>
/// Detailed error information for CLI display
/// </summary>
public class StartupError
{
    public string Context { get; set; } = "";
    public string Message { get; set; } = "";
    public string? ExceptionType { get; set; }
    public string? StackTrace { get; set; }
    public string? InnerError { get; set; }
    public DateTime Timestamp { get; set; }
    public string? SuggestedFix { get; set; }
}

/// <summary>
/// Complete probe state for serialization
/// </summary>
public class StartupProbeState
{
    public StartupPhase Phase { get; set; }
    public bool IsReady { get; set; }
    public bool IsAlive { get; set; }
    public int Progress { get; set; }
    public long ElapsedMs { get; set; }
    public string? Error { get; set; }
    public StartupError? ErrorDetails { get; set; }
    public StartupEvent[] Events { get; set; } = Array.Empty<StartupEvent>();
}
