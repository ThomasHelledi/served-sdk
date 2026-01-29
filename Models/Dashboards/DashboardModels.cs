using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.Dashboards;

#region Enums

/// <summary>
/// Dashboard scope determines who can access the dashboard.
/// </summary>
public enum DashboardScope
{
    /// <summary>Personal dashboard visible only to the owner.</summary>
    Personal = 0,
    /// <summary>Dashboard shared with workspace members.</summary>
    Workspace = 1,
    /// <summary>Dashboard scoped to a specific project.</summary>
    Project = 2,
    /// <summary>Dashboard available to all organization members.</summary>
    Organization = 3
}

/// <summary>
/// Widget type identifiers for dashboard widgets.
/// </summary>
public enum WidgetType
{
    /// <summary>Key Performance Indicator card.</summary>
    KPI = 1,
    /// <summary>Line chart for time series data.</summary>
    LineChart = 2,
    /// <summary>Bar chart for categorical data.</summary>
    BarChart = 3,
    /// <summary>Pie/donut chart for proportional data.</summary>
    PieChart = 4,
    /// <summary>Data table widget.</summary>
    Table = 5,
    /// <summary>Activity feed showing recent events.</summary>
    ActivityFeed = 6,
    /// <summary>User's current tasks list.</summary>
    MyTasks = 7,
    /// <summary>Quick action buttons widget.</summary>
    QuickActions = 8,
    /// <summary>Gauge/meter widget.</summary>
    Gauge = 9,
    /// <summary>Progress bar widget.</summary>
    ProgressBar = 10,
    /// <summary>Markdown/HTML text widget.</summary>
    Text = 11,
    /// <summary>Embedded iframe widget.</summary>
    Embed = 12
}

#endregion

#region Dashboard Models

/// <summary>
/// Summary view of a dashboard for listing purposes.
/// </summary>
public class DashboardSummary
{
    /// <summary>Unique dashboard identifier.</summary>
    public int Id { get; set; }

    /// <summary>Dashboard display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Dashboard scope (Personal, Workspace, Project, Organization).</summary>
    public DashboardScope Scope { get; set; }

    /// <summary>Number of widgets on the dashboard.</summary>
    public int WidgetCount { get; set; }

    /// <summary>Whether this is the user's default dashboard.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Dashboard color theme.</summary>
    public string? Theme { get; set; }

    /// <summary>Creation timestamp.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Last update timestamp.</summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>ID of the user who created the dashboard.</summary>
    public int CreatedByUserId { get; set; }
}

/// <summary>
/// Detailed dashboard view including all widgets.
/// </summary>
public class DashboardDetail
{
    /// <summary>Unique dashboard identifier.</summary>
    public int Id { get; set; }

    /// <summary>Dashboard display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Dashboard scope.</summary>
    public DashboardScope Scope { get; set; }

    /// <summary>Dashboard color theme (light, dark, system).</summary>
    public string? Theme { get; set; }

    /// <summary>Auto-refresh interval in seconds (0 = disabled).</summary>
    public int? RefreshIntervalSeconds { get; set; }

    /// <summary>Whether this is the user's default dashboard.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Associated workspace ID (for Workspace scope).</summary>
    public int? WorkspaceId { get; set; }

    /// <summary>Associated project ID (for Project scope).</summary>
    public int? ProjectId { get; set; }

    /// <summary>Associated board ID (optional).</summary>
    public int? BoardId { get; set; }

    /// <summary>Layout configuration JSON.</summary>
    public string? LayoutConfig { get; set; }

    /// <summary>Creation timestamp.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Last update timestamp.</summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>ID of the user who created the dashboard.</summary>
    public int CreatedByUserId { get; set; }

    /// <summary>All widgets on this dashboard.</summary>
    public List<WidgetDetail> Widgets { get; set; } = new();
}

/// <summary>
/// Request model for creating a new dashboard.
/// </summary>
public class CreateDashboardRequest
{
    /// <summary>Dashboard display name (required).</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Dashboard scope (defaults to Personal).</summary>
    [JsonProperty("scope")]
    public DashboardScope Scope { get; set; } = DashboardScope.Personal;

