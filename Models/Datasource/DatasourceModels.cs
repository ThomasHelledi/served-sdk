using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.Datasource;

#region Entity Discovery

/// <summary>
/// Information about a queryable entity.
/// </summary>
public class EntityInfo
{
    /// <summary>Entity name used in queries.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Category for grouping (e.g., "Projects", "Finance", "CRM").</summary>
    public string? Category { get; set; }

    /// <summary>Entity description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the entity supports filtering.</summary>
    public bool IsFilterable { get; set; } = true;

    /// <summary>Whether the entity supports grouping.</summary>
    public bool IsGroupable { get; set; } = true;
}

/// <summary>
/// Full schema information for an entity.
/// </summary>
public class EntitySchema
{
    /// <summary>Entity name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Category for grouping.</summary>
    public string? Category { get; set; }

    /// <summary>All fields available on this entity.</summary>
    public List<EntityField> Fields { get; set; } = new();

    /// <summary>Relations to other entities.</summary>
    public List<EntityRelation> Relations { get; set; } = new();
}

/// <summary>
/// Information about an entity field.
/// </summary>
public class EntityField
{
    /// <summary>Field name used in queries.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Data type (String, Int, Decimal, DateTime, Boolean, etc.).</summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>Whether this field can be used in filters.</summary>
    public bool IsFilterable { get; set; }

    /// <summary>Whether this field can be used for sorting.</summary>
    public bool IsSortable { get; set; }

    /// <summary>Whether this field can be used for grouping.</summary>
    public bool IsGroupable { get; set; }

    /// <summary>Whether aggregations can be applied to this field.</summary>
    public bool IsAggregatable { get; set; }

    /// <summary>Whether this is a custom field.</summary>
    public bool IsCustomField { get; set; }

    /// <summary>Available filter operators for this field's data type.</summary>
    public List<string>? Operators { get; set; }
}

/// <summary>
/// Relation to another entity.
/// </summary>
public class EntityRelation
{
    /// <summary>Relation name used in queries.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Target entity name.</summary>
    public string TargetEntity { get; set; } = string.Empty;

    /// <summary>Relation type (OneToMany, ManyToOne, ManyToMany).</summary>
    public string RelationType { get; set; } = string.Empty;

    /// <summary>Foreign key field name.</summary>
    public string? ForeignKey { get; set; }
}

#endregion

#region Query Models

/// <summary>
/// A complete query definition for the datasource API.
/// </summary>
public class DatasourceQuery
{
    /// <summary>Entity name to query.</summary>
    [JsonProperty("entity")]
    public string Entity { get; set; } = string.Empty;

    /// <summary>Fields to select (empty = all fields).</summary>
    [JsonProperty("fields")]
    public List<QueryFieldSelection>? Fields { get; set; }

    /// <summary>Filter conditions.</summary>
    [JsonProperty("filters")]
    public List<QueryFilter>? Filters { get; set; }

    /// <summary>Sort specifications.</summary>
    [JsonProperty("sorting")]
    public List<QuerySort>? Sorting { get; set; }

    /// <summary>Group by fields.</summary>
    [JsonProperty("groupBy")]
    public List<QueryGroupBy>? GroupBy { get; set; }

    /// <summary>Aggregation functions.</summary>
    [JsonProperty("aggregations")]
    public List<QueryAggregation>? Aggregations { get; set; }

    /// <summary>Entity joins.</summary>
    [JsonProperty("joins")]
    public List<QueryJoin>? Joins { get; set; }

    /// <summary>Maximum rows to return.</summary>
    [JsonProperty("limit")]
    public int? Limit { get; set; }

    /// <summary>Number of rows to skip.</summary>
    [JsonProperty("offset")]
    public int? Offset { get; set; }
}

/// <summary>
/// Field selection with optional alias.
/// </summary>
public class QueryFieldSelection
{
    /// <summary>Field name.</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional alias for the result.</summary>
    [JsonProperty("alias")]
    public string? Alias { get; set; }

    /// <summary>Entity path for joined fields (e.g., "Customer.Name").</summary>
    [JsonProperty("entityPath")]
    public string? EntityPath { get; set; }
}

