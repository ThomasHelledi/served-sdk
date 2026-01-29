using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Served.SDK.Infrastructure;

/// <summary>
/// Infrastructure validator for Served platform
/// Validates git, ingress, services, and version consistency
/// </summary>
public class InfrastructureValidator
{
    private readonly InfrastructureValidatorOptions _options;
    private readonly HttpClient _httpClient;

    public InfrastructureValidator(InfrastructureValidatorOptions? options = null)
    {
        _options = options ?? new InfrastructureValidatorOptions();
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds)
        };
    }

    /// <summary>
    /// Run full infrastructure validation
    /// </summary>
    public async Task<InfraValidationResult> ValidateAsync(CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        var result = new InfraValidationResult
        {
            ValidationId = $"val_{startTime:yyyyMMddHHmmssfff}",
            Target = _options.Target,
            StartedAt = startTime
        };

        // Run validations in parallel where possible
        async Task<GitValidationResult?> RunGitValidation() => _options.ValidateGit ? await ValidateGitAsync(ct) : null;
        async Task<List<IngressValidationResult>?> RunIngressValidation() => _options.ValidateIngress ? await ValidateIngressAsync(ct) : null;
        async Task<List<ServiceHealthValidationResult>?> RunServicesValidation() => _options.ValidateServices ? await ValidateServicesAsync(ct) : null;

        var gitTask = RunGitValidation();
        var ingressTask = RunIngressValidation();
        var servicesTask = RunServicesValidation();

        await Task.WhenAll(gitTask, ingressTask, servicesTask);

        result.Git = await gitTask;
        result.Ingress = await ingressTask;
        result.Services = await servicesTask;

        // Version consistency depends on services
        if (_options.ValidateVersions && result.Services?.Any() == true)
        {
            result.VersionConsistency = ValidateVersionConsistency(result.Services);
        }

        result.CompletedAt = DateTime.UtcNow;
        result.Duration = result.CompletedAt - result.StartedAt;
        result.CalculateSummary();

        return result;
    }

    /// <summary>
    /// Validate Git configuration
    /// </summary>
    public async Task<GitValidationResult> ValidateGitAsync(CancellationToken ct = default)
    {
        var result = new GitValidationResult();
        var sw = Stopwatch.StartNew();

        try
        {
            // Get current branch
            result.CurrentBranch = await ExecuteGitCommandAsync("rev-parse --abbrev-ref HEAD", ct);

            // Get remotes
            var remotesOutput = await ExecuteGitCommandAsync("remote -v", ct);
            result.Remotes = ParseRemotes(remotesOutput);

            // Check working tree
            var statusOutput = await ExecuteGitCommandAsync("status --porcelain", ct);
            result.WorkingTreeClean = string.IsNullOrWhiteSpace(statusOutput);
            result.UncommittedChanges = statusOutput?.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length ?? 0;

            // Check ahead/behind
            var aheadBehind = await ExecuteGitCommandAsync(
                $"rev-list --left-right --count origin/{result.CurrentBranch}...HEAD 2>/dev/null || echo '0\t0'", ct);
            var counts = aheadBehind?.Split('\t') ?? Array.Empty<string>();
            if (counts.Length >= 2)
            {
                result.CommitsBehind = int.TryParse(counts[0], out var b) ? b : 0;
                result.CommitsAhead = int.TryParse(counts[1], out var a) ? a : 0;
            }

            // Last commit info
            var lastCommit = await ExecuteGitCommandAsync("log -1 --format='%h|%s|%an|%ar'", ct);
            var parts = lastCommit?.Split('|') ?? Array.Empty<string>();
            if (parts.Length >= 4)
            {
                result.LastCommitHash = parts[0].Trim('\'');
                result.LastCommitMessage = parts[1];
                result.LastCommitAuthor = parts[2];
                result.LastCommitTime = parts[3].Trim('\'');
            }

            result.Valid = true;
            result.Warnings = new List<string>();

            // Validate against expectations
            foreach (var expectedRemote in _options.ExpectedRemotes)
            {
                if (!result.Remotes.ContainsKey(expectedRemote))
                {
                    result.Valid = false;
                    result.Warnings.Add($"Missing remote: {expectedRemote}");
                }
            }

            if (!result.WorkingTreeClean)
                result.Warnings.Add($"{result.UncommittedChanges} uncommitted changes");

            if (result.CommitsBehind > 0)
                result.Warnings.Add($"{result.CommitsBehind} commits behind origin");
        }
        catch (Exception ex)
        {
            result.Valid = false;
            result.Error = ex.Message;
        }

        sw.Stop();
        result.DurationMs = sw.ElapsedMilliseconds;
        return result;
    }

    /// <summary>
    /// Validate ingress endpoints
    /// </summary>
    public async Task<List<IngressValidationResult>> ValidateIngressAsync(CancellationToken ct = default)
    {
        var results = new List<IngressValidationResult>();

        foreach (var endpoint in _options.Endpoints)
        {
            var result = await ValidateEndpointAsync(endpoint, ct);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// Validate service health
    /// </summary>
    public async Task<List<ServiceHealthValidationResult>> ValidateServicesAsync(CancellationToken ct = default)
    {
        var results = new List<ServiceHealthValidationResult>();

        foreach (var endpoint in _options.Endpoints)
        {
            var result = await ValidateServiceHealthAsync(endpoint, ct);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// Validate a single endpoint
    /// </summary>
    public async Task<IngressValidationResult> ValidateEndpointAsync(
        ServiceEndpoint endpoint, CancellationToken ct = default)
    {
        var result = new IngressValidationResult
        {
            Name = endpoint.Name,
            Url = endpoint.Url
        };

        var sw = Stopwatch.StartNew();

        try
        {
            var url = endpoint.Url.StartsWith("http") ? endpoint.Url : $"https://{endpoint.Url}";
            var response = await _httpClient.GetAsync(url, ct);
            sw.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.ResponseTimeMs = sw.ElapsedMilliseconds;
            result.Healthy = response.IsSuccessStatusCode;

            // Extract X-Application headers
            if (response.Headers.TryGetValues("X-Application-Name", out var names))
                result.ApplicationName = names.FirstOrDefault();
            if (response.Headers.TryGetValues("X-Application-Version", out var versions))
                result.ApplicationVersion = versions.FirstOrDefault();

            // Determine severity
            result.Severity = result.ResponseTimeMs > _options.CriticalResponseTimeMs ? "Critical"
                : result.ResponseTimeMs > _options.WarningResponseTimeMs ? "Warning" : "Ok";

            // Check SSL if configured
            if (!_options.QuickMode && url.StartsWith("https"))
            {
                result.SslInfo = await CheckSslAsync(new Uri(url).Host, ct);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            result.ResponseTimeMs = sw.ElapsedMilliseconds;
            result.Healthy = false;
            result.Error = ex.Message;
            result.Severity = "Critical";
        }

        return result;
    }

    /// <summary>
    /// Validate service health
    /// </summary>
    public async Task<ServiceHealthValidationResult> ValidateServiceHealthAsync(
        ServiceEndpoint endpoint, CancellationToken ct = default)
    {
        var result = new ServiceHealthValidationResult
        {
            Name = endpoint.Name,
            Url = endpoint.Url
        };

        var sw = Stopwatch.StartNew();

        try
        {
            var healthUrl = endpoint.Url.TrimEnd('/') + (endpoint.HealthPath ?? "/healthz/startup");
            if (!healthUrl.StartsWith("http"))
                healthUrl = $"https://{healthUrl}";

            var response = await _httpClient.GetAsync(healthUrl, ct);
            sw.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.ResponseTimeMs = sw.ElapsedMilliseconds;
            result.Healthy = response.IsSuccessStatusCode;

            // Extract headers
            if (response.Headers.TryGetValues("X-Application-Name", out var names))
                result.ApplicationName = names.FirstOrDefault();
            if (response.Headers.TryGetValues("X-Application-Version", out var versions))
                result.ApplicationVersion = versions.FirstOrDefault();

            // Parse health response
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var body = await response.Content.ReadAsStringAsync(ct);
                    var health = JsonSerializer.Deserialize<HealthResponse>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    result.Version = health?.Version ?? result.ApplicationVersion;
                    result.Status = health?.Status ?? "Healthy";
                    result.Uptime = health?.Uptime;
                }
                catch { /* Body parsing is optional */ }
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            result.ResponseTimeMs = sw.ElapsedMilliseconds;
            result.Healthy = false;
            result.Error = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Validate version consistency across services
    /// </summary>
    public VersionConsistencyResult ValidateVersionConsistency(List<ServiceHealthValidationResult> services)
    {
        var result = new VersionConsistencyResult();

        var versions = services
            .Where(s => !string.IsNullOrEmpty(s.ApplicationVersion))
            .Select(s => s.ApplicationVersion!)
            .Distinct()
            .ToList();

        result.Consistent = versions.Count <= 1;
        result.Versions = versions;

        if (versions.Any())
        {
            result.ExpectedVersion = services
                .Where(s => !string.IsNullOrEmpty(s.ApplicationVersion))
                .GroupBy(s => s.ApplicationVersion)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            result.ServicesWithMismatch = services
                .Where(s => !string.IsNullOrEmpty(s.ApplicationVersion) && s.ApplicationVersion != result.ExpectedVersion)
                .Select(s => s.Name)
                .ToList();
        }

        return result;
    }

    private async Task<string?> ExecuteGitCommandAsync(string args, CancellationToken ct)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return null;

            var output = await process.StandardOutput.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            return output.Trim();
        }
        catch
        {
            return null;
        }
    }

    private Dictionary<string, string> ParseRemotes(string? output)
    {
        var remotes = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(output)) return remotes;

        foreach (var line in output.Split('\n').Where(l => l.Contains("(fetch)")))
        {
            var parts = line.Split('\t');
            if (parts.Length >= 2)
            {
                var name = parts[0].Trim();
                var url = parts[1].Replace("(fetch)", "").Trim();
                remotes[name] = url;
            }
        }

        return remotes;
    }

    private async Task<SslValidationInfo?> CheckSslAsync(string host, CancellationToken ct)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"echo | openssl s_client -connect {host}:443 -servername {host} 2>/dev/null | openssl x509 -noout -dates 2>/dev/null\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return null;

            var output = await process.StandardOutput.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            if (string.IsNullOrEmpty(output)) return null;

            var info = new SslValidationInfo();
            foreach (var line in output.Split('\n'))
            {
                if (line.StartsWith("notAfter="))
                {
                    var dateStr = line.Replace("notAfter=", "").Trim();
                    if (DateTime.TryParse(dateStr, out var validUntil))
                    {
                        info.ValidUntil = validUntil;
                        info.DaysUntilExpiry = (int)(validUntil - DateTime.UtcNow).TotalDays;
                        info.Severity = info.DaysUntilExpiry < _options.CriticalCertExpiryDays ? "Critical"
                            : info.DaysUntilExpiry < _options.WarningCertExpiryDays ? "Warning" : "Ok";
                    }
                }
            }

            return info;
        }
        catch
        {
            return null;
        }
    }
}

