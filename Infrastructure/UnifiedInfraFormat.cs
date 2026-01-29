using System.Text;
using System.Text.RegularExpressions;

namespace Served.SDK.Infrastructure;

/// <summary>
/// Unified infrastructure format (.unified.infra)
/// Compact, token-efficient format for infrastructure state
/// </summary>
public class UnifiedInfraFormat
{
    public int Version { get; set; } = 1;
    public string Target { get; set; } = "production";
    public DateTime ValidatedAt { get; set; }
    public UnifiedGitState Git { get; set; } = new();
    public List<UnifiedEndpointState> Endpoints { get; set; } = new();
    public UnifiedSummaryState Summary { get; set; } = new();
    public UnifiedThresholdState Thresholds { get; set; } = new();

    private static readonly string DefaultPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".served", "infra.unified.infra");

    /// <summary>
    /// Load from default or specified path
    /// </summary>
    public static UnifiedInfraFormat Load(string? path = null)
    {
        path ??= DefaultPath;

        if (!File.Exists(path))
            return new UnifiedInfraFormat();

        var content = File.ReadAllText(path);
        return Parse(content);
    }

    /// <summary>
    /// Save to default or specified path
    /// </summary>
    public void Save(string? path = null)
    {
        path ??= DefaultPath;

        var dir = Path.GetDirectoryName(path);
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var content = Serialize();
        File.WriteAllText(path, content);
    }

    /// <summary>
    /// Check if state is recent (within threshold)
    /// </summary>
    public bool IsRecent(TimeSpan? maxAge = null)
    {
        maxAge ??= TimeSpan.FromMinutes(5);
        return DateTime.UtcNow - ValidatedAt < maxAge;
    }

    /// <summary>
    /// Check if infrastructure is healthy
    /// </summary>
    public bool IsHealthy => Summary.Ok && Git.Clean;

    /// <summary>
    /// Update from validation result
    /// </summary>
    public void UpdateFrom(InfraValidationResult result)
    {
        ValidatedAt = result.CompletedAt;
        Target = result.Target;

        if (result.Git != null)
        {
            Git = new UnifiedGitState
            {
                Branch = result.Git.CurrentBranch ?? "",
                Remotes = result.Git.Remotes?.Keys.ToList() ?? new List<string>(),
                Clean = result.Git.WorkingTreeClean,
                Head = result.Git.LastCommitHash ?? "",
                Ahead = result.Git.CommitsAhead,
                Behind = result.Git.CommitsBehind
            };
        }

        Endpoints.Clear();
        if (result.Services != null)
        {
            foreach (var svc in result.Services)
            {
                var ep = new UnifiedEndpointState
                {
                    Name = svc.Name,
                    Url = svc.Url.Replace("https://", ""),
                    Status = svc.StatusCode,
                    Time = (int)svc.ResponseTimeMs,
                    Version = svc.ApplicationVersion
                };

                // Find SSL info from ingress
                var ingress = result.Ingress?.FirstOrDefault(i => i.Name == svc.Name);
                if (ingress?.SslInfo?.DaysUntilExpiry != null)
                    ep.SslDays = ingress.SslInfo.DaysUntilExpiry;

                Endpoints.Add(ep);
            }
        }

        Summary = new UnifiedSummaryState
        {
            Ok = result.Summary?.Success ?? false,
            Passed = result.Summary?.PassedChecks ?? 0,
            Warnings = result.Summary?.WarningChecks ?? 0,
            Failed = result.Summary?.FailedChecks ?? 0,
            Duration = (int)result.Duration.TotalMilliseconds,
            ValidationId = result.ValidationId
        };
    }

    /// <summary>
    /// Convert to compact single-line string (for AI agents)
    /// </summary>
    public string ToCompactString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"@ok:{Summary.Ok} @t:{Target} @p:{Summary.Passed} @f:{Summary.Failed} @d:{Summary.Duration}ms");
        sb.AppendLine($"git:{Git.Branch}:{(Git.Clean ? "clean" : "dirty")}:+{Git.Ahead}:-{Git.Behind}");

        foreach (var ep in Endpoints)
        {
            sb.AppendLine($"{ep.Url}:{ep.Status}:{ep.Time}ms:{ep.Version ?? "-"}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Convert to status line
    /// </summary>
    public string ToStatusLine()
    {
        var status = Summary.Ok ? "✓" : "✗";
        var healthy = Endpoints.Count(e => e.Status >= 200 && e.Status < 300);
        var total = Endpoints.Count;
        var ago = FormatTimeAgo(DateTime.UtcNow - ValidatedAt);

        return $"{status} {Target}: {healthy}/{total} healthy (last checked: {ago})";
    }

    /// <summary>
    /// Serialize to YAML-like format
    /// </summary>
    public string Serialize()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"@v: {Version}");
        sb.AppendLine($"@t: {Target}");
        sb.AppendLine($"@at: {ValidatedAt:yyyy-MM-ddTHH:mm:ssZ}");
        sb.AppendLine();

        sb.AppendLine("git:");
        sb.AppendLine($"  b: {Git.Branch}");
        sb.AppendLine($"  r: [{string.Join(", ", Git.Remotes)}]");
        sb.AppendLine($"  c: {Git.Clean.ToString().ToLower()}");
        sb.AppendLine($"  h: {Git.Head}");
        sb.AppendLine($"  +: {Git.Ahead}");
        sb.AppendLine($"  -: {Git.Behind}");
        sb.AppendLine();

        sb.AppendLine("eps:");
        foreach (var ep in Endpoints)
        {
            sb.AppendLine($"  - n: {ep.Name}");
            sb.AppendLine($"    u: {ep.Url}");
            if (!string.IsNullOrEmpty(ep.HealthPath))
                sb.AppendLine($"    h: {ep.HealthPath}");
            sb.AppendLine($"    s: {ep.Status}");
            sb.AppendLine($"    t: {ep.Time}");
            if (!string.IsNullOrEmpty(ep.Version))
                sb.AppendLine($"    v: {ep.Version}");
            if (ep.SslDays.HasValue)
                sb.AppendLine($"    ssl: {ep.SslDays}");
            sb.AppendLine();
        }

        sb.AppendLine("sum:");
        sb.AppendLine($"  ok: {Summary.Ok.ToString().ToLower()}");
        sb.AppendLine($"  p: {Summary.Passed}");
        sb.AppendLine($"  w: {Summary.Warnings}");
        sb.AppendLine($"  f: {Summary.Failed}");
        sb.AppendLine($"  d: {Summary.Duration}");
        if (!string.IsNullOrEmpty(Summary.ValidationId))
            sb.AppendLine($"  vid: {Summary.ValidationId}");
        sb.AppendLine();

        sb.AppendLine("thr:");
        sb.AppendLine($"  rt: [{Thresholds.ResponseTimeWarn}, {Thresholds.ResponseTimeCrit}]");
        sb.AppendLine($"  ssl: [{Thresholds.SslWarn}, {Thresholds.SslCrit}]");

        return sb.ToString();
    }

    /// <summary>
    /// Parse from YAML-like format
    /// </summary>
    public static UnifiedInfraFormat Parse(string content)
    {
        var result = new UnifiedInfraFormat();
        var lines = content.Split('\n');
        var currentSection = "";
        UnifiedEndpointState? currentEndpoint = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                continue;

            // Top-level fields
            if (line.StartsWith("@v:"))
                result.Version = int.Parse(line.Split(':')[1].Trim());
            else if (line.StartsWith("@t:"))
                result.Target = line.Split(':')[1].Trim();
            else if (line.StartsWith("@at:"))
                result.ValidatedAt = DateTime.Parse(line.Substring(4).Trim());

            // Section headers
            else if (line == "git:")
                currentSection = "git";
            else if (line == "eps:")
                currentSection = "eps";
            else if (line == "sum:")
                currentSection = "sum";
            else if (line == "thr:")
                currentSection = "thr";

            // Git section
            else if (currentSection == "git")
            {
                if (line.StartsWith("b:"))
                    result.Git.Branch = line.Substring(2).Trim();
                else if (line.StartsWith("r:"))
                    result.Git.Remotes = ParseList(line.Substring(2).Trim());
                else if (line.StartsWith("c:"))
                    result.Git.Clean = line.Substring(2).Trim() == "true";
                else if (line.StartsWith("h:"))
                    result.Git.Head = line.Substring(2).Trim();
                else if (line.StartsWith("+:"))
                    result.Git.Ahead = int.Parse(line.Substring(2).Trim());
                else if (line.StartsWith("-:"))
                    result.Git.Behind = int.Parse(line.Substring(2).Trim());
            }

            // Endpoints section
            else if (currentSection == "eps")
            {
                if (line.StartsWith("- n:"))
                {
                    if (currentEndpoint != null)
                        result.Endpoints.Add(currentEndpoint);
                    currentEndpoint = new UnifiedEndpointState { Name = line.Substring(4).Trim() };
                }
                else if (currentEndpoint != null)
                {
                    if (line.StartsWith("u:"))
                        currentEndpoint.Url = line.Substring(2).Trim();
                    else if (line.StartsWith("h:"))
                        currentEndpoint.HealthPath = line.Substring(2).Trim();
                    else if (line.StartsWith("s:"))
                        currentEndpoint.Status = int.Parse(line.Substring(2).Trim());
                    else if (line.StartsWith("t:"))
                        currentEndpoint.Time = int.Parse(line.Substring(2).Trim());
                    else if (line.StartsWith("v:"))
                        currentEndpoint.Version = line.Substring(2).Trim();
                    else if (line.StartsWith("ssl:"))
                        currentEndpoint.SslDays = int.Parse(line.Substring(4).Trim());
                }
            }

            // Summary section
            else if (currentSection == "sum")
            {
                if (line.StartsWith("ok:"))
                    result.Summary.Ok = line.Substring(3).Trim() == "true";
                else if (line.StartsWith("p:"))
                    result.Summary.Passed = int.Parse(line.Substring(2).Trim());
                else if (line.StartsWith("w:"))
                    result.Summary.Warnings = int.Parse(line.Substring(2).Trim());
                else if (line.StartsWith("f:"))
                    result.Summary.Failed = int.Parse(line.Substring(2).Trim());
                else if (line.StartsWith("d:"))
                    result.Summary.Duration = int.Parse(line.Substring(2).Trim());
                else if (line.StartsWith("vid:"))
                    result.Summary.ValidationId = line.Substring(4).Trim();
            }

            // Thresholds section
            else if (currentSection == "thr")
            {
                if (line.StartsWith("rt:"))
                {
                    var values = ParseIntList(line.Substring(3).Trim());
                    if (values.Count >= 2)
                    {
                        result.Thresholds.ResponseTimeWarn = values[0];
                        result.Thresholds.ResponseTimeCrit = values[1];
                    }
                }
                else if (line.StartsWith("ssl:"))
                {
                    var values = ParseIntList(line.Substring(4).Trim());
                    if (values.Count >= 2)
                    {
                        result.Thresholds.SslWarn = values[0];
                        result.Thresholds.SslCrit = values[1];
                    }
                }
            }
        }

        // Add last endpoint
        if (currentEndpoint != null)
            result.Endpoints.Add(currentEndpoint);

        return result;
    }

    private static List<string> ParseList(string value)
    {
        value = value.Trim('[', ']');
        return value.Split(',').Select(s => s.Trim()).ToList();
    }

    private static List<int> ParseIntList(string value)
    {
        value = value.Trim('[', ']');
        return value.Split(',').Select(s => int.Parse(s.Trim())).ToList();
    }

    private static string FormatTimeAgo(TimeSpan ago)
    {
        if (ago.TotalSeconds < 60) return "just now";
        if (ago.TotalMinutes < 60) return $"{(int)ago.TotalMinutes}m ago";
        if (ago.TotalHours < 24) return $"{(int)ago.TotalHours}h ago";
        return $"{(int)ago.TotalDays}d ago";
    }
}

#region State Models

public class UnifiedGitState
{
    public string Branch { get; set; } = "";
    public List<string> Remotes { get; set; } = new();
    public bool Clean { get; set; }
    public string Head { get; set; } = "";
    public int Ahead { get; set; }
    public int Behind { get; set; }
}

public class UnifiedEndpointState
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public string? HealthPath { get; set; }
    public int Status { get; set; }
    public int Time { get; set; }
    public string? Version { get; set; }
    public int? SslDays { get; set; }
}

public class UnifiedSummaryState
{
    public bool Ok { get; set; }
    public int Passed { get; set; }
    public int Warnings { get; set; }
    public int Failed { get; set; }
    public int Duration { get; set; }
    public string? ValidationId { get; set; }
}

public class UnifiedThresholdState
{
    public int ResponseTimeWarn { get; set; } = 500;
    public int ResponseTimeCrit { get; set; } = 2000;
    public int SslWarn { get; set; } = 30;
    public int SslCrit { get; set; } = 7;
}

#endregion
