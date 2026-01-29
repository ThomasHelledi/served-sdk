using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Datasource;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for datasource and query builder operations.
/// </summary>
public class DatasourceClient : IDatasourceClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasourceClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public DatasourceClient(IServedClient client)
    {
        _client = client;
    }

    #region Entity Discovery

    /// <inheritdoc/>
    public async Task<List<EntityInfo>> GetEntitiesAsync()
    {
        var response = await _client.GetAsync<DatasourceListResponse<EntityInfo>>("api/datasource/entities");
        return response.Data;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetCategoriesAsync()
    {
        var response = await _client.GetAsync<DatasourceListResponse<string>>("api/datasource/categories");
        return response.Data;
    }

    /// <inheritdoc/>
    public async Task<List<EntityInfo>> GetEntitiesByCategoryAsync(string category)
    {
        var response = await _client.GetAsync<DatasourceListResponse<EntityInfo>>($"api/datasource/entities/category/{category}");
        return response.Data;
    }

    #endregion

    #region Schema

    /// <inheritdoc/>
    public Task<EntitySchema> GetEntitySchemaAsync(string entityName)
    {
        return _client.GetAsync<EntitySchema>($"api/datasource/schema/{entityName}");
    }

    /// <inheritdoc/>
    public Task<EntityField> GetFieldAsync(string entityName, string fieldName)
    {
        return _client.GetAsync<EntityField>($"api/datasource/schema/{entityName}/field/{fieldName}");
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetOperatorsForTypeAsync(string dataType)
    {
        var response = await _client.GetAsync<DatasourceListResponse<string>>($"api/datasource/operators/{dataType}");
        return response.Data;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetAggregationsForTypeAsync(string dataType)
    {
        var response = await _client.GetAsync<DatasourceListResponse<string>>($"api/datasource/aggregations/{dataType}");
        return response.Data;
    }

    #endregion

    #region Query Execution

    /// <inheritdoc/>
    public Task<QueryResult> ExecuteQueryAsync(DatasourceQuery query, bool includeTotalCount = true)
    {
        var request = new ExecuteQueryRequest
        {
            Query = query,
            IncludeTotalCount = includeTotalCount
        };
        return _client.PostAsync<QueryResult>("api/datasource/query", request);
    }

    /// <inheritdoc/>
    public Task<QueryResult> PreviewQueryAsync(DatasourceQuery query, int maxRows = 100)
    {
        var request = new PreviewQueryRequest
        {
            Query = query,
            MaxRows = maxRows
        };
        return _client.PostAsync<QueryResult>("api/datasource/query/preview", request);
    }

    /// <inheritdoc/>
    public Task<QueryValidationResult> ValidateQueryAsync(DatasourceQuery query)
    {
        var request = new ValidateQueryRequest { Query = query };
        return _client.PostAsync<QueryValidationResult>("api/datasource/query/validate", request);
    }

    #endregion

    #region Query Builder Helpers

    /// <inheritdoc/>
    public DatasourceQuery CreateQuery(string entityName)
    {
        return new DatasourceQuery
        {
            Entity = entityName,
            Fields = new List<QueryFieldSelection>(),
            Filters = new List<QueryFilter>(),
            Sorting = new List<QuerySort>(),
            GroupBy = new List<QueryGroupBy>(),
            Aggregations = new List<QueryAggregation>(),
            Joins = new List<QueryJoin>()
        };
    }

    /// <inheritdoc/>
    public DatasourceQuery AddField(DatasourceQuery query, string fieldName, string? alias = null)
    {
        query.Fields ??= new List<QueryFieldSelection>();
        query.Fields.Add(new QueryFieldSelection
        {
            Name = fieldName,
            Alias = alias
        });
        return query;
    }

    /// <inheritdoc/>
    public DatasourceQuery AddFilter(DatasourceQuery query, string field, string op, object? value, string? logicalOperator = null)
    {
        query.Filters ??= new List<QueryFilter>();
        query.Filters.Add(new QueryFilter
        {
            Field = field,
            Operator = op,
            Value = value,
            LogicalOperator = logicalOperator
        });
        return query;
    }

    /// <inheritdoc/>
    public DatasourceQuery AddSort(DatasourceQuery query, string field, string direction = "asc")
    {
        query.Sorting ??= new List<QuerySort>();
        query.Sorting.Add(new QuerySort
        {
            Field = field,
            Direction = direction
        });
        return query;
    }

    /// <inheritdoc/>
    public DatasourceQuery AddGroupBy(DatasourceQuery query, string field, string? datePart = null)
    {
        query.GroupBy ??= new List<QueryGroupBy>();
        query.GroupBy.Add(new QueryGroupBy
        {
            Field = field,
            DatePart = datePart
        });
        return query;
    }

    /// <inheritdoc/>
    public DatasourceQuery AddAggregation(DatasourceQuery query, string field, string function, string? alias = null)
    {
        query.Aggregations ??= new List<QueryAggregation>();
        query.Aggregations.Add(new QueryAggregation
        {
            Field = field,
            Function = function,
            Alias = alias
        });
        return query;
    }

    /// <inheritdoc/>
    public DatasourceQuery SetPagination(DatasourceQuery query, int limit, int offset = 0)
    {
        query.Limit = limit;
        query.Offset = offset;
        return query;
    }

    #endregion
}