#region Configuration

/// <summary>
/// Options for infrastructure validator
/// </summary>
public class InfrastructureValidatorOptions
{
    /// <summary>
    /// Validation target name
    /// </summary>
    public string Target { get; set; } = "production";

    /// <summary>
    /// Endpoints to validate
    /// </summary>
    public List<ServiceEndpoint> Endpoints { get; set; } = new()
    {
        new ServiceEndpoint { Name = "ServedApi", Url = "https://api.served.dk", HealthPath = "/healthz/startup" },
        new ServiceEndpoint { Name = "ServedApp", Url = "https://app.served.dk", HealthPath = "/healthz/startup" }
    };

    /// <summary>
    /// Expected Git remotes
    /// </summary>
    public List<string> ExpectedRemotes { get; set; } = new() { "origin", "gitlab" };

    /// <summary>
    /// Whether to validate Git
    /// </summary>
    public bool ValidateGit { get; set; } = true;

    /// <summary>
    /// Whether to validate ingress
    /// </summary>
    public bool ValidateIngress { get; set; } = true;

    /// <summary>
    /// Whether to validate services
    /// </summary>
    public bool ValidateServices { get; set; } = true;

    /// <summary>
    /// Whether to validate version consistency
    /// </summary>
    public bool ValidateVersions { get; set; } = true;

