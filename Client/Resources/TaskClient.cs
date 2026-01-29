using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Tasks;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for task management operations using API V2 endpoints.
/// </summary>
public class TaskClient : ITaskClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public TaskClient(IServedClient client)
    {
        _client = client;
    }

    #region CRUD Operations

    /// <inheritdoc/>
    public async Task<List<TaskSummary>> GetAllAsync(TaskQueryParams? query = null)
    {
        var q = query ?? new TaskQueryParams();

        // Build query string for API V2 endpoint
        var queryParams = new List<string>();

        if (q.ProjectId.HasValue)
            queryParams.Add($"projectId={q.ProjectId.Value}");
        if (!string.IsNullOrEmpty(q.Search))
            queryParams.Add($"search={HttpUtility.UrlEncode(q.Search)}");

        var page = q.Skip.HasValue && q.Take.HasValue ? (q.Skip.Value / q.Take.Value) + 1 : 1;
        var pageSize = q.Take ?? 50;
        queryParams.Add($"page={page}");
        queryParams.Add($"pageSize={pageSize}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var response = await _client.GetAsync<ApiV2ListResponse<TaskDetail>>($"api/tasks{queryString}");

        return response.Data?.Select(d => new TaskSummary
        {
            Id = d.Id,
            Name = d.Name ?? string.Empty,
            TaskNo = d.TaskNo,
            ProjectId = d.ProjectId,
            ProjectName = d.ProjectName,
            ParentTaskId = d.ParentId
        }).ToList() ?? new List<TaskSummary>();
    }

    /// <inheritdoc/>
    public async Task<TaskDetail> GetAsync(int id)
    {
        return await _client.GetAsync<TaskDetail>($"api/tasks/{id}");
    }

    /// <inheritdoc/>
    public async Task<TaskDetail> CreateAsync(CreateTaskRequest request)
    {
        return await _client.PostAsync<TaskDetail>("api/tasks", request);
    }

    /// <inheritdoc/>
    public async Task<TaskDetail> UpdateAsync(int id, UpdateTaskRequest request)
    {
        return await _client.PutAsync<TaskDetail>($"api/tasks/{id}", request);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(int id)
    {
        return _client.DeleteAsync($"api/tasks/{id}");
    }

    /// <inheritdoc/>
    public async Task<TaskDetail> UpdateStatusAsync(int id, UpdateTaskStatusRequest request)
    {
        return await _client.PatchAsync<TaskDetail>($"api/tasks/{id}/status", request);
    }

    #endregion

    #region Bulk Operations

    /// <inheritdoc/>
    public Task<BulkResponse<TaskDetail>> CreateBulkAsync(BulkCreateTasksRequest request)
    {
        return _client.PostAsync<BulkResponse<TaskDetail>>("api/tasks/bulk", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<TaskDetail>> UpdateBulkAsync(BulkUpdateTasksRequest request)
    {
        return _client.PutAsync<BulkResponse<TaskDetail>>("api/tasks/bulk", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<TaskDetail>> DeleteBulkAsync(BulkDeleteTasksRequest request)
    {
        return _client.DeleteWithBodyAsync<BulkResponse<TaskDetail>>("api/tasks", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<TaskDetail>> UpdateStatusBulkAsync(BulkUpdateTaskStatusRequest request)
    {
        return _client.PatchAsync<BulkResponse<TaskDetail>>("api/tasks/bulk/status", request);
    }

    #endregion

    #region Query Operations

    /// <inheritdoc/>
    public async Task<List<int>> GetKeysAsync(TaskQueryParams query)
    {
        // For backwards compatibility, use the list endpoint and extract IDs
        var tasks = await GetAllAsync(query);
        return tasks.Select(t => t.Id).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<TaskDetail>> GetRangeAsync(List<int> ids)
    {
        // Fetch each task individually using the V2 endpoint
        var tasks = new List<TaskDetail>();
        foreach (var id in ids)
        {
            try
            {
                var task = await GetAsync(id);
                if (task != null)
                    tasks.Add(task);
            }
            catch
            {
                // Skip tasks that fail to load
            }
        }
        return tasks;
    }

    /// <inheritdoc/>
    public async Task<List<TaskSummary>> GetByProjectAsync(int projectId, bool includeCompleted = true)
    {
        var response = await _client.GetAsync<ApiV2ListResponse<TaskDetail>>($"api/tasks/by-project/{projectId}");

        return response.Data?.Select(d => new TaskSummary
        {
            Id = d.Id,
            Name = d.Name ?? string.Empty,
            TaskNo = d.TaskNo,
            ProjectId = d.ProjectId,
            ProjectName = d.ProjectName,
            ParentTaskId = d.ParentId
        }).ToList() ?? new List<TaskSummary>();
    }

    /// <inheritdoc/>
    public async Task<List<TaskSummary>> GetByAssigneeAsync(int employeeId, bool includeCompleted = false)
    {
        // Note: This endpoint was removed from the simplified API
        // Fall back to getting all tasks and filtering by project
        var query = new TaskQueryParams();
        return await GetAllAsync(query);
    }

    /// <inheritdoc/>
    public async Task<List<TaskSummary>> GetSubTasksAsync(int parentTaskId)
    {
        var query = new TaskQueryParams { ParentTaskId = parentTaskId };
        return await GetAllAsync(query);
    }

    /// <inheritdoc/>
    public async Task<List<TaskSummary>> SearchAsync(string searchTerm, int take = 20)
    {
        var query = new TaskQueryParams
        {
            Search = searchTerm,
            Take = take
        };
        return await GetAllAsync(query);
    }

    #endregion
}