    /// <summary>Dashboard color theme.</summary>
    [JsonProperty("theme")]
    public string? Theme { get; set; }

    /// <summary>Auto-refresh interval in seconds.</summary>
    [JsonProperty("refreshIntervalSeconds")]
    public int? RefreshIntervalSeconds { get; set; }

    /// <summary>Associated workspace ID (required for Workspace scope).</summary>
    [JsonProperty("workspaceId")]
    public int? WorkspaceId { get; set; }

    /// <summary>Associated project ID (required for Project scope).</summary>
    [JsonProperty("projectId")]
    public int? ProjectId { get; set; }

    /// <summary>Associated board ID (optional).</summary>
    [JsonProperty("boardId")]
    public int? BoardId { get; set; }
}

/// <summary>
/// Request model for updating an existing dashboard.
/// </summary>
public class UpdateDashboardRequest
{
    /// <summary>Updated dashboard name.</summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>Updated description.</summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>Updated theme.</summary>
    [JsonProperty("theme")]
    public string? Theme { get; set; }

    /// <summary>Updated refresh interval.</summary>
    [JsonProperty("refreshIntervalSeconds")]
    public int? RefreshIntervalSeconds { get; set; }

    /// <summary>Layout configuration JSON.</summary>
    [JsonProperty("layoutConfig")]
    public string? LayoutConfig { get; set; }
}

#endregion

#region Widget Models

/// <summary>
/// Detailed widget view including all configuration.
/// </summary>
public class WidgetDetail
{
    /// <summary>Unique widget identifier.</summary>
    public int Id { get; set; }

    /// <summary>Parent dashboard ID.</summary>
    public int DashboardId { get; set; }

    /// <summary>Widget type identifier.</summary>
    public WidgetType Type { get; set; }

    /// <summary>Widget type as string (e.g., "KPI", "LineChart").</summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>Widget title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional subtitle.</summary>
    public string? Subtitle { get; set; }

    /// <summary>Icon name (e.g., "chart-line", "currency-dollar").</summary>
    public string? Icon { get; set; }

    /// <summary>Grid X position (column).</summary>
    public int GridX { get; set; }

    /// <summary>Grid Y position (row).</summary>
    public int GridY { get; set; }

    /// <summary>Widget width in grid units.</summary>
    public int GridWidth { get; set; }

    /// <summary>Widget height in grid units.</summary>
    public int GridHeight { get; set; }

    /// <summary>Widget-specific configuration JSON.</summary>
    public string? Config { get; set; }

    /// <summary>Datasource query configuration JSON.</summary>
    public string? DatasourceConfig { get; set; }

    /// <summary>Data transformation configuration JSON.</summary>
    public string? TransformConfig { get; set; }

    /// <summary>Style/appearance configuration JSON.</summary>
    public string? StyleConfig { get; set; }

    /// <summary>Creation timestamp.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Last update timestamp.</summary>
    public DateTime? UpdatedDate { get; set; }
}