    /// <summary>
    /// Quick mode - skip slow checks
    /// </summary>
    public bool QuickMode { get; set; } = false;

    /// <summary>
    /// HTTP timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 15;

    /// <summary>
    /// Response time warning threshold (ms)
    /// </summary>
    public int WarningResponseTimeMs { get; set; } = 500;

    /// <summary>
    /// Response time critical threshold (ms)
    /// </summary>
    public int CriticalResponseTimeMs { get; set; } = 2000;

    /// <summary>
    /// Certificate expiry warning threshold (days)
    /// </summary>
    public int WarningCertExpiryDays { get; set; } = 30;

    /// <summary>
    /// Certificate expiry critical threshold (days)
    /// </summary>
    public int CriticalCertExpiryDays { get; set; } = 7;
}

/// <summary>
/// Service endpoint configuration
/// </summary>
public class ServiceEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? HealthPath { get; set; }
    public Dictionary<string, string>? ExpectedHeaders { get; set; }
}

#endregion

#region Results

/// <summary>
/// Complete infrastructure validation result
/// </summary>
public class InfraValidationResult
{
    public string ValidationId { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public TimeSpan Duration { get; set; }

    public GitValidationResult? Git { get; set; }
    public List<IngressValidationResult>? Ingress { get; set; }
    public List<ServiceHealthValidationResult>? Services { get; set; }
    public VersionConsistencyResult? VersionConsistency { get; set; }
    public ValidationSummary? Summary { get; set; }

    [JsonIgnore]
    public bool Success => Summary?.Success ?? false;

    public void CalculateSummary()
    {
        Summary = new ValidationSummary();

        if (Git != null)
        {
            Summary.TotalChecks++;
            if (Git.Valid && !(Git.Warnings?.Any() ?? false))
                Summary.PassedChecks++;
            else if (Git.Valid)
                Summary.WarningChecks++;
            else
                Summary.FailedChecks++;
        }

        if (Ingress != null)
        {
            foreach (var ing in Ingress)
            {
                Summary.TotalChecks++;
                if (ing.Healthy && ing.Severity == "Ok")
                    Summary.PassedChecks++;
                else if (ing.Healthy)
                    Summary.WarningChecks++;
                else
                    Summary.FailedChecks++;
            }
        }

        if (Services != null)
        {
            foreach (var svc in Services)
            {
                Summary.TotalChecks++;
                if (svc.Healthy)
                    Summary.PassedChecks++;
                else
                    Summary.FailedChecks++;
            }
        }

        if (VersionConsistency != null)
        {
            Summary.TotalChecks++;
            if (VersionConsistency.Consistent)
                Summary.PassedChecks++;
            else
                Summary.WarningChecks++;
        }

        Summary.Success = Summary.FailedChecks == 0;
    }
}

public class ValidationSummary
{
    public int TotalChecks { get; set; }
    public int PassedChecks { get; set; }
    public int WarningChecks { get; set; }
    public int FailedChecks { get; set; }
    public bool Success { get; set; }
}

public class GitValidationResult
{
    public bool Valid { get; set; }
    public string? CurrentBranch { get; set; }
    public Dictionary<string, string>? Remotes { get; set; }
    public bool WorkingTreeClean { get; set; }
    public int UncommittedChanges { get; set; }
    public int CommitsAhead { get; set; }
    public int CommitsBehind { get; set; }
    public string? LastCommitHash { get; set; }
    public string? LastCommitMessage { get; set; }
    public string? LastCommitAuthor { get; set; }
    public string? LastCommitTime { get; set; }
    public List<string>? Warnings { get; set; }
    public string? Error { get; set; }
    public long DurationMs { get; set; }
}

public class IngressValidationResult
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool Healthy { get; set; }
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? Severity { get; set; }
    public string? ApplicationName { get; set; }
    public string? ApplicationVersion { get; set; }
    public SslValidationInfo? SslInfo { get; set; }
    public string? Error { get; set; }
}

public class SslValidationInfo
{
    public DateTime? ValidUntil { get; set; }
    public int? DaysUntilExpiry { get; set; }
    public string? Severity { get; set; }
    public string? Issuer { get; set; }
}

public class ServiceHealthValidationResult
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool Healthy { get; set; }
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? Version { get; set; }
    public string? ApplicationName { get; set; }
    public string? ApplicationVersion { get; set; }
    public string? Status { get; set; }
    public string? Uptime { get; set; }
    public string? Error { get; set; }
}

public class VersionConsistencyResult
{
    public bool Consistent { get; set; }
    public string? ExpectedVersion { get; set; }
    public List<string>? Versions { get; set; }
    public List<string>? ServicesWithMismatch { get; set; }
}

/// <summary>
/// Health response from /healthz endpoint
/// </summary>
internal class HealthResponse
{
    public string? Status { get; set; }
    public string? Version { get; set; }
    public string? Uptime { get; set; }
}

#endregion
