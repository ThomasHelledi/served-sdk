using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.Projects;

#region Enums

/// <summary>
/// Project time registration model types.
/// </summary>
public enum ProjectTimeRegistrationModel
{
    /// <summary>All employees can register time.</summary>
    All = 0,
    /// <summary>Only allocated employees can register time.</summary>
    AllocatedOnly = 1,
    /// <summary>Only members can register time.</summary>
    MembersOnly = 2
}

/// <summary>
/// Project stage types.
/// </summary>
public enum ProjectStage
{
    /// <summary>Project is in draft stage.</summary>
    Draft = 1,
    /// <summary>Project is active.</summary>
    Active = 2,
    /// <summary>Project is completed.</summary>
    Completed = 3,
    /// <summary>Project is archived.</summary>
    Archived = 4
}

#endregion

#region Response Wrappers

/// <summary>
/// Generic list response for project queries.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class ProjectListResponse<T>
{
    /// <summary>The data items.</summary>
    [JsonProperty("data")]
    public List<T> Data { get; set; } = new();

    /// <summary>Total count of items (for pagination).</summary>
    [JsonProperty("totalCount")]
    public int? TotalCount { get; set; }
}

#endregion

#region View Models

/// <summary>
/// Basic project summary for lists.
/// </summary>
public class ProjectSummary
{
    /// <summary>Project ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Project number.</summary>
    [JsonProperty("projectNo")]
    public string? ProjectNo { get; set; }

    /// <summary>Project description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Start date.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>End date.</summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Status ID.</summary>
    [JsonProperty("projectStatusId")]
    public int? ProjectStatusId { get; set; }

    /// <summary>Project manager ID.</summary>
    [JsonProperty("projectManagerId")]
    public int? ProjectManagerId { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Whether the project is active.</summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Project progress (0-100).</summary>
    [JsonProperty("progress")]
    public double Progress { get; set; }
}

/// <summary>
/// Full project details with all related data.
/// </summary>
public class ProjectDetail
{
    /// <summary>Project ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <summary>Tenant ID.</summary>
    [JsonProperty("tenantId")]
    public int TenantId { get; set; }

    /// <summary>Created date.</summary>
    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>Last updated date.</summary>
    [JsonProperty("updatedDate")]
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Project category ID.</summary>
    [JsonProperty("projectCategogyId")]
    public int? ProjectCategoryId { get; set; }

    /// <summary>Project type ID.</summary>
    [JsonProperty("projectTypeId")]
    public int ProjectTypeId { get; set; }

    /// <summary>Project status ID.</summary>
    [JsonProperty("projectStatusId")]
    public int ProjectStatusId { get; set; }

