using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Datasource;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for datasource and query builder operations.
/// </summary>
public interface IDatasourceClient
{
    #region Entity Discovery

    /// <summary>
    /// Gets all available entities for querying.
    /// </summary>
    /// <returns>List of entity information.</returns>
    Task<List<EntityInfo>> GetEntitiesAsync();

    /// <summary>
    /// Gets entity categories.
    /// </summary>
    /// <returns>List of category names.</returns>
    Task<List<string>> GetCategoriesAsync();

    /// <summary>
    /// Gets entities filtered by category.
    /// </summary>
    /// <param name="category">Category name.</param>
    /// <returns>List of entities in the category.</returns>
    Task<List<EntityInfo>> GetEntitiesByCategoryAsync(string category);

    #endregion

    #region Schema

    /// <summary>
    /// Gets the full schema for an entity including all fields and relations.
    /// </summary>
    /// <param name="entityName">Entity name.</param>
    /// <returns>Entity schema.</returns>
    Task<EntitySchema> GetEntitySchemaAsync(string entityName);

    /// <summary>
    /// Gets field information for a specific field.
    /// </summary>
    /// <param name="entityName">Entity name.</param>
    /// <param name="fieldName">Field name.</param>
    /// <returns>Field information.</returns>
    Task<EntityField> GetFieldAsync(string entityName, string fieldName);

    /// <summary>
    /// Gets available filter operators for a data type.
    /// </summary>
    /// <param name="dataType">Data type name.</param>
    /// <returns>List of available operators.</returns>
    Task<List<string>> GetOperatorsForTypeAsync(string dataType);

    /// <summary>
    /// Gets available aggregation functions for a data type.
    /// </summary>
    /// <param name="dataType">Data type name.</param>
    /// <returns>List of available aggregation functions.</returns>
    Task<List<string>> GetAggregationsForTypeAsync(string dataType);

    #endregion

    #region Query Execution

    /// <summary>
    /// Executes a query and returns results.
    /// </summary>
    /// <param name="query">Query to execute.</param>
    /// <param name="includeTotalCount">Whether to include total count (may be slow).</param>
    /// <returns>Query result with data and metadata.</returns>
    Task<QueryResult> ExecuteQueryAsync(DatasourceQuery query, bool includeTotalCount = true);

    /// <summary>
    /// Previews a query with limited results.
    /// </summary>
    /// <param name="query">Query to preview.</param>
    /// <param name="maxRows">Maximum rows to return.</param>
    /// <returns>Query result with limited data.</returns>
    Task<QueryResult> PreviewQueryAsync(DatasourceQuery query, int maxRows = 100);

    /// <summary>
    /// Validates a query configuration without executing.
    /// </summary>
    /// <param name="query">Query to validate.</param>
    /// <returns>Validation result.</returns>
    Task<QueryValidationResult> ValidateQueryAsync(DatasourceQuery query);

    #endregion

    #region Query Builder Helpers

    /// <summary>
    /// Creates an empty query for an entity.
    /// </summary>
    /// <param name="entityName">Entity name.</param>
    /// <returns>Empty query.</returns>
    DatasourceQuery CreateQuery(string entityName);

    /// <summary>
    /// Adds a field to the query.
    /// </summary>
    /// <param name="query">Query to modify.</param>
    /// <param name="fieldName">Field name to add.</param>
    /// <param name="alias">Optional alias.</param>
    /// <returns>Modified query.</returns>
    DatasourceQuery AddField(DatasourceQuery query, string fieldName, string? alias = null);

    /// <summary>
    /// Adds a filter to the query.
    /// </summary>
    /// <param name="query">Query to modify.</param>
    /// <param name="field">Field name.</param>
    /// <param name="op">Filter operator.</param>
    /// <param name="value">Filter value.</param>
    /// <param name="logicalOperator">Logical operator (and/or).</param>
    /// <returns>Modified query.</returns>
    DatasourceQuery AddFilter(DatasourceQuery query, string field, string op, object? value, string? logicalOperator = null);

    /// <summary>
    /// Adds sorting to the query.
    /// </summary>
    /// <param name="query">Query to modify.</param>
    /// <param name="field">Field name.</param>
    /// <param name="direction">Sort direction (asc/desc).</param>
    /// <returns>Modified query.</returns>
    DatasourceQuery AddSort(DatasourceQuery query, string field, string direction = "asc");

    /// <summary>
    /// Adds grouping to the query.
    /// </summary>
    /// <param name="query">Query to modify.</param>
    /// <param name="field">Field name to group by.</param>
    /// <param name="datePart">Date part for date fields.</param>
    /// <returns>Modified query.</returns>
    DatasourceQuery AddGroupBy(DatasourceQuery query, string field, string? datePart = null);

    /// <summary>
    /// Adds an aggregation to the query.
    /// </summary>
    /// <param name="query">Query to modify.</param>
    /// <param name="field">Field name.</param>
    /// <param name="function">Aggregation function.</param>
    /// <param name="alias">Optional alias.</param>
    /// <returns>Modified query.</returns>
    DatasourceQuery AddAggregation(DatasourceQuery query, string field, string function, string? alias = null);

    /// <summary>
    /// Sets pagination on the query.
    /// </summary>
    /// <param name="query">Query to modify.</param>
    /// <param name="limit">Maximum rows.</param>
    /// <param name="offset">Rows to skip.</param>
    /// <returns>Modified query.</returns>
    DatasourceQuery SetPagination(DatasourceQuery query, int limit, int offset = 0);

    #endregion
}
