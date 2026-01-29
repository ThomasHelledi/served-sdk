namespace Served.SDK.Models.DevOps;

#region Repository Models

/// <summary>
/// Represents a connected DevOps repository (GitHub, GitLab, Azure DevOps).
/// </summary>
public class DevOpsRepositoryViewModel
{
    public int Id { get; set; }
    public string Provider { get; set; } = "";
    public string RepositoryName { get; set; } = "";
    public string RepositoryUrl { get; set; } = "";
    public string? CloneUrl { get; set; }
    public string DefaultBranch { get; set; } = "main";
    public string? Description { get; set; }
    public bool IsPrivate { get; set; }
    public bool WebhookActive { get; set; }
    public DateTime? LastWebhookAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Request to connect a new repository.
/// </summary>
public class CreateRepositoryRequest
{
    /// <summary>
    /// Provider: GitHub, GitLab, or AzureDevOps.
    /// </summary>
    public string Provider { get; set; } = "GitHub";

    /// <summary>
    /// Repository name (e.g., 'owner/repo' for GitHub, 'namespace/project' for GitLab).
    /// </summary>
    public string RepositoryName { get; set; } = "";

    /// <summary>
    /// Full repository URL.
    /// </summary>
    public string RepositoryUrl { get; set; } = "";

    /// <summary>
    /// Clone URL (optional).
    /// </summary>
    public string? CloneUrl { get; set; }

    /// <summary>
    /// Default branch. Default: main.
    /// </summary>
    public string DefaultBranch { get; set; } = "main";

    /// <summary>
    /// Repository description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Is repository private? Default: false.
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Access token (PAT) for API access.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Azure DevOps organization (required for AzureDevOps provider).
    /// </summary>
    public string? AzureOrganization { get; set; }

    /// <summary>
    /// Azure DevOps project (required for AzureDevOps provider).
    /// </summary>
    public string? AzureProject { get; set; }

    /// <summary>
    /// Setup webhook automatically? Default: true.
    /// </summary>
    public bool SetupWebhook { get; set; } = true;
}

/// <summary>
/// Request to update a repository.
/// </summary>
public class UpdateRepositoryRequest
{
    public int Id { get; set; }
    public string? DefaultBranch { get; set; }
    public string? Description { get; set; }
    public string? AccessToken { get; set; }
    public bool? IsActive { get; set; }
}

#endregion

#region Pull Request Models

/// <summary>
/// Represents a pull request from a connected repository.
/// </summary>
public class PullRequestViewModel
{
    public int Id { get; set; }
    public int RepositoryId { get; set; }
    public string RepositoryName { get; set; } = "";
    public string Provider { get; set; } = "";
    public int ExternalPrNumber { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Url { get; set; } = "";
    public string SourceBranch { get; set; } = "";
    public string TargetBranch { get; set; } = "";

    /// <summary>
    /// PR state: Open, Merged, Closed.
    /// </summary>
    public string State { get; set; } = "Open";
    public bool IsDraft { get; set; }
    public string? AuthorUsername { get; set; }
    public string? AuthorAvatarUrl { get; set; }
    public int CommitCount { get; set; }
    public int FilesChanged { get; set; }
    public int Additions { get; set; }
    public int Deletions { get; set; }

    /// <summary>
    /// CI status: pending, success, failure, error.
    /// </summary>
    public string? CiStatus { get; set; }
    public string? CiRunUrl { get; set; }
    public DateTime ExternalCreatedAt { get; set; }
    public DateTime? MergedAt { get; set; }

    /// <summary>
    /// Linked Served task ID (if any).
    /// </summary>
    public int? TaskId { get; set; }
    public string? TaskName { get; set; }

    /// <summary>
    /// Agent session ID that created this PR (if any).
    /// </summary>
    public int? AgentSessionId { get; set; }
}

/// <summary>
/// Request to link a pull request to a task.
/// </summary>
public class LinkPullRequestRequest
{
    public int PullRequestId { get; set; }
    public int TaskId { get; set; }
}

#endregion

#region Pipeline Run Models

/// <summary>
/// Represents a CI/CD pipeline run.
/// </summary>
public class PipelineRunViewModel
{
    public int Id { get; set; }
    public int RepositoryId { get; set; }
    public int? PullRequestId { get; set; }
    public string ExternalRunId { get; set; } = "";
    public long? RunNumber { get; set; }
    public string? PipelineName { get; set; }
    public string Url { get; set; } = "";