    /// <summary>Project stage ID.</summary>
    [JsonProperty("projectStageId")]
    public int ProjectStageId { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Full project name.</summary>
    [JsonProperty("fullName")]
    public string? FullName { get; set; }

    /// <summary>Project description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Sort order.</summary>
    [JsonProperty("sortOrder")]
    public int SortOrder { get; set; }

    /// <summary>Whether the project is active.</summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Project number.</summary>
    [JsonProperty("projectNo")]
    public string? ProjectNo { get; set; }

    /// <summary>Start date.</summary>
    [JsonProperty("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>End date.</summary>
    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }

    /// <summary>Project profitability percentage.</summary>
    [JsonProperty("projectProfitability")]
    public double? ProjectProfitability { get; set; }

    /// <summary>Contract amount.</summary>
    [JsonProperty("contractAmount")]
    public double? ContractAmount { get; set; }

    /// <summary>Project budget amount.</summary>
    [JsonProperty("projectBudgetAmount")]
    public double? ProjectBudgetAmount { get; set; }

    /// <summary>Project budget hours.</summary>
    [JsonProperty("projectBudgetHours")]
    public double? ProjectBudgetHours { get; set; }

    /// <summary>Tasks budget amount.</summary>
    [JsonProperty("tasksBudgetAmount")]
    public double? TasksBudgetAmount { get; set; }

    /// <summary>Tasks budget hours.</summary>
    [JsonProperty("tasksBudgetHours")]
    public double? TasksBudgetHours { get; set; }

    /// <summary>Allocation hours.</summary>
    [JsonProperty("allocationHours")]
    public double? AllocationHours { get; set; }

    /// <summary>Allocation amount.</summary>
    [JsonProperty("allocationAmount")]
    public double? AllocationAmount { get; set; }

    /// <summary>Registered hours.</summary>
    [JsonProperty("regHours")]
    public double? RegHours { get; set; }

    /// <summary>Adjusted hours.</summary>
    [JsonProperty("adjHours")]
    public double? AdjHours { get; set; }

    /// <summary>Invoiced hours.</summary>
    [JsonProperty("invHours")]
    public double? InvHours { get; set; }

    /// <summary>Billed hours.</summary>
    [JsonProperty("billedHours")]
    public double? BilledHours { get; set; }

    /// <summary>Cost amount.</summary>
    [JsonProperty("costAmount")]
    public double? CostAmount { get; set; }

    /// <summary>Registered amount.</summary>
    [JsonProperty("regAmount")]
    public double? RegAmount { get; set; }

    /// <summary>Adjusted amount.</summary>
    [JsonProperty("adjAmount")]
    public double? AdjAmount { get; set; }

    /// <summary>Invoiced amount.</summary>
    [JsonProperty("invAmount")]
    public double? InvAmount { get; set; }

    /// <summary>Billed amount.</summary>
    [JsonProperty("billedAmount")]
    public double? BilledAmount { get; set; }

    /// <summary>Approval manager ID.</summary>
    [JsonProperty("approvalManagerId")]
    public int? ApprovalManagerId { get; set; }

    /// <summary>Project partner ID.</summary>
    [JsonProperty("projectPartnerId")]
    public int? ProjectPartnerId { get; set; }

    /// <summary>Project manager ID.</summary>
    [JsonProperty("projectManagerId")]
    public int? ProjectManagerId { get; set; }

    /// <summary>Parent project ID.</summary>
    [JsonProperty("parentId")]
    public int? ParentId { get; set; }

    /// <summary>External entity ID.</summary>
    [JsonProperty("exposedEntityId")]
    public string? ExposedEntityId { get; set; }

    /// <summary>Time registration model setting.</summary>
    [JsonProperty("timeRegistrationModel")]
    public ProjectTimeRegistrationModel? TimeRegistrationModel { get; set; }

    /// <summary>Project tags (comma-separated).</summary>
    [JsonProperty("tags")]
    public string? Tags { get; set; }

    /// <summary>Project summary.</summary>
    [JsonProperty("summary")]
    public string? Summary { get; set; }

    /// <summary>Project color (hex).</summary>
    [JsonProperty("color")]
    public string? Color { get; set; }

    /// <summary>Project progress (0-100).</summary>
    [JsonProperty("progress")]
    public double Progress { get; set; }

    /// <summary>List of member employee IDs.</summary>
    [JsonProperty("memberIds")]
    public List<int> MemberIds { get; set; } = new();

    /// <summary>Whether this is a parent project.</summary>
    [JsonProperty("isParent")]
    public bool IsParent { get; set; }

    /// <summary>Created by user ID.</summary>
    [JsonProperty("createdBy")]
    public int CreatedBy { get; set; }

    /// <summary>Updated by user ID.</summary>
    [JsonProperty("updatedBy")]
    public int? UpdatedBy { get; set; }

    /// <summary>Whether custom fields are loaded.</summary>
    [JsonProperty("isCustomFieldsLoaded")]
    public bool IsCustomFieldsLoaded { get; set; }
}

#endregion

#region Request Models

/// <summary>
/// Request to create a new project.
/// </summary>
public class CreateProjectRequest
{
    /// <summary>Project name (required).</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Project description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Project summary.</summary>
    [JsonProperty("summary")]
    public string? Summary { get; set; }

    /// <summary>Start date.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>End date.</summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Project manager ID.</summary>
    [JsonProperty("projectManagerId")]
    public int? ProjectManagerId { get; set; }

    /// <summary>Organization/workspace ID.</summary>
    [JsonProperty("organizationId")]
    public int? OrganizationId { get; set; }

    /// <summary>Project type ID.</summary>
    [JsonProperty("projectTypeId")]
    public int? ProjectTypeId { get; set; }

    /// <summary>Project status ID.</summary>
    [JsonProperty("projectStatusId")]
    public int? ProjectStatusId { get; set; }

    /// <summary>Parent project ID for sub-projects.</summary>
    [JsonProperty("parentId")]
    public int? ParentId { get; set; }

    /// <summary>Project color (hex).</summary>
    [JsonProperty("color")]
    public string? Color { get; set; }

    /// <summary>Project tags (comma-separated).</summary>
    [JsonProperty("tags")]
    public string? Tags { get; set; }
}

/// <summary>
/// Request to update an existing project.
/// Note: ID is passed separately to the update method.
/// </summary>
public class UpdateProjectRequest
{
    /// <summary>Project name.</summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>Project description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Project summary.</summary>
    [JsonProperty("summary")]
    public string? Summary { get; set; }

    /// <summary>Start date.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>End date.</summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Project status ID.</summary>
    [JsonProperty("projectStatusId")]
    public int? ProjectStatusId { get; set; }

    /// <summary>Project manager ID.</summary>
    [JsonProperty("projectManagerId")]
    public int? ProjectManagerId { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Whether the project is active.</summary>
    [JsonProperty("isActive")]
    public bool? IsActive { get; set; }

    /// <summary>Project color (hex).</summary>
    [JsonProperty("color")]
    public string? Color { get; set; }

    /// <summary>Project tags (comma-separated).</summary>
    [JsonProperty("tags")]
    public string? Tags { get; set; }

    /// <summary>Project progress (0-100).</summary>
    [JsonProperty("progress")]
    public double? Progress { get; set; }

    /// <summary>Time registration model setting.</summary>
    [JsonProperty("timeRegistrationModel")]
    public ProjectTimeRegistrationModel? TimeRegistrationModel { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("rowVersion")]
    public byte[]? RowVersion { get; set; }
}

/// <summary>
/// Query parameters for filtering and paginating projects.
/// </summary>
public class ProjectQueryParams : Common.QueryParams
{
    /// <summary>Organization/workspace ID filter.</summary>
    [JsonProperty("organizationId")]
    public int? OrganizationId { get; set; }

    /// <summary>Customer ID filter.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Project status ID filter.</summary>
    [JsonProperty("projectStatusId")]
    public int? ProjectStatusId { get; set; }

    /// <summary>Project manager ID filter.</summary>
    [JsonProperty("projectManagerId")]
    public int? ProjectManagerId { get; set; }

    /// <summary>Parent project ID filter.</summary>
    [JsonProperty("parentId")]
    public int? ParentId { get; set; }

    /// <summary>Active status filter.</summary>
    [JsonProperty("isActive")]
    public bool? IsActive { get; set; }

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
/// Request to bulk create projects.
/// </summary>
public class BulkCreateProjectsRequest
{
    /// <summary>List of projects to create.</summary>
    [JsonProperty("projects")]
    public List<CreateProjectRequest> Projects { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Item for bulk project update.
/// </summary>
public class BulkUpdateProjectItem
{
    /// <summary>Project ID to update.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Update data.</summary>
    [JsonProperty("data")]
    public UpdateProjectRequest Data { get; set; } = new();
}

/// <summary>
/// Request to bulk update projects.
/// </summary>
public class BulkUpdateProjectsRequest
{
    /// <summary>List of project updates.</summary>
    [JsonProperty("projects")]
    public List<BulkUpdateProjectItem> Projects { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Request to bulk delete projects.
/// </summary>
public class BulkDeleteProjectsRequest
{
    /// <summary>List of project IDs to delete.</summary>
    [JsonProperty("ids")]
    public List<int> Ids { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

#endregion
