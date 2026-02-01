# DevOps API

Repository, Pull Request, and Pipeline operations.

Access via: `client.DevOpsModule`

---

## Repositories

### GetRepositoriesAsync

Get all connected repositories.

```csharp
var repos = await client.DevOpsModule.GetRepositoriesAsync();

// Active only (default)
var activeRepos = await client.DevOpsModule.GetRepositoriesAsync(activeOnly: true);

// All repos including inactive
var allRepos = await client.DevOpsModule.GetRepositoriesAsync(activeOnly: false);

foreach (var repo in repos)
{
    Console.WriteLine($"{repo.Name} ({repo.Provider}) - {repo.DefaultBranch}");
}
```

**Returns:** `List<DevOpsRepositoryViewModel>`

---

### GetRepositoryAsync

Get a repository by ID.

```csharp
var repo = await client.DevOpsModule.GetRepositoryAsync(123);

Console.WriteLine($"Repository: {repo.Name}");
Console.WriteLine($"Provider: {repo.Provider}");
Console.WriteLine($"URL: {repo.Url}");
Console.WriteLine($"Default Branch: {repo.DefaultBranch}");
```

---

### CreateRepositoryAsync

Connect a new repository.

```csharp
var repo = await client.DevOpsModule.CreateRepositoryAsync(
    new CreateRepositoryRequest
    {
        Name = "my-project",
        Provider = "github",
        Url = "https://github.com/company/my-project",
        DefaultBranch = "main",
        IsActive = true
    }
);
```

**Supported Providers:**
- `github` - GitHub
- `gitlab` - GitLab
- `forge` - Forge Go (self-hosted)
- `azure` - Azure DevOps
- `bitbucket` - Bitbucket

---

### UpdateRepositoryAsync

Update repository settings.

```csharp
await client.DevOpsModule.UpdateRepositoryAsync(
    new UpdateRepositoryRequest
    {
        Id = 123,
        DefaultBranch = "develop",
        IsActive = true
    }
);
```

---

### DeleteRepositoryAsync

Disconnect a repository.

```csharp
await client.DevOpsModule.DeleteRepositoryAsync(123);
```

---

## Pull Requests

### GetPullRequestsAsync

Get all pull requests for the tenant.

```csharp
// All PRs
var prs = await client.DevOpsModule.GetPullRequestsAsync();

// Open PRs only
var openPRs = await client.DevOpsModule.GetPullRequestsAsync(state: "Open");

// Merged PRs
var mergedPRs = await client.DevOpsModule.GetPullRequestsAsync(
    state: "Merged",
    limit: 100
);
```

**State Values:**
- `Open` - Not yet merged
- `Merged` - Successfully merged
- `Closed` - Closed without merge

---

### GetPullRequestsByRepositoryAsync

Get PRs for a specific repository.

```csharp
var prs = await client.DevOpsModule.GetPullRequestsByRepositoryAsync(
    repositoryId: 123,
    state: "Open",
    limit: 50
);
```

---

### GetPullRequestsByTaskAsync

Get PRs linked to a task.

```csharp
var prs = await client.DevOpsModule.GetPullRequestsByTaskAsync(taskId: 456);

foreach (var pr in prs)
{
    Console.WriteLine($"PR #{pr.Number}: {pr.Title} ({pr.State})");
}
```

---

### GetPullRequestsBySessionAsync

Get PRs created by an AI agent session.

```csharp
var prs = await client.DevOpsModule.GetPullRequestsBySessionAsync(sessionId: 789);
```

---

## Pipeline Runs

### GetPipelineRunsAsync

Get pipeline runs for a pull request.

```csharp
var runs = await client.DevOpsModule.GetPipelineRunsAsync(pullRequestId: 123);

foreach (var run in runs)
{
    Console.WriteLine($"Pipeline: {run.Name}");
    Console.WriteLine($"Status: {run.Status}");
    Console.WriteLine($"Duration: {run.Duration}");
}
```

---

### GetLatestPipelineRunAsync

Get the most recent pipeline run for a PR.