    /// <summary>
    /// Status: Queued, InProgress, Completed.
    /// </summary>
    public string Status { get; set; } = "Queued";

    /// <summary>
    /// Conclusion: success, failure, cancelled, skipped.
    /// </summary>
    public string? Conclusion { get; set; }
    public string? TriggerEvent { get; set; }
    public string? HeadBranch { get; set; }
    public string? HeadSha { get; set; }
    public string? CommitMessage { get; set; }
    public string? Actor { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationSeconds { get; set; }
    public string? ErrorMessage { get; set; }
}

#endregion

#region Unified Release Models

/// <summary>
/// Release stage in the unified release workflow.
/// Matches stages from releases.unified config.
/// </summary>
public enum ReleaseStage
{
    /// <summary>Release created, version bumped.</summary>
    Created,

    /// <summary>Building Docker images and packages.</summary>
    Building,

    /// <summary>Running tests and validation.</summary>
    Testing,

    /// <summary>Deploying to Kubernetes.</summary>
    Deploying,

    /// <summary>Verifying health checks.</summary>
    Verifying,

    /// <summary>Published and live.</summary>
    Published,

    /// <summary>Release failed at some stage.</summary>
    Failed,

    /// <summary>Rolled back to previous version.</summary>
    RolledBack
}

/// <summary>
/// Unified release view model with CalVer versioning and stage tracking.
/// </summary>
public class UnifiedReleaseViewModel
{
    public int Id { get; set; }

    /// <summary>
    /// Tenant that owns this release.
    /// </summary>
    public int TenantId { get; set; }

    /// <summary>
    /// CalVer version (e.g., 2026.01.42).
    /// </summary>
    public string Version { get; set; } = "";

    /// <summary>
    /// Previous version for rollback.
    /// </summary>
    public string? PreviousVersion { get; set; }

    /// <summary>
    /// Current stage in the release workflow.
    /// </summary>
    public ReleaseStage Stage { get; set; }

    /// <summary>
    /// Display name for the stage.
    /// </summary>
    public string StageName => Stage.ToString().ToLowerInvariant();

    /// <summary>
    /// Git tag name (e.g., v2026.01.42).
    /// </summary>
    public string? TagName { get; set; }

    /// <summary>
    /// Git commit SHA.
    /// </summary>
    public string? CommitSha { get; set; }

    /// <summary>
    /// Git branch the release was created from.
    /// </summary>
    public string? Branch { get; set; }

    /// <summary>
    /// Release notes (auto-generated from commits or manual).
    /// </summary>
    public string? ReleaseNotes { get; set; }

    /// <summary>
    /// User who initiated the release.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Targets included in this release (api, unifiedhq, sdk, etc.).
    /// </summary>
    public List<string> Targets { get; set; } = new();

    /// <summary>
    /// Individual deployments within this release.
    /// </summary>
    public List<ReleaseDeploymentViewModel> Deployments { get; set; } = new();

    /// <summary>
    /// Published packages (NuGet, npm) in this release.
    /// </summary>
    public List<ReleasePackageViewModel> Packages { get; set; } = new();

    /// <summary>
    /// Error message if stage is Failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Stage at which failure occurred.
    /// </summary>
    public ReleaseStage? FailedAtStage { get; set; }

    /// <summary>
    /// Health check results.
    /// </summary>
    public List<ReleaseHealthCheckViewModel> HealthChecks { get; set; } = new();

    /// <summary>
    /// Is auto-publish enabled for this release?
    /// </summary>
    public bool AutoPublish { get; set; }

    /// <summary>
    /// Timestamp when each stage was entered.
    /// </summary>
    public Dictionary<string, DateTime> StageTimestamps { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Total duration in seconds.
    /// </summary>
    public long? DurationSeconds { get; set; }

    /// <summary>
    /// Is the release currently active/healthy?
    /// </summary>
    public bool IsHealthy => Stage == ReleaseStage.Published &&
                              HealthChecks.All(h => h.IsHealthy);

    /// <summary>
    /// Can this release be rolled back?
    /// </summary>
    public bool CanRollback => Stage == ReleaseStage.Published &&
                                !string.IsNullOrEmpty(PreviousVersion);
}

/// <summary>
/// Individual deployment within a release.
/// </summary>
public class ReleaseDeploymentViewModel
{
    public int Id { get; set; }
    public int ReleaseId { get; set; }

