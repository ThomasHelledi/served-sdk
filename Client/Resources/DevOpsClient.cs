using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Common;
using Served.SDK.Models.DevOps;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for DevOps repository, pull request, and pipeline operations.
/// </summary>
public class DevOpsClient : IDevOpsClient
{
    private readonly IServedClient _client;

    public DevOpsClient(IServedClient client)
    {
        _client = client;
    }

    #region Repositories

    /// <inheritdoc/>
    public async Task<List<DevOpsRepositoryViewModel>> GetRepositoriesAsync(bool activeOnly = true)
    {
        var response = await _client.GetAsync<ApiListResponse<DevOpsRepositoryViewModel>>(
            $"api/devops/repositories?activeOnly={activeOnly}");
        return response.Data ?? new List<DevOpsRepositoryViewModel>();
    }

    /// <inheritdoc/>
    public Task<DevOpsRepositoryViewModel> GetRepositoryAsync(int id)
    {
        return _client.GetAsync<DevOpsRepositoryViewModel>($"api/devops/repositories/{id}");
    }

    /// <inheritdoc/>
    public Task<DevOpsRepositoryViewModel> CreateRepositoryAsync(CreateRepositoryRequest request)
    {
        return _client.PostAsync<DevOpsRepositoryViewModel>("api/devops/repositories", request);
    }

    /// <inheritdoc/>
    public Task<DevOpsRepositoryViewModel> UpdateRepositoryAsync(UpdateRepositoryRequest request)
    {
        return _client.PutAsync<DevOpsRepositoryViewModel>("api/devops/repositories", request);
    }

    /// <inheritdoc/>
    public Task DeleteRepositoryAsync(int id)
    {
        return _client.DeleteAsync($"api/devops/repositories/{id}");
    }

    #endregion

    #region Pull Requests

    /// <inheritdoc/>
    public async Task<List<PullRequestViewModel>> GetPullRequestsAsync(string? state = null, int limit = 50)
    {
        var query = $"api/devops/pullrequests?limit={limit}";
        if (!string.IsNullOrEmpty(state))
            query += $"&state={state}";

        var response = await _client.GetAsync<ApiListResponse<PullRequestViewModel>>(query);
        return response.Data ?? new List<PullRequestViewModel>();
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestViewModel>> GetPullRequestsByRepositoryAsync(int repositoryId, string? state = null, int limit = 50)
    {
        var query = $"api/devops/repositories/{repositoryId}/pullrequests?limit={limit}";
        if (!string.IsNullOrEmpty(state))
            query += $"&state={state}";

        var response = await _client.GetAsync<ApiListResponse<PullRequestViewModel>>(query);
        return response.Data ?? new List<PullRequestViewModel>();
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestViewModel>> GetPullRequestsByTaskAsync(int taskId)
    {
        var response = await _client.GetAsync<ApiListResponse<PullRequestViewModel>>(
            $"api/devops/tasks/{taskId}/pullrequests");
        return response.Data ?? new List<PullRequestViewModel>();
    }

    /// <inheritdoc/>
    public async Task<List<PullRequestViewModel>> GetPullRequestsBySessionAsync(int sessionId)
    {
        var response = await _client.GetAsync<ApiListResponse<PullRequestViewModel>>(
            $"api/devops/sessions/{sessionId}/pullrequests");
        return response.Data ?? new List<PullRequestViewModel>();
    }

    #endregion

    #region Pipeline Runs

    /// <inheritdoc/>
    public async Task<List<PipelineRunViewModel>> GetPipelineRunsAsync(int pullRequestId)
    {
        var response = await _client.GetAsync<ApiListResponse<PipelineRunViewModel>>(
            $"api/devops/pullrequests/{pullRequestId}/runs");
        return response.Data ?? new List<PipelineRunViewModel>();
    }

    /// <inheritdoc/>
    public async Task<PipelineRunViewModel?> GetLatestPipelineRunAsync(int pullRequestId)
    {
        try
        {
            return await _client.GetAsync<PipelineRunViewModel>(
                $"api/devops/pullrequests/{pullRequestId}/runs/latest");
        }
        catch (Exceptions.ServedApiException ex) when (ex.StatusCode == 404)
        {
            // No pipeline runs exist yet - return null
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<List<PipelineRunViewModel>> GetPipelineRunsByRepositoryAsync(int repositoryId, int limit = 50)
    {
        var response = await _client.GetAsync<ApiListResponse<PipelineRunViewModel>>(
            $"api/devops/repositories/{repositoryId}/runs?limit={limit}");
        return response.Data ?? new List<PipelineRunViewModel>();
    }

    #endregion
}
