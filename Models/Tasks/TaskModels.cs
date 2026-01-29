using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.Tasks;

#region Enums

/// <summary>
/// Task status values.
/// </summary>
public enum TaskStatus
{
    /// <summary>New/open task.</summary>
    New = 0,
    /// <summary>Task is in progress.</summary>
    InProgress = 1,
    /// <summary>Task is completed.</summary>
    Completed = 2,
    /// <summary>Task is on hold.</summary>
    OnHold = 3,
    /// <summary>Task is cancelled.</summary>
    Cancelled = 4
}

/// <summary>
/// Task priority levels.
/// </summary>
public enum TaskPriority
{
    /// <summary>Low priority.</summary>
    Low = 0,
    /// <summary>Normal priority.</summary>
    Normal = 1,
    /// <summary>High priority.</summary>
    High = 2,
    /// <summary>Critical priority.</summary>
    Critical = 3
}

#endregion

#region Response Wrappers

/// <summary>
/// Generic list response for task queries.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class TaskListResponse<T>
{
    /// <summary>The data items.</summary>
    [JsonProperty("data")]
    public List<T> Data { get; set; } = new();

    /// <summary>Total count of items (for pagination).</summary>
    [JsonProperty("totalCount")]
    public int? TotalCount { get; set; }
}

/// <summary>
/// Response containing grouped task data.
/// </summary>
public class GroupingResponse
{
    /// <summary>List of query groups.</summary>
    [JsonProperty("groups")]
    public List<QueryGroup> Groups { get; set; } = new();
}

/// <summary>
/// A group of query results.
/// </summary>
public class QueryGroup
{
    /// <summary>Group key.</summary>
    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>Number of items in the group.</summary>
    [JsonProperty("count")]
    public int Count { get; set; }

    /// <summary>Item IDs in the group.</summary>
    [JsonProperty("items")]
    public List<int> Items { get; set; } = new();
}

/// <summary>
/// A single grouping item.
/// </summary>
public class GroupingItem
{
    /// <summary>Item key.</summary>
    [JsonProperty("key")]
    public int Key { get; set; }

    /// <summary>Parent item ID.</summary>
    [JsonProperty("parent")]
    public int? Parent { get; set; }

    /// <summary>Parent group name.</summary>
    [JsonProperty("parentGroup")]
    public string? ParentGroup { get; set; }
}

#endregion

#region View Models

/// <summary>
/// Summary view of a task for listings.
/// </summary>
public class TaskSummary
{
    /// <summary>Task ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Task name.</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Task number.</summary>
    [JsonProperty("taskNo")]
    public string? TaskNo { get; set; }

    /// <summary>Task description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Task status enum.</summary>
    [JsonProperty("status")]
    public TaskStatus Status { get; set; }

    /// <summary>Task status ID.</summary>
    [JsonProperty("taskStatusId")]
    public int? TaskStatusId { get; set; }

    /// <summary>Task type ID.</summary>
    [JsonProperty("taskTypeId")]
    public int? TaskTypeId { get; set; }

    /// <summary>Task priority.</summary>
    [JsonProperty("priority")]
    public TaskPriority? Priority { get; set; }

    /// <summary>Parent task ID.</summary>
    [JsonProperty("parentTaskId")]
    public int? ParentTaskId { get; set; }

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int ProjectId { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("projectName")]
    public string? ProjectName { get; set; }

    /// <summary>Assigned employee ID.</summary>
    [JsonProperty("assignedTo")]
    public int? AssignedTo { get; set; }

    /// <summary>Assigned employee ID (alternative property).</summary>
    [JsonProperty("assignedToId")]
    public int? AssignedToId { get; set; }

    /// <summary>Assigned employee name.</summary>
    [JsonProperty("assignedToName")]
    public string? AssignedToName { get; set; }

    /// <summary>Due date.</summary>
    [JsonProperty("dueDate")]
    public DateTime? DueDate { get; set; }

    /// <summary>Whether task is completed.</summary>
    [JsonProperty("isCompleted")]
    public bool IsCompleted { get; set; }

    /// <summary>Whether task is open (not completed or cancelled).</summary>
    [JsonProperty("isOpen")]
    public bool IsOpen { get; set; }
}

/// <summary>
/// Detailed task view with all fields.
/// </summary>
public class TaskDetail
{
    /// <summary>Task ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <summary>Tenant ID.</summary>
    [JsonProperty("tenantId")]
    public int TenantId { get; set; }