    /// <summary>
    /// Target being deployed (api, unifiedhq, etc.).
    /// </summary>
    public string Target { get; set; } = "";

    /// <summary>
    /// Docker image with tag.
    /// </summary>
    public string? ImageTag { get; set; }

    /// <summary>
    /// Kubernetes resource (e.g., statefulset/served-api).
    /// </summary>
    public string? K8sResource { get; set; }

    /// <summary>
    /// Kubernetes namespace.
    /// </summary>
    public string? K8sNamespace { get; set; }

    /// <summary>
    /// Deployment status.
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// Health check URL.
    /// </summary>
    public string? HealthCheckUrl { get; set; }

    /// <summary>
    /// Is deployment healthy?
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationSeconds { get; set; }
}

/// <summary>
/// Published package in a release.
/// </summary>
public class ReleasePackageViewModel
{
    public int Id { get; set; }
    public int ReleaseId { get; set; }

    /// <summary>
    /// Package name (e.g., Served.SDK).
    /// </summary>
    public string PackageName { get; set; } = "";

    /// <summary>
    /// Package type: nuget, npm, helm.
    /// </summary>
    public string PackageType { get; set; } = "";

    /// <summary>
    /// Package version.
    /// </summary>
    public string Version { get; set; } = "";

    /// <summary>
    /// Registry URL where published.
    /// </summary>
    public string? RegistryUrl { get; set; }

    /// <summary>
    /// Package download URL.
    /// </summary>
    public string? DownloadUrl { get; set; }

    /// <summary>
    /// Is package successfully published?
    /// </summary>
    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }
}

/// <summary>
/// Health check result for a deployment.
/// </summary>
public class ReleaseHealthCheckViewModel
{
    public string Target { get; set; } = "";
    public string Url { get; set; } = "";
    public bool IsHealthy { get; set; }
    public int? HttpStatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}

/// <summary>
/// Request to create a new unified release.
/// </summary>
public class CreateUnifiedReleaseRequest
{
    /// <summary>
    /// Targets to include in release. If empty, includes all.
    /// </summary>
    public List<string>? Targets { get; set; }

    /// <summary>
    /// Branch to release from. Default: current branch.
    /// </summary>
    public string? Branch { get; set; }

    /// <summary>
    /// Custom release notes. If null, auto-generated from commits.
    /// </summary>
    public string? ReleaseNotes { get; set; }

    /// <summary>
    /// Skip tests? Default: false.
    /// </summary>
    public bool SkipTests { get; set; }

    /// <summary>
    /// Auto-publish after verification? Default: false.
    /// </summary>
    public bool AutoPublish { get; set; }

    /// <summary>
    /// Dry run mode - don't actually deploy.
    /// </summary>
    public bool DryRun { get; set; }
}

/// <summary>
/// Request to progress a release to the next stage.
/// </summary>
public class UpdateReleaseStageRequest
{
    /// <summary>
    /// New stage for the release.
    /// </summary>
    public ReleaseStage Stage { get; set; }

    /// <summary>
    /// Error message if setting to Failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Health check results when setting to Verifying or Published.
    /// </summary>
    public List<ReleaseHealthCheckViewModel>? HealthChecks { get; set; }
}

/// <summary>
/// Request to rollback a release.
/// </summary>
public class RollbackReleaseRequest
{
    /// <summary>
    /// Version to rollback to. If null, rollback to previous version.
    /// </summary>
    public string? TargetVersion { get; set; }

    /// <summary>
    /// Reason for rollback.
    /// </summary>
    public string? Reason { get; set; }
}

/// <summary>
/// Summary of releases for dashboard display.
/// </summary>
public class ReleaseSummaryViewModel
{
    public int TotalReleases { get; set; }
    public int PublishedCount { get; set; }
    public int FailedCount { get; set; }
    public int InProgressCount { get; set; }
    public UnifiedReleaseViewModel? CurrentRelease { get; set; }
    public UnifiedReleaseViewModel? LastSuccessfulRelease { get; set; }
    public List<UnifiedReleaseViewModel> RecentReleases { get; set; } = new();
}

#endregion
