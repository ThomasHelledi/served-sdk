using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.DevOps;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for DevOps repository, pull request, and pipeline operations.
/// </summary>
public interface IDevOpsClient
{
    #region Repositories

    /// <summary>
    /// Gets all connected repositories.
    /// </summary>
    /// <param name="activeOnly">Only return active repositories. Default: true.</param>
    Task<List<DevOpsRepositoryViewModel>> GetRepositoriesAsync(bool activeOnly = true);

    /// <summary>
    /// Gets a repository by ID.
    /// </summary>
    Task<DevOpsRepositoryViewModel> GetRepositoryAsync(int id);

    /// <summary>
    /// Connects a new repository.
    /// </summary>
    Task<DevOpsRepositoryViewModel> CreateRepositoryAsync(CreateRepositoryRequest request);

    /// <summary>
    /// Updates repository settings.
    /// </summary>
    Task<DevOpsRepositoryViewModel> UpdateRepositoryAsync(UpdateRepositoryRequest request);

    /// <summary>
    /// Disconnects/deletes a repository.
    /// </summary>
    Task DeleteRepositoryAsync(int id);

    #endregion

    #region Pull Requests

    /// <summary>
    /// Gets all pull requests for the tenant.
    /// </summary>
    /// <param name="state">Filter by state: Open, Merged, Closed.</param>
    /// <param name="limit">Maximum number of results. Default: 50.</param>
    Task<List<PullRequestViewModel>> GetPullRequestsAsync(string? state = null, int limit = 50);

    /// <summary>
    /// Gets pull requests for a specific repository.
    /// </summary>
    /// <param name="repositoryId">Repository ID.</param>
    /// <param name="state">Filter by state: Open, Merged, Closed.</param>
    /// <param name="limit">Maximum number of results. Default: 50.</param>
    Task<List<PullRequestViewModel>> GetPullRequestsByRepositoryAsync(int repositoryId, string? state = null, int limit = 50);

    /// <summary>
    /// Gets pull requests linked to a task.
    /// </summary>
    Task<List<PullRequestViewModel>> GetPullRequestsByTaskAsync(int taskId);

    /// <summary>
    /// Gets pull requests created by an agent session.
    /// </summary>
    Task<List<PullRequestViewModel>> GetPullRequestsBySessionAsync(int sessionId);

    #endregion

    #region Pipeline Runs

    /// <summary>
    /// Gets pipeline runs for a pull request.
    /// </summary>
    Task<List<PipelineRunViewModel>> GetPipelineRunsAsync(int pullRequestId);

    /// <summary>
    /// Gets the latest pipeline run for a pull request.
    /// </summary>
    Task<PipelineRunViewModel?> GetLatestPipelineRunAsync(int pullRequestId);

    /// <summary>
    /// Gets pipeline runs for a repository.
    /// </summary>
    /// <param name="repositoryId">Repository ID.</param>
    /// <param name="limit">Maximum number of results. Default: 50.</param>
    Task<List<PipelineRunViewModel>> GetPipelineRunsByRepositoryAsync(int repositoryId, int limit = 50);

    #endregion
}
