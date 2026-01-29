using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Served.SDK.Diagnostics;

/// <summary>
/// Extensions for integrating UnifiedStartupProbe with ASP.NET Core
/// </summary>
public static class StartupProbeExtensions
{
    /// <summary>
    /// Add startup probe services to the service collection
    /// </summary>
    public static IServiceCollection AddUnifiedStartupProbe(this IServiceCollection services)
    {
        // Clear any previous state
        UnifiedStartupProbe.ClearState();

        // Set initial phase
        UnifiedStartupProbe.Instance.SetPhase(StartupPhase.ConfiguringServices, "Adding services to container");

        // Add health checks that use the probe
        services.AddHealthChecks()
            .AddCheck("startup", () =>
            {
                var probe = UnifiedStartupProbe.Instance;
                if (probe.IsReady)
                    return HealthCheckResult.Healthy("Startup complete");
                if (!probe.IsAlive)
                    return HealthCheckResult.Unhealthy($"Startup failed: {probe.Error}");
                return HealthCheckResult.Degraded($"Starting up... Phase: {probe.Phase}, Progress: {probe.Progress}%");
            }, tags: new[] { "startup", "ready" });

        return services;
    }

    /// <summary>
    /// Map startup probe endpoints for Kubernetes
    /// </summary>
    public static IEndpointRouteBuilder MapStartupProbeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        };

        // Kubernetes readiness probe - returns 200 when ready to receive traffic
        endpoints.MapGet("/health/ready", async context =>
        {
            var probe = UnifiedStartupProbe.Instance;
            context.Response.ContentType = "application/json";

            if (probe.IsReady)
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "Ready",
                    phase = probe.Phase.ToString(),
                    elapsedMs = probe.Elapsed.TotalMilliseconds
                }, jsonOptions));
            }
            else
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "NotReady",
                    phase = probe.Phase.ToString(),
                    progress = probe.Progress,
                    error = probe.Error
                }, jsonOptions));
            }
        }).AllowAnonymous();

        // Kubernetes liveness probe - returns 200 if application is alive
        endpoints.MapGet("/health/live", async context =>
        {
            var probe = UnifiedStartupProbe.Instance;
            context.Response.ContentType = "application/json";

            if (probe.IsAlive)
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "Alive",
                    phase = probe.Phase.ToString()
                }, jsonOptions));
            }
            else
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "Dead",
                    error = probe.Error,
                    errorDetails = probe.ErrorDetails
                }, jsonOptions));
            }
        }).AllowAnonymous();

        // Kubernetes startup probe - for slow-starting containers
        endpoints.MapGet("/health/startup", async context =>
        {
            var probe = UnifiedStartupProbe.Instance;
            context.Response.ContentType = "application/json";

            // Startup probe succeeds once app is ready OR if it failed (let liveness handle that)
            if (probe.IsReady || !probe.IsAlive)
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = probe.IsReady ? "Started" : "Failed",
                    phase = probe.Phase.ToString(),
                    elapsedMs = probe.Elapsed.TotalMilliseconds,
                    error = probe.Error
                }, jsonOptions));
            }
            else
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "Starting",
                    phase = probe.Phase.ToString(),
                    progress = probe.Progress,
                    elapsedMs = probe.Elapsed.TotalMilliseconds
                }, jsonOptions));
            }
        }).AllowAnonymous();

        // Detailed probe state for CLI monitoring
        endpoints.MapGet("/health/probe", async context =>
        {
            var probe = UnifiedStartupProbe.Instance;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = probe.IsReady ? 200 : (probe.IsAlive ? 503 : 500);

            var state = probe.GetState();
            await context.Response.WriteAsync(JsonSerializer.Serialize(state, jsonOptions));
        }).AllowAnonymous();

        // SDK info endpoint for CLI service discovery
        endpoints.MapGet("/health/sdk", async context =>
        {
            var probe = UnifiedStartupProbe.Instance;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;

            // Add SDK header for easy detection
            context.Response.Headers["X-Served-SDK"] = SdkInfo.Version;

            var sdkInfo = new
            {
                sdk = new
                {
                    name = SdkInfo.Name,
                    version = SdkInfo.Version,
                    runtime = SdkInfo.Runtime,
                    features = SdkInfo.Features
                },
                service = new
                {
                    id = Environment.GetEnvironmentVariable("SERVICE_ID") ?? "unknown",
                    name = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? AppDomain.CurrentDomain.FriendlyName,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    startedAt = probe.GetState().Events.FirstOrDefault()?.Timestamp,
                    uptime = probe.Elapsed.TotalSeconds
                },
                probe = new
                {
                    phase = probe.Phase.ToString(),
                    isReady = probe.IsReady,
                    isAlive = probe.IsAlive,
                    progress = probe.Progress
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(sdkInfo, jsonOptions));
        }).AllowAnonymous();

        return endpoints;
    }

    /// <summary>
    /// Use startup probe middleware to catch unhandled startup exceptions
    /// </summary>
    public static IApplicationBuilder UseStartupProbe(this IApplicationBuilder app)
    {
        UnifiedStartupProbe.Instance.SetPhase(StartupPhase.BuildingApp, "Configuring middleware pipeline");
        return app;
    }

    /// <summary>
    /// Mark application as ready (call after all initialization is complete)
    /// </summary>
    public static void MarkStartupComplete(this IApplicationBuilder app)
    {
        UnifiedStartupProbe.Instance.MarkReady();
    }

    /// <summary>
    /// Wrap startup code with probe error handling
    /// </summary>
    public static void RunWithProbe(Action action, string context, StartupPhase phase)
    {
        try
        {
            UnifiedStartupProbe.Instance.SetPhase(phase, $"Starting: {context}");
            action();
        }
        catch (Exception ex)
        {
            UnifiedStartupProbe.Instance.ReportFailure(ex, context);
            throw;
        }
    }

    /// <summary>
    /// Wrap async startup code with probe error handling
    /// </summary>
    public static async Task RunWithProbeAsync(Func<Task> action, string context, StartupPhase phase)
    {
        try
        {
            UnifiedStartupProbe.Instance.SetPhase(phase, $"Starting: {context}");
            await action();
        }
        catch (Exception ex)
        {
            UnifiedStartupProbe.Instance.ReportFailure(ex, context);
            throw;
        }
    }
}
