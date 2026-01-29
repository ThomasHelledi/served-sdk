using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.TimeRegistration;

#region View Models

/// <summary>
/// Summary view of a time registration.
/// </summary>
public class TimeRegistrationSummary
{
    /// <summary>Time registration ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Registration date.</summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("projectName")]
    public string? ProjectName { get; set; }

    /// <summary>Task ID.</summary>
    [JsonProperty("taskId")]
    public int? TaskId { get; set; }

    /// <summary>Task name.</summary>
    [JsonProperty("taskName")]
    public string? TaskName { get; set; }

    /// <summary>Hours registered.</summary>
    [JsonProperty("hours")]
    public double? Hours { get; set; }

    /// <summary>Minutes registered.</summary>
    [JsonProperty("minutes")]
    public int? Minutes { get; set; }

    /// <summary>Description/comment.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Employee ID.</summary>
    [JsonProperty("employeeId")]
    public int? EmployeeId { get; set; }

    /// <summary>Employee name.</summary>
    [JsonProperty("employeeName")]
    public string? EmployeeName { get; set; }
}

/// <summary>
/// Detailed time registration view.
/// </summary>
public class TimeRegistrationDetail
{
    /// <summary>Time registration ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <summary>Tenant ID.</summary>
    [JsonProperty("tenantId")]
    public int TenantId { get; set; }

    /// <summary>Registration date.</summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    /// <summary>Start date/time.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>End date/time.</summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Comment/description.</summary>
    [JsonProperty("comment")]
    public string? Comment { get; set; }

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Task ID.</summary>
    [JsonProperty("taskId")]
    public int? TaskId { get; set; }

    /// <summary>Hours registered.</summary>
    [JsonProperty("hours")]
    public double? Hours { get; set; }

    /// <summary>Minutes registered.</summary>
    [JsonProperty("minutes")]
    public int? Minutes { get; set; }

    /// <summary>Task name.</summary>
    [JsonProperty("taskName")]
    public string? TaskName { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("projectName")]
    public string? ProjectName { get; set; }

    /// <summary>Whether the registration is billable.</summary>
    [JsonProperty("billable")]
    public bool Billable { get; set; }

    /// <summary>Employee ID.</summary>
    [JsonProperty("employeeId")]
    public int? EmployeeId { get; set; }

    /// <summary>Created date.</summary>
    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>Updated date.</summary>
    [JsonProperty("updatedDate")]
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Description/comment (alias for Comment).</summary>
    [JsonProperty("description")]
    public string? Description { get => Comment; set => Comment = value; }

    /// <summary>Employee name.</summary>
    [JsonProperty("employeeName")]
    public string? EmployeeName { get; set; }
}

#endregion

#region Request Models

/// <summary>
/// Request to create a time registration.
/// </summary>
public class CreateTimeRegistrationRequest
{
    /// <summary>Start date/time.</summary>
    [JsonProperty("start")]
    public DateTime Start { get; set; }

    /// <summary>End date/time.</summary>
    [JsonProperty("end")]
    public DateTime End { get; set; }

    /// <summary>Task ID.</summary>
    [JsonProperty("taskId")]
    public int? TaskId { get; set; }

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Description/comment.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Whether billable.</summary>
    [JsonProperty("billable")]
    public bool Billable { get; set; }

    /// <summary>Minutes (alternative to start/end).</summary>
    [JsonProperty("minutes")]
    public int Minutes { get; set; }
}

/// <summary>
/// Request to update a time registration.
/// </summary>
public class UpdateTimeRegistrationRequest
{
    /// <summary>Start date/time.</summary>
    [JsonProperty("start")]
    public DateTime? Start { get; set; }

    /// <summary>End date/time.</summary>
    [JsonProperty("end")]
    public DateTime? End { get; set; }

    /// <summary>Task ID.</summary>
    [JsonProperty("taskId")]
    public int? TaskId { get; set; }

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Description/comment.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Whether billable.</summary>
    [JsonProperty("billable")]
    public bool? Billable { get; set; }

    /// <summary>Minutes.</summary>
    [JsonProperty("minutes")]
    public int? Minutes { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("rowVersion")]
    public byte[]? RowVersion { get; set; }
}

/// <summary>
/// Query parameters for time registrations.
/// </summary>
public class TimeRegistrationQueryParams : Common.QueryParams
{
    /// <summary>Period start date (ISO format string).</summary>
    [JsonProperty("startDate")]
    public string? StartDate { get; set; }

    /// <summary>Period end date (ISO format string).</summary>
    [JsonProperty("endDate")]
    public string? EndDate { get; set; }

    /// <summary>Period start as DateTime (legacy support).</summary>
    [JsonIgnore]
    public DateTime? Start { get; set; }

    /// <summary>Period end as DateTime (legacy support).</summary>
    [JsonIgnore]
    public DateTime? End { get; set; }

    /// <summary>Project ID filter.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Task ID filter.</summary>
    [JsonProperty("taskId")]
    public int? TaskId { get; set; }

    /// <summary>Employee ID filter.</summary>
    [JsonProperty("employeeId")]
    public int? EmployeeId { get; set; }
}

#endregion

#region Bulk Operations

/// <summary>
/// Request to bulk create time registrations.
/// </summary>
public class BulkCreateTimeRegistrationsRequest
{
    /// <summary>List of time registrations to create.</summary>
    [JsonProperty("items")]
    public List<CreateTimeRegistrationRequest> Items { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Item for bulk time registration update.
/// </summary>
public class BulkUpdateTimeRegistrationItem
{
    /// <summary>Time registration ID to update.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Update data.</summary>
    [JsonProperty("data")]
    public UpdateTimeRegistrationRequest Data { get; set; } = new();
}

/// <summary>
/// Request to bulk update time registrations.
/// </summary>
public class BulkUpdateTimeRegistrationsRequest
{
    /// <summary>List of time registration updates.</summary>
    [JsonProperty("items")]
    public List<BulkUpdateTimeRegistrationItem> Items { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Request to bulk delete time registrations.
/// </summary>
public class BulkDeleteTimeRegistrationsRequest
{
    /// <summary>List of time registration IDs to delete.</summary>
    [JsonProperty("ids")]
    public List<int> Ids { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

#endregion
