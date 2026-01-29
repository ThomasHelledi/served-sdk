using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.Agreements;

#region View Models

/// <summary>
/// Summary view of an agreement/appointment.
/// </summary>
public class AgreementSummary
{
    /// <summary>Agreement ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Agreement title.</summary>
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Start date/time.</summary>
    [JsonProperty("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>End date/time.</summary>
    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Customer name.</summary>
    [JsonProperty("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("projectName")]
    public string? ProjectName { get; set; }

    /// <summary>Description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }
}

/// <summary>
/// Detailed agreement view.
/// </summary>
public class AgreementDetail
{
    /// <summary>Agreement ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <summary>Tenant ID.</summary>
    [JsonProperty("tenantId")]
    public int TenantId { get; set; }

    /// <summary>Agreement title.</summary>
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Start date/time.</summary>
    [JsonProperty("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>End date/time.</summary>
    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Customer name.</summary>
    [JsonProperty("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Project ID.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Project name.</summary>
    [JsonProperty("projectName")]
    public string? ProjectName { get; set; }

    /// <summary>List of user/employee IDs.</summary>
    [JsonProperty("userIds")]
    public List<int> UserIds { get; set; } = new();

    /// <summary>Created date.</summary>
    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>Updated date.</summary>
    [JsonProperty("updatedDate")]
    public DateTime? UpdatedDate { get; set; }
}

#endregion

#region Request Models

/// <summary>
/// Request to create a new agreement.
/// </summary>
public class CreateAgreementRequest
{
    /// <summary>Agreement title (required).</summary>
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Start date/time.</summary>
    [JsonProperty("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>End date/time.</summary>
    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>List of user/employee IDs.</summary>
    [JsonProperty("userIds")]
    public List<int> UserIds { get; set; } = new();
}

/// <summary>
/// Request to update an existing agreement.
/// </summary>
public class UpdateAgreementRequest
{
    /// <summary>Agreement title.</summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>Start date/time.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>End date/time.</summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Customer ID.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>List of user/employee IDs.</summary>
    [JsonProperty("userIds")]
    public List<int>? UserIds { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("rowVersion")]
    public byte[]? RowVersion { get; set; }
}

/// <summary>
/// Query parameters for agreements.
/// </summary>
public class AgreementQueryParams : Common.QueryParams
{
    /// <summary>Customer ID filter.</summary>
    [JsonProperty("customerId")]
    public int? CustomerId { get; set; }

    /// <summary>Project ID filter.</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Employee ID filter.</summary>
    [JsonProperty("employeeId")]
    public int? EmployeeId { get; set; }

    /// <summary>Start date filter (ISO format string).</summary>
    [JsonIgnore]
    public string? StartDateStr { get; set; }

    /// <summary>End date filter (ISO format string).</summary>
    [JsonIgnore]
    public string? EndDateStr { get; set; }

    /// <summary>Start date filter as DateTime.</summary>
    [JsonProperty("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>End date filter as DateTime.</summary>
    [JsonProperty("endDate")]
    public DateTime? EndDate { get; set; }
}

#endregion

#region Bulk Operations

/// <summary>
/// Request to bulk create agreements.
/// </summary>
public class BulkCreateAgreementsRequest
{
    /// <summary>List of agreements to create.</summary>
    [JsonProperty("items")]
    public List<CreateAgreementRequest> Items { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Item for bulk agreement update.
/// </summary>
public class BulkUpdateAgreementItem
{
    /// <summary>Agreement ID to update.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Update data.</summary>
    [JsonProperty("data")]
    public UpdateAgreementRequest Data { get; set; } = new();
}

/// <summary>
/// Request to bulk update agreements.
/// </summary>
public class BulkUpdateAgreementsRequest
{
    /// <summary>List of agreement updates.</summary>
    [JsonProperty("items")]
    public List<BulkUpdateAgreementItem> Items { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Request to bulk delete agreements.
/// </summary>
public class BulkDeleteAgreementsRequest
{
    /// <summary>List of agreement IDs to delete.</summary>
    [JsonProperty("ids")]
    public List<int> Ids { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

#endregion
