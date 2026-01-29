using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Projects;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for project management operations.
/// </summary>
public interface IProjectClient
{
    #region CRUD Operations

    /// <summary>
    /// Gets all projects with optional filtering and pagination.
    /// </summary>
    /// <param name="query">Optional query parameters.</param>
    /// <returns>List of project summaries.</returns>
    Task<List<ProjectSummary>> GetAllAsync(ProjectQueryParams? query = null);

    /// <summary>
    /// Gets a project by ID.
    /// </summary>
    /// <param name="id">Project ID.</param>
    /// <returns>Project details.</returns>
    Task<ProjectDetail> GetAsync(int id);

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request.</param>
    /// <returns>The created project.</returns>
    Task<ProjectDetail> CreateAsync(CreateProjectRequest request);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">Project ID.</param>
    /// <param name="request">The project update request.</param>
    /// <returns>The updated project.</returns>
    Task<ProjectDetail> UpdateAsync(int id, UpdateProjectRequest request);

    /// <summary>
    /// Deletes a project by ID.
    /// </summary>
    /// <param name="id">The project ID to delete.</param>
    Task DeleteAsync(int id);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Creates multiple projects in a single operation.
    /// </summary>
    /// <param name="request">Bulk creation request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<ProjectDetail>> CreateBulkAsync(BulkCreateProjectsRequest request);

    /// <summary>
    /// Updates multiple projects in a single operation.
    /// </summary>
    /// <param name="request">Bulk update request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<ProjectDetail>> UpdateBulkAsync(BulkUpdateProjectsRequest request);

    /// <summary>
    /// Deletes multiple projects in a single operation.
    /// </summary>
    /// <param name="request">Bulk delete request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<ProjectDetail>> DeleteBulkAsync(BulkDeleteProjectsRequest request);

    #endregion

    #region Query Operations

    /// <summary>
    /// Gets project IDs matching the specified query parameters.
    /// </summary>
    /// <param name="query">Query parameters for filtering projects.</param>
    /// <returns>List of matching project IDs.</returns>
    Task<List<int>> GetKeysAsync(ProjectQueryParams query);

    /// <summary>
    /// Gets multiple projects by their IDs.
    /// </summary>
    /// <param name="ids">List of project IDs to retrieve.</param>
    /// <returns>List of project details.</returns>
    Task<List<ProjectDetail>> GetRangeAsync(List<int> ids);

    /// <summary>
    /// Gets sub-projects for a parent project.
    /// </summary>
    /// <param name="parentId">Parent project ID.</param>
    /// <returns>List of sub-projects.</returns>
    Task<List<ProjectSummary>> GetSubProjectsAsync(int parentId);

    /// <summary>
    /// Searches projects by name or description.
    /// </summary>
    /// <param name="searchTerm">Search term.</param>
    /// <param name="take">Maximum results to return.</param>
    /// <returns>Matching projects.</returns>
    Task<List<ProjectSummary>> SearchAsync(string searchTerm, int take = 20);

    #endregion
}
