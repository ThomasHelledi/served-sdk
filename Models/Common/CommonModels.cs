using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.Common;

/// <summary>
/// Represents standard query parameters for listing data.
/// </summary>
public class QueryParams
{
    /// <summary>
    /// Maximum number of items to return.
    /// </summary>
    public int? Take { get; set; }

    /// <summary>
    /// Number of items to skip for pagination.
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// Field name to sort by.
    /// </summary>
    public string Sort { get; set; } = "";

    /// <summary>
    /// Filter expression string.
    /// </summary>
    public string Filter { get; set; } = "";

    /// <summary>
    /// Search term for filtering results.
    /// </summary>
    public string Search { get; set; } = "";

    /// <summary>
    /// List of specific IDs to retrieve.
    /// </summary>
    public List<object> Keys { get; set; } = new List<object>();
}

/// <summary>
/// Represents a date range for filtering queries.
/// </summary>
public class PeriodModel
{
    /// <summary>
    /// Start date of the period in ISO format (yyyy-MM-dd).
    /// </summary>
    [JsonProperty("startsAt")]
    public string? StartsAt { get; set; }

    /// <summary>
    /// End date of the period in ISO format (yyyy-MM-dd).
    /// </summary>
    [JsonProperty("endsAt")]
    public string? EndsAt { get; set; }

    /// <summary>
    /// Creates a PeriodModel from DateTime values.
    /// </summary>
    /// <param name="start">Start date.</param>
    /// <param name="end">End date.</param>
    /// <returns>A PeriodModel with formatted date strings.</returns>
    public static PeriodModel FromDates(DateTime start, DateTime end)
    {
        return new PeriodModel
        {
            StartsAt = start.ToString("yyyy-MM-dd"),
            EndsAt = end.ToString("yyyy-MM-dd")
        };
    }
}

/// <summary>
/// Extended request filter with tenant and workspace context.
/// </summary>
public class RequestFilter : QueryParams
{
    /// <summary>
    /// Tenant ID for multi-tenant filtering.
    /// </summary>
    public int? TenantId { get; set; }

    /// <summary>
    /// Workspace ID for workspace-scoped filtering.
    /// </summary>
    public int? WorkspaceId { get; set; }

    /// <summary>
    /// Location ID for location-scoped filtering.
    /// </summary>
    public int? LocationId { get; set; }

    /// <summary>
    /// Date period for time-based filtering.
    /// </summary>
    public PeriodModel? Period { get; set; }
}

/// <summary>
/// Minimal tenant view model.
/// </summary>
public class TenantViewModel
{
    /// <summary>
    /// Unique tenant identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tenant display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly tenant slug.
    /// </summary>
    public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Minimal workspace view model.
/// </summary>
public class WorkspaceViewModel
{
    /// <summary>
    /// Unique workspace identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Workspace display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly workspace slug.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Type of workspace (e.g., "project", "sales").
    /// </summary>
    public string WorkspaceType { get; set; } = string.Empty;
}

/// <summary>
/// Generic wrapper for cached data items returned from cache providers.
/// </summary>
/// <typeparam name="T">The type of cached data.</typeparam>
public class CacheDataItem<T>
{
    /// <summary>
    /// Entity identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Entity version for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Whether the entity has been deleted.
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// The cached entity data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Timestamp when the data was cached.
    /// </summary>
    public DateTime CachedDateTime { get; set; }
}

/// <summary>
/// Request structure for deleting multiple entities.
/// </summary>
public class DeleteRequest
{
    /// <summary>
    /// Tenant ID context for the delete operation.
    /// </summary>
    [JsonProperty("tenantId")]
    public int TenantId { get; set; }

    /// <summary>
    /// List of items to delete.
    /// </summary>
    [JsonProperty("items")]
    public List<DeleteItem> Items { get; set; } = new List<DeleteItem>();
}

/// <summary>
/// Represents a single item to be deleted.
/// </summary>
public class DeleteItem
{
    /// <summary>
    /// Entity identifier to delete.
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Entity version for optimistic concurrency check.
    /// </summary>
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <summary>
    /// Domain type identifier indicating the entity type.
    /// </summary>
    /// <seealso cref="DomainType"/>
    [JsonProperty("domainType")]
    public int DomainType { get; set; }
}

/// <summary>
/// Domain type identifiers used for entity classification in delete operations.
/// </summary>
public enum DomainType
{
    /// <summary>
    /// Project entity type.
    /// </summary>
    Project = 1,

    /// <summary>
    /// Task entity type.
    /// </summary>
    Task = 2,

    /// <summary>
    /// Time registration entity type.
    /// </summary>
    TimeRegistration = 3,

    /// <summary>
    /// Agreement/appointment entity type.
    /// </summary>
    Agreement = 4
}

/// <summary>
/// API V2 standard list response wrapper with pagination metadata.
/// </summary>
/// <typeparam name="T">Type of items in the list.</typeparam>
public class ApiV2ListResponse<T>
{
    /// <summary>
    /// List of items returned by the query.
    /// </summary>
    public List<T>? Data { get; set; }

    /// <summary>
    /// Pagination metadata.
    /// </summary>
    public ApiV2PageMeta? Meta { get; set; }
}

/// <summary>
/// Pagination metadata for API V2 list responses.
/// </summary>
public class ApiV2PageMeta
{
    /// <summary>
    /// Total number of items matching the query.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;
}

/// <summary>
/// Standard API list response wrapper with pagination metadata.
/// Used across all SDK API clients for consistent response handling.
/// </summary>
/// <typeparam name="T">Type of items in the list.</typeparam>
public class ApiListResponse<T>
{
    /// <summary>
    /// List of items returned by the query.
    /// </summary>
    public List<T>? Data { get; set; }

    /// <summary>
    /// Pagination metadata.
    /// </summary>
    public ApiPageMeta? Meta { get; set; }
}

/// <summary>
/// Pagination metadata for API list responses.
/// </summary>
public class ApiPageMeta
{
    /// <summary>
    /// Total number of items matching the query.
    /// </summary>
    public int Total { get; set; }
}

/// <summary>
/// Response from bulk operations.
/// </summary>
/// <typeparam name="T">Type of created/updated items.</typeparam>
public class BulkResponse<T>
{
    /// <summary>Total items in request.</summary>
    public int Total { get; set; }

    /// <summary>Successfully processed items.</summary>
    public int Succeeded { get; set; }

    /// <summary>Failed items.</summary>
    public int Failed { get; set; }

    /// <summary>List of successfully created/updated items.</summary>
    public List<T> Items { get; set; } = new();

    /// <summary>Error messages for failed items.</summary>
    public List<BulkError> Errors { get; set; } = new();
}

/// <summary>
/// Error details for a single item in bulk operation.
/// </summary>
public class BulkError
{
    /// <summary>Index of the item that failed.</summary>
    public int Index { get; set; }

    /// <summary>Item ID if available.</summary>
    public int? ItemId { get; set; }

    /// <summary>Error message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Error code if available.</summary>
    public string? Code { get; set; }
}