    /// <summary>Task name.</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Task number.</summary>
    [JsonProperty("taskNo")]
    public string? TaskNo { get; set; }

    /// <summary>Task description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Task status enum.</summary>
    [JsonProperty("status")]
    public TaskStatus Status { get; set; }

    /// <summary>Task status ID.</summary>
    [JsonProperty("taskStatusId")]
    public int? TaskStatusId { get; set; }

    /// <summary>Task type ID.</summary>
    [JsonProperty("taskTypeId")]
    public int? TaskTypeId { get; set; }

    /// <summary>Task priority.</summary>
    [JsonProperty("priority")]
    public TaskPriority? Priority { get; set; }

    /// <summary>Parent task ID (parentId from API).</summary>
    [JsonProperty("parentId")]
    public int? ParentId { get; set; }

    /// <summary>Parent task ID alias.</summary>
    [JsonIgnore]
    public int? ParentTaskId => ParentId;

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int ProjectId { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("projectName")]
    public string? ProjectName { get; set; }

    /// <summary>Assigned employee ID.</summary>
    [JsonProperty("assignedTo")]
    public int? AssignedTo { get; set; }

    /// <summary>Assigned employee ID (alternative property).</summary>
    [JsonProperty("assignedToId")]
    public int? AssignedToId { get; set; }

    /// <summary>Assigned employee name.</summary>
    [JsonProperty("assignedToName")]
    public string? AssignedToName { get; set; }

    /// <summary>Whether task is open (not completed or cancelled).</summary>
    [JsonProperty("isOpen")]
    public bool IsOpen { get; set; }

    /// <summary>Start date.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>Due date.</summary>
    [JsonProperty("dueDate")]
    public DateTime? DueDate { get; set; }

    /// <summary>Completion date.</summary>
    [JsonProperty("completedDate")]
    public DateTime? CompletedDate { get; set; }

    /// <summary>Estimated hours.</summary>
    [JsonProperty("estimatedHours")]
    public double? EstimatedHours { get; set; }

    /// <summary>Actual hours spent.</summary>
    [JsonProperty("actualHours")]
    public double? ActualHours { get; set; }

    /// <summary>Task progress (0-100).</summary>
    [JsonProperty("progress")]
    public double Progress { get; set; }

    /// <summary>Whether task is completed.</summary>
    [JsonProperty("isCompleted")]
    public bool IsCompleted { get; set; }

    /// <summary>Created date.</summary>
    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>Updated date.</summary>
    [JsonProperty("updatedDate")]
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Created by user ID.</summary>
    [JsonProperty("createdBy")]
    public int CreatedBy { get; set; }

    /// <summary>Updated by user ID.</summary>
    [JsonProperty("updatedBy")]
    public int? UpdatedBy { get; set; }

    /// <summary>Task tags.</summary>
    [JsonProperty("tags")]
    public string? Tags { get; set; }

    /// <summary>Number of sub-tasks.</summary>
    [JsonProperty("subTaskCount")]
    public int SubTaskCount { get; set; }

    /// <summary>Number of completed sub-tasks.</summary>
    [JsonProperty("completedSubTaskCount")]
    public int CompletedSubTaskCount { get; set; }
}

#endregion

#region Request Models

/// <summary>
/// Request to create a new task.
/// </summary>
public class CreateTaskRequest
{
    /// <summary>Task name (required).</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Project ID (required).</summary>
    [JsonProperty("projectId")]
    public int ProjectId { get; set; }

    /// <summary>Task description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Assigned employee ID.</summary>
    [JsonProperty("assignedTo")]
    public int? AssignedTo { get; set; }

    /// <summary>Task status (default: New).</summary>
    [JsonProperty("status")]
    public TaskStatus Status { get; set; } = TaskStatus.New;

    /// <summary>Task priority.</summary>
    [JsonProperty("priority")]
    public TaskPriority? Priority { get; set; }

    /// <summary>Parent task ID for sub-tasks.</summary>
    [JsonProperty("parentTaskId")]
    public int? ParentTaskId { get; set; }

    /// <summary>Start date.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>Due date.</summary>
    [JsonProperty("dueDate")]
    public DateTime? DueDate { get; set; }

    /// <summary>Estimated hours.</summary>
    [JsonProperty("estimatedHours")]
    public double? EstimatedHours { get; set; }

    /// <summary>Task tags.</summary>
    [JsonProperty("tags")]
    public string? Tags { get; set; }
}