/// <summary>
/// Filter condition.
/// </summary>
public class QueryFilter
{
    /// <summary>Field name to filter on.</summary>
    [JsonProperty("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>Filter operator (eq, ne, gt, gte, lt, lte, contains, startswith, endswith, isnull, isnotnull, in, between).</summary>
    [JsonProperty("operator")]
    public string Operator { get; set; } = "eq";

    /// <summary>Filter value (type depends on field and operator).</summary>
    [JsonProperty("value")]
    public object? Value { get; set; }

    /// <summary>Logical operator to combine with previous filter (and, or).</summary>
    [JsonProperty("logicalOperator")]
    public string? LogicalOperator { get; set; }

    /// <summary>Sub-filters for complex conditions.</summary>
    [JsonProperty("subFilters")]
    public List<QueryFilter>? SubFilters { get; set; }
}

/// <summary>
/// Sort specification.
/// </summary>
public class QuerySort
{
    /// <summary>Field name to sort by.</summary>
    [JsonProperty("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>Sort direction (asc, desc).</summary>
    [JsonProperty("direction")]
    public string Direction { get; set; } = "asc";
}

/// <summary>
/// Group by specification.
/// </summary>
public class QueryGroupBy
{
    /// <summary>Field name to group by.</summary>
    [JsonProperty("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>Date part for date fields (day, week, month, quarter, year).</summary>
    [JsonProperty("datePart")]
    public string? DatePart { get; set; }
}

/// <summary>
/// Aggregation function specification.
/// </summary>
public class QueryAggregation
{
    /// <summary>Field name to aggregate.</summary>
    [JsonProperty("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>Aggregation function (count, sum, avg, min, max, distinct_count).</summary>
    [JsonProperty("function")]
    public string Function { get; set; } = "count";

    /// <summary>Alias for the aggregated result.</summary>
    [JsonProperty("alias")]
    public string? Alias { get; set; }
}

/// <summary>
/// Entity join specification.
/// </summary>
public class QueryJoin
{
    /// <summary>Entity to join.</summary>
    [JsonProperty("entity")]
    public string Entity { get; set; } = string.Empty;

    /// <summary>Join type (inner, left, right).</summary>
    [JsonProperty("joinType")]
    public string JoinType { get; set; } = "inner";

    /// <summary>Join condition field from source entity.</summary>
    [JsonProperty("sourceField")]
    public string SourceField { get; set; } = string.Empty;

    /// <summary>Join condition field from target entity.</summary>
    [JsonProperty("targetField")]
    public string TargetField { get; set; } = string.Empty;

    /// <summary>Alias for the joined entity.</summary>
    [JsonProperty("alias")]
    public string? Alias { get; set; }
}

#endregion

#region Query Results

/// <summary>
/// Result from executing a datasource query.
/// </summary>
public class QueryResult
{
    /// <summary>Query result data as a list of dynamic objects.</summary>
    [JsonProperty("data")]
    public List<Dictionary<string, object?>> Data { get; set; } = new();

    /// <summary>Result metadata.</summary>
    [JsonProperty("meta")]
    public QueryResultMeta? Meta { get; set; }
}

/// <summary>
/// Metadata about the query result.
/// </summary>
public class QueryResultMeta
{
    /// <summary>Total count of matching rows (before pagination).</summary>
    public int TotalCount { get; set; }

    /// <summary>Number of rows returned in this response.</summary>
    public int ReturnedCount { get; set; }

    /// <summary>Query execution time in milliseconds.</summary>
    public int ExecutionTimeMs { get; set; }

    /// <summary>Column metadata for the result set.</summary>
    public List<QueryResultColumn>? Columns { get; set; }
}

/// <summary>
/// Column metadata for query results.
/// </summary>
public class QueryResultColumn
{
    /// <summary>Column name as returned in data.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Human-readable display name.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Data type of the column.</summary>
    public string DataType { get; set; } = string.Empty;
}

/// <summary>
/// Result from query validation.
/// </summary>
public class QueryValidationResult
{
    /// <summary>Whether the query is valid.</summary>
    public bool IsValid { get; set; }

    /// <summary>List of validation errors.</summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>List of validation warnings.</summary>
    public List<string> Warnings { get; set; } = new();
}

#endregion

#region API Requests

/// <summary>
/// Request to execute a datasource query.
/// </summary>
public class ExecuteQueryRequest
{
    /// <summary>The query to execute.</summary>
    [JsonProperty("query")]
    public DatasourceQuery Query { get; set; } = new();

    /// <summary>Whether to include total count (can be slow for large datasets).</summary>
    [JsonProperty("includeTotalCount")]
    public bool IncludeTotalCount { get; set; } = true;
}

/// <summary>
/// Request to preview a query with limited results.
/// </summary>
public class PreviewQueryRequest
{
    /// <summary>The query to preview.</summary>
    [JsonProperty("query")]
    public DatasourceQuery Query { get; set; } = new();

    /// <summary>Maximum rows to return in preview.</summary>
    [JsonProperty("maxRows")]
    public int MaxRows { get; set; } = 100;
}

/// <summary>
/// Request to validate a query.
/// </summary>
public class ValidateQueryRequest
{
    /// <summary>The query to validate.</summary>
    [JsonProperty("query")]
    public DatasourceQuery Query { get; set; } = new();
}

#endregion

#region API Response Wrappers

/// <summary>
/// Standard list response wrapper for datasource API.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class DatasourceListResponse<T>
{
    /// <summary>List of items.</summary>
    [JsonProperty("data")]
    public List<T> Data { get; set; } = new();
}

#endregion
