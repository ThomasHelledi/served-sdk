using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Tasks;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for task management operations.
/// </summary>
public interface ITaskClient
{
    #region CRUD Operations

    /// <summary>
    /// Gets all tasks with optional filtering and pagination.
    /// </summary>
    /// <param name="query">Optional query parameters.</param>
    /// <returns>List of task summaries.</returns>
    Task<List<TaskSummary>> GetAllAsync(TaskQueryParams? query = null);

    /// <summary>
    /// Gets a task by ID.
    /// </summary>
    /// <param name="id">Task ID.</param>
    /// <returns>Task details.</returns>
    Task<TaskDetail> GetAsync(int id);

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">The task creation request.</param>
    /// <returns>The created task.</returns>
    Task<TaskDetail> CreateAsync(CreateTaskRequest request);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">Task ID.</param>
    /// <param name="request">The task update request.</param>
    /// <returns>The updated task.</returns>
    Task<TaskDetail> UpdateAsync(int id, UpdateTaskRequest request);

    /// <summary>
    /// Deletes a task by ID.
    /// </summary>
    /// <param name="id">The task ID to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Updates task status only.
    /// </summary>
    /// <param name="id">Task ID.</param>
    /// <param name="request">Status update request.</param>
    /// <returns>Updated task.</returns>
    Task<TaskDetail> UpdateStatusAsync(int id, UpdateTaskStatusRequest request);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Creates multiple tasks in a single operation.
    /// </summary>
    /// <param name="request">Bulk creation request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<TaskDetail>> CreateBulkAsync(BulkCreateTasksRequest request);

    /// <summary>
    /// Updates multiple tasks in a single operation.
    /// </summary>
    /// <param name="request">Bulk update request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<TaskDetail>> UpdateBulkAsync(BulkUpdateTasksRequest request);

    /// <summary>
    /// Deletes multiple tasks in a single operation.
    /// </summary>
    /// <param name="request">Bulk delete request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<TaskDetail>> DeleteBulkAsync(BulkDeleteTasksRequest request);

    /// <summary>
    /// Updates status for multiple tasks.
    /// </summary>
    /// <param name="request">Bulk status update request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<TaskDetail>> UpdateStatusBulkAsync(BulkUpdateTaskStatusRequest request);

    #endregion

    #region Query Operations

    /// <summary>
    /// Gets task IDs matching the specified query parameters.
    /// </summary>
    /// <param name="query">Query parameters for filtering tasks.</param>
    /// <returns>List of matching task IDs.</returns>
    Task<List<int>> GetKeysAsync(TaskQueryParams query);

    /// <summary>
    /// Gets multiple tasks by their IDs.
    /// </summary>
    /// <param name="ids">List of task IDs to retrieve.</param>
    /// <returns>List of task details.</returns>
    Task<List<TaskDetail>> GetRangeAsync(List<int> ids);

    /// <summary>
    /// Gets tasks for a specific project.
    /// </summary>
    /// <param name="projectId">Project ID.</param>
    /// <param name="includeCompleted">Include completed tasks.</param>
    /// <returns>List of tasks.</returns>
    Task<List<TaskSummary>> GetByProjectAsync(int projectId, bool includeCompleted = true);

    /// <summary>
    /// Gets tasks assigned to a specific employee.
    /// </summary>
    /// <param name="employeeId">Employee ID.</param>
    /// <param name="includeCompleted">Include completed tasks.</param>
    /// <returns>List of tasks.</returns>
    Task<List<TaskSummary>> GetByAssigneeAsync(int employeeId, bool includeCompleted = false);

    /// <summary>
    /// Gets sub-tasks for a parent task.
    /// </summary>
    /// <param name="parentTaskId">Parent task ID.</param>
    /// <returns>List of sub-tasks.</returns>
    Task<List<TaskSummary>> GetSubTasksAsync(int parentTaskId);

    /// <summary>
    /// Searches tasks by name or description.
    /// </summary>
    /// <param name="searchTerm">Search term.</param>
    /// <param name="take">Maximum results to return.</param>
    /// <returns>Matching tasks.</returns>
    Task<List<TaskSummary>> SearchAsync(string searchTerm, int take = 20);

    #endregion
}