/// <summary>
/// Request model for creating a new widget.
/// </summary>
public class CreateWidgetRequest
{
    /// <summary>Widget type (e.g., "KPI", "LineChart").</summary>
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Widget title (required).</summary>
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional subtitle.</summary>
    [JsonProperty("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>Icon name.</summary>
    [JsonProperty("icon")]
    public string? Icon { get; set; }

    /// <summary>Grid X position.</summary>
    [JsonProperty("gridX")]
    public int GridX { get; set; }

    /// <summary>Grid Y position.</summary>
    [JsonProperty("gridY")]
    public int GridY { get; set; }

    /// <summary>Widget width in grid units.</summary>
    [JsonProperty("gridWidth")]
    public int GridWidth { get; set; } = 3;

    /// <summary>Widget height in grid units.</summary>
    [JsonProperty("gridHeight")]
    public int GridHeight { get; set; } = 2;

    /// <summary>Widget-specific configuration.</summary>
    [JsonProperty("config")]
    public object? Config { get; set; }

    /// <summary>Datasource query configuration.</summary>
    [JsonProperty("datasourceConfig")]
    public object? DatasourceConfig { get; set; }

    /// <summary>Style configuration.</summary>
    [JsonProperty("styleConfig")]
    public object? StyleConfig { get; set; }
}

/// <summary>
/// Request model for updating an existing widget.
/// </summary>
public class UpdateWidgetRequest
{
    /// <summary>Updated widget title.</summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>Updated subtitle.</summary>
    [JsonProperty("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>Updated icon.</summary>
    [JsonProperty("icon")]
    public string? Icon { get; set; }

    /// <summary>Updated grid X position.</summary>
    [JsonProperty("gridX")]
    public int? GridX { get; set; }

    /// <summary>Updated grid Y position.</summary>
    [JsonProperty("gridY")]
    public int? GridY { get; set; }

    /// <summary>Updated width.</summary>
    [JsonProperty("gridWidth")]
    public int? GridWidth { get; set; }

    /// <summary>Updated height.</summary>
    [JsonProperty("gridHeight")]
    public int? GridHeight { get; set; }

    /// <summary>Updated widget configuration.</summary>
    [JsonProperty("config")]
    public object? Config { get; set; }

    /// <summary>Updated datasource configuration.</summary>
    [JsonProperty("datasourceConfig")]
    public object? DatasourceConfig { get; set; }

    /// <summary>Updated style configuration.</summary>
    [JsonProperty("styleConfig")]
    public object? StyleConfig { get; set; }
}

/// <summary>
/// Widget position for batch layout updates.
/// </summary>
public class WidgetLayoutItem
{
    /// <summary>Widget ID.</summary>
    [JsonProperty("widgetId")]
    public int WidgetId { get; set; }

    /// <summary>New grid X position.</summary>
    [JsonProperty("gridX")]
    public int GridX { get; set; }

    /// <summary>New grid Y position.</summary>
    [JsonProperty("gridY")]
    public int GridY { get; set; }

    /// <summary>New width.</summary>
    [JsonProperty("gridWidth")]
    public int GridWidth { get; set; }

    /// <summary>New height.</summary>
    [JsonProperty("gridHeight")]
    public int GridHeight { get; set; }
}

#endregion

#region Bulk Operations

/// <summary>
/// Request for bulk widget creation.
/// </summary>
public class BulkCreateWidgetsRequest
{
    /// <summary>List of widgets to create.</summary>
    [JsonProperty("widgets")]
    public List<CreateWidgetRequest> Widgets { get; set; } = new();
}

/// <summary>
/// Request for bulk widget update.
/// </summary>
public class BulkUpdateWidgetsRequest
{
    /// <summary>List of widget updates with IDs.</summary>
    [JsonProperty("widgets")]
    public List<BulkWidgetUpdate> Widgets { get; set; } = new();
}

/// <summary>
/// Single widget update item for bulk operations.
/// </summary>
public class BulkWidgetUpdate : UpdateWidgetRequest
{
    /// <summary>Widget ID to update.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }
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

#endregion

#region API Response Wrappers

/// <summary>
/// Standard list response wrapper.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class DashboardListResponse<T>
{
    /// <summary>List of items.</summary>
    [JsonProperty("data")]
    public List<T> Data { get; set; } = new();

    /// <summary>Total count of items.</summary>
    [JsonProperty("total")]
    public int Total { get; set; }
}

#endregion

#region View Model Aliases

/// <summary>
/// Dashboard view model alias for API compatibility.
/// </summary>
public class DashboardViewModel : DashboardDetail
{
}

#endregion

#region Datasource Models

/// <summary>
/// Available entity for datasource queries.
/// </summary>
public class EntityViewModel
{
    /// <summary>Entity name/key.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Entity description.</summary>
    public string? Description { get; set; }

    /// <summary>Number of available fields.</summary>
    public int FieldCount { get; set; }

    /// <summary>Whether the entity supports filtering.</summary>
    public bool SupportsFiltering { get; set; }

    /// <summary>Whether the entity supports aggregation.</summary>
    public bool SupportsAggregation { get; set; }
}

/// <summary>
/// Entity metadata including available fields.
/// </summary>
public class EntityMetadataViewModel
{
    /// <summary>Entity name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Entity description.</summary>
    public string? Description { get; set; }

    /// <summary>Available fields.</summary>
    public List<EntityFieldViewModel> Fields { get; set; } = new();

    /// <summary>Available relationships.</summary>
    public List<EntityRelationshipViewModel> Relationships { get; set; } = new();
}

/// <summary>
/// Field definition for an entity.
/// </summary>
public class EntityFieldViewModel
{
    /// <summary>Field name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Data type.</summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>Whether the field can be filtered.</summary>
    public bool Filterable { get; set; }

    /// <summary>Whether the field can be sorted.</summary>
    public bool Sortable { get; set; }

    /// <summary>Whether the field can be aggregated.</summary>
    public bool Aggregatable { get; set; }
}

/// <summary>
/// Relationship definition for an entity.
/// </summary>
public class EntityRelationshipViewModel
{
    /// <summary>Relationship name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Target entity name.</summary>
    public string TargetEntity { get; set; } = string.Empty;

    /// <summary>Relationship type (one-to-one, one-to-many, etc.).</summary>
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Request for executing a datasource query.
/// </summary>
public class DatasourceQueryRequest
{
    /// <summary>Entity to query.</summary>
    [JsonProperty("entity")]
    public string Entity { get; set; } = string.Empty;

    /// <summary>Fields to select.</summary>
    [JsonProperty("select")]
    public List<string>? Select { get; set; }

    /// <summary>Filter conditions.</summary>
    [JsonProperty("filter")]
    public object? Filter { get; set; }

    /// <summary>Sort configuration.</summary>
    [JsonProperty("sort")]
    public List<DatasourceSortItem>? Sort { get; set; }

    /// <summary>Group by fields.</summary>
    [JsonProperty("groupBy")]
    public List<string>? GroupBy { get; set; }

    /// <summary>Aggregations to perform.</summary>
    [JsonProperty("aggregations")]
    public List<DatasourceAggregation>? Aggregations { get; set; }

    /// <summary>Number of items to skip.</summary>
    [JsonProperty("skip")]
    public int? Skip { get; set; }

    /// <summary>Number of items to take.</summary>
    [JsonProperty("take")]
    public int? Take { get; set; }
}

/// <summary>
/// Sort item for datasource queries.
/// </summary>
public class DatasourceSortItem
{
    /// <summary>Field to sort by.</summary>
    [JsonProperty("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>Sort direction (asc/desc).</summary>
    [JsonProperty("direction")]
    public string Direction { get; set; } = "asc";
}

/// <summary>
/// Aggregation definition for datasource queries.
/// </summary>
public class DatasourceAggregation
{
    /// <summary>Field to aggregate.</summary>
    [JsonProperty("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>Aggregation function (sum, avg, count, min, max).</summary>
    [JsonProperty("function")]
    public string Function { get; set; } = string.Empty;

    /// <summary>Alias for the aggregation result.</summary>
    [JsonProperty("alias")]
    public string? Alias { get; set; }
}

/// <summary>
/// Result from a datasource query.
/// </summary>
public class DatasourceQueryResult
{
    /// <summary>Query results.</summary>
    [JsonProperty("data")]
    public List<Dictionary<string, object?>> Data { get; set; } = new();

    /// <summary>Total count of matching items (before pagination).</summary>
    [JsonProperty("total")]
    public int Total { get; set; }

    /// <summary>Whether more items are available.</summary>
    [JsonProperty("hasMore")]
    public bool HasMore { get; set; }

    /// <summary>Execution time in milliseconds.</summary>
    [JsonProperty("executionTimeMs")]
    public int ExecutionTimeMs { get; set; }
}

#endregion