```csharp
var latestRun = await client.DevOpsModule.GetLatestPipelineRunAsync(pullRequestId: 123);

if (latestRun != null)
{
    Console.WriteLine($"Status: {latestRun.Status}");
    Console.WriteLine($"Started: {latestRun.StartedAt}");
    Console.WriteLine($"Finished: {latestRun.FinishedAt}");
}
```

---

### GetPipelineRunsByRepositoryAsync

Get pipeline history for a repository.

```csharp
var runs = await client.DevOpsModule.GetPipelineRunsByRepositoryAsync(
    repositoryId: 123,
    limit: 50
);

var successRate = runs.Count(r => r.Status == "Success") / (double)runs.Count;
Console.WriteLine($"Success rate: {successRate:P0}");
```

---

## Models

### DevOpsRepositoryViewModel

```csharp
public class DevOpsRepositoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Provider { get; set; }
    public string Url { get; set; }
    public string DefaultBranch { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
}
```

### PullRequestViewModel

```csharp
public class PullRequestViewModel
{
    public int Id { get; set; }
    public int RepositoryId { get; set; }
    public string RepositoryName { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string State { get; set; }
    public string SourceBranch { get; set; }
    public string TargetBranch { get; set; }
    public string AuthorName { get; set; }
    public string Url { get; set; }
    public int? TaskId { get; set; }
    public int? AgentSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? MergedAt { get; set; }
}
```

### PipelineRunViewModel

```csharp
public class PipelineRunViewModel
{
    public int Id { get; set; }
    public int PullRequestId { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public string Url { get; set; }
    public string LogUrl { get; set; }
}
```

**Pipeline Status Values:**
- `Pending` - Queued
- `Running` - In progress
- `Success` - Completed successfully
- `Failed` - Failed
- `Cancelled` - Cancelled by user

---

## Common Patterns

### Repository Health Dashboard

```csharp
public async Task<List<RepoHealth>> GetRepositoryHealthAsync()
{
    var repos = await _client.DevOpsModule.GetRepositoriesAsync();
    var results = new List<RepoHealth>();

    foreach (var repo in repos)
    {
        var runs = await _client.DevOpsModule.GetPipelineRunsByRepositoryAsync(
            repo.Id, limit: 20
        );

        var successCount = runs.Count(r => r.Status == "Success");

        results.Add(new RepoHealth
        {
            RepositoryName = repo.Name,
            PipelineSuccessRate = runs.Count > 0
                ? (double)successCount / runs.Count
                : 1.0,
            LastPipelineRun = runs.FirstOrDefault()?.FinishedAt
        });
    }

    return results;
}
```

### Task PR Tracker

```csharp
public async Task<TaskDevOpsStatus> GetTaskDevOpsStatusAsync(int taskId)
{
    var prs = await _client.DevOpsModule.GetPullRequestsByTaskAsync(taskId);

    var status = new TaskDevOpsStatus
    {
        TaskId = taskId,
        HasOpenPR = prs.Any(p => p.State == "Open"),
        HasMergedPR = prs.Any(p => p.State == "Merged"),
        PullRequests = prs
    };

    // Get latest pipeline status for open PRs
    foreach (var pr in prs.Where(p => p.State == "Open"))
    {
        var latestRun = await _client.DevOpsModule.GetLatestPipelineRunAsync(pr.Id);
        if (latestRun != null)
        {
            status.LatestPipelineStatus = latestRun.Status;
        }
    }

    return status;
}
```

### AI Agent PR Summary

```csharp
public async Task<AgentPRSummary> GetAgentSessionPRsAsync(int sessionId)
{
    var prs = await _client.DevOpsModule.GetPullRequestsBySessionAsync(sessionId);

    return new AgentPRSummary
    {
        SessionId = sessionId,
        TotalPRs = prs.Count,
        OpenPRs = prs.Count(p => p.State == "Open"),
        MergedPRs = prs.Count(p => p.State == "Merged"),
        PullRequests = prs.OrderByDescending(p => p.CreatedAt).ToList()
    };
}
```