/// <summary>
/// Request to update an existing task.
/// Note: ID is passed separately to the update method.
/// </summary>
public class UpdateTaskRequest
{
    /// <summary>Task name.</summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>Task description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Assigned employee ID.</summary>
    [JsonProperty("assignedTo")]
    public int? AssignedTo { get; set; }

    /// <summary>Task status.</summary>
    [JsonProperty("status")]
    public TaskStatus? Status { get; set; }

    /// <summary>Task priority.</summary>
    [JsonProperty("priority")]
    public TaskPriority? Priority { get; set; }

    /// <summary>Start date.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>Due date.</summary>
    [JsonProperty("dueDate")]
    public DateTime? DueDate { get; set; }

    /// <summary>Estimated hours.</summary>
    [JsonProperty("estimatedHours")]
    public double? EstimatedHours { get; set; }

    /// <summary>Task progress (0-100).</summary>
    [JsonProperty("progress")]
    public double? Progress { get; set; }

    /// <summary>Task tags.</summary>
    [JsonProperty("tags")]
    public string? Tags { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("rowVersion")]
    public byte[]? RowVersion { get; set; }
}

/// <summary>
/// Request to update task status only.
/// </summary>
public class UpdateTaskStatusRequest
{
    /// <summary>New task status.</summary>
    [JsonProperty("status")]
    public TaskStatus Status { get; set; }
}

/// <summary>
/// Query parameters for filtering and paginating tasks.
/// </summary>
public class TaskQueryParams : Common.QueryParams
{
    /// <summary>Project ID filter.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Multiple project IDs filter.</summary>
    [JsonProperty("projectIds")]
    public List<int>? ProjectIds { get; set; }

    /// <summary>Status filter.</summary>
    [JsonProperty("status")]
    public TaskStatus? Status { get; set; }

    /// <summary>Task status ID filter.</summary>
    [JsonProperty("taskStatusId")]
    public int? TaskStatusId { get; set; }

    /// <summary>Task type ID filter.</summary>
    [JsonProperty("taskTypeId")]
    public int? TaskTypeId { get; set; }

    /// <summary>Assigned employee ID filter.</summary>
    [JsonProperty("assignedTo")]
    public int? AssignedTo { get; set; }

    /// <summary>Assigned employee ID filter (alternative).</summary>
    [JsonProperty("assignedToId")]
    public int? AssignedToId { get; set; }

    /// <summary>Parent task ID filter.</summary>
    [JsonProperty("parentTaskId")]
    public int? ParentTaskId { get; set; }

    /// <summary>Include completed tasks.</summary>
    [JsonProperty("includeCompleted")]
    public bool IncludeCompleted { get; set; } = true;

    /// <summary>Filter for open tasks only.</summary>
    [JsonProperty("isOpen")]
    public bool? IsOpen { get; set; }

    /// <summary>Field to sort by.</summary>
    [JsonProperty("sortBy")]
    public string SortBy { get; set; } = "name";

    /// <summary>Sort direction (asc/desc).</summary>
    [JsonProperty("sortDirection")]
    public string SortDirection { get; set; } = "asc";

    /// <summary>Include total count in response.</summary>
    [JsonProperty("includeTotalCount")]
    public bool IncludeTotalCount { get; set; } = true;
}

#endregion

#region Bulk Operations

/// <summary>
/// Request to bulk create tasks.
/// </summary>
public class BulkCreateTasksRequest
{
    /// <summary>List of tasks to create.</summary>
    [JsonProperty("tasks")]
    public List<CreateTaskRequest> Tasks { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Item for bulk task update.
/// </summary>
public class BulkUpdateTaskItem
{
    /// <summary>Task ID to update.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Update data.</summary>
    [JsonProperty("data")]
    public UpdateTaskRequest Data { get; set; } = new();
}

/// <summary>
/// Request to bulk update tasks.
/// </summary>
public class BulkUpdateTasksRequest
{
    /// <summary>List of task updates.</summary>
    [JsonProperty("tasks")]
    public List<BulkUpdateTaskItem> Tasks { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Request to bulk delete tasks.
/// </summary>
public class BulkDeleteTasksRequest
{
    /// <summary>List of task IDs to delete.</summary>
    [JsonProperty("ids")]
    public List<int> Ids { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Request to bulk update task status.
/// </summary>
public class BulkUpdateTaskStatusRequest
{
    /// <summary>List of task IDs to update.</summary>
    [JsonProperty("ids")]
    public List<int> Ids { get; set; } = new();

    /// <summary>New status for all tasks.</summary>
    [JsonProperty("status")]
    public TaskStatus Status { get; set; }
}

#endregion

