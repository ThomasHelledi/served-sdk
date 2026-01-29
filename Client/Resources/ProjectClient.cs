using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Projects;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for project management operations using API V2 endpoints.
/// </summary>
public class ProjectClient : IProjectClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public ProjectClient(IServedClient client)
    {
        _client = client;
    }

    #region CRUD Operations

    /// <inheritdoc/>
    public async Task<List<ProjectSummary>> GetAllAsync(ProjectQueryParams? query = null)
    {
        var q = query ?? new ProjectQueryParams();

        // Build query string for API V2 endpoint
        var queryParams = new List<string>();

        if (q.CustomerId.HasValue)
            queryParams.Add($"customerId={q.CustomerId.Value}");
        if (q.ProjectStatusId.HasValue)
            queryParams.Add($"projectStatusId={q.ProjectStatusId.Value}");
        if (q.IsActive.HasValue)
            queryParams.Add($"isActive={q.IsActive.Value}");
        if (!string.IsNullOrEmpty(q.Search))
            queryParams.Add($"search={HttpUtility.UrlEncode(q.Search)}");

        var page = q.Skip.HasValue && q.Take.HasValue ? (q.Skip.Value / q.Take.Value) + 1 : 1;
        var pageSize = q.Take ?? 50;
        queryParams.Add($"page={page}");
        queryParams.Add($"pageSize={pageSize}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var response = await _client.GetAsync<ApiV2ListResponse<ProjectDetail>>($"api/projects{queryString}");

        return response.Data?.Select(d => new ProjectSummary
        {
            Id = d.Id,
            Name = d.Name,
            ProjectNo = d.ProjectNo,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            ProjectStatusId = d.ProjectStatusId,
            CustomerId = d.CustomerId,
            ProjectManagerId = d.ProjectManagerId,
            IsActive = d.IsActive
        }).ToList() ?? new List<ProjectSummary>();
    }

    /// <inheritdoc/>
    public async Task<ProjectDetail> GetAsync(int id)
    {
        return await _client.GetAsync<ProjectDetail>($"api/projects/{id}");
    }

    /// <inheritdoc/>
    public async Task<ProjectDetail> CreateAsync(CreateProjectRequest request)
    {
        return await _client.PostAsync<ProjectDetail>("api/projects", request);
    }

    /// <inheritdoc/>
    public async Task<ProjectDetail> UpdateAsync(int id, UpdateProjectRequest request)
    {
        return await _client.PutAsync<ProjectDetail>($"api/projects/{id}", request);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(int id)
    {
        return _client.DeleteAsync($"api/projects/{id}");
    }

    #endregion

    #region Bulk Operations

    /// <inheritdoc/>
    public Task<BulkResponse<ProjectDetail>> CreateBulkAsync(BulkCreateProjectsRequest request)
    {
        return _client.PostAsync<BulkResponse<ProjectDetail>>("api/projects/bulk", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<ProjectDetail>> UpdateBulkAsync(BulkUpdateProjectsRequest request)
    {
        return _client.PutAsync<BulkResponse<ProjectDetail>>("api/projects/bulk", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<ProjectDetail>> DeleteBulkAsync(BulkDeleteProjectsRequest request)
    {
        return _client.DeleteWithBodyAsync<BulkResponse<ProjectDetail>>("api/projects/bulk", request);
    }

    #endregion

    #region Query Operations

    /// <inheritdoc/>
    public async Task<List<int>> GetKeysAsync(ProjectQueryParams query)
    {
        // For backwards compatibility, use the list endpoint and extract IDs
        var projects = await GetAllAsync(query);
        return projects.Select(p => p.Id).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<ProjectDetail>> GetRangeAsync(List<int> ids)
    {
        // Fetch each project individually using the V2 endpoint
        var projects = new List<ProjectDetail>();
        foreach (var id in ids)
        {
            try
            {
                var project = await GetAsync(id);
                if (project != null)
                    projects.Add(project);
            }
            catch
            {
                // Skip projects that fail to load
            }
        }
        return projects;
    }

    /// <inheritdoc/>
    public async Task<List<ProjectSummary>> GetSubProjectsAsync(int parentId)
    {
        var query = new ProjectQueryParams { ParentId = parentId };
        return await GetAllAsync(query);
    }

    /// <inheritdoc/>
    public async Task<List<ProjectSummary>> SearchAsync(string searchTerm, int take = 20)
    {
        var query = new ProjectQueryParams
        {
            Search = searchTerm,
            Take = take,
            IsActive = true
        };
        return await GetAllAsync(query);
    }

    /// <inheritdoc/>
    public async Task<List<ProjectSummary>> GetByCustomerAsync(int customerId, int take = 100)
    {
        var response = await _client.GetAsync<ApiV2ListResponse<ProjectDetail>>($"api/projects/by-customer/{customerId}?pageSize={take}");

        return response.Data?.Select(d => new ProjectSummary
        {
            Id = d.Id,
            Name = d.Name,
            ProjectNo = d.ProjectNo,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            ProjectStatusId = d.ProjectStatusId,
            CustomerId = d.CustomerId,
            ProjectManagerId = d.ProjectManagerId,
            IsActive = d.IsActive
        }).ToList() ?? new List<ProjectSummary>();
    }

    #endregion
}
