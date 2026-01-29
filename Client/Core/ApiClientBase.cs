using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Core;

/// <summary>
/// Abstract base class for API clients providing standard CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The entity/summary type returned from list operations.</typeparam>
/// <typeparam name="TDetail">The detailed entity type returned from get/create/update operations.</typeparam>
/// <typeparam name="TCreate">The request type for creating entities.</typeparam>
/// <typeparam name="TUpdate">The request type for updating entities.</typeparam>
/// <typeparam name="TQuery">The query parameters type for filtering.</typeparam>
public abstract class ApiClientBase<TEntity, TDetail, TCreate, TUpdate, TQuery>
    : IApiClient<TEntity, TDetail, TCreate, TUpdate, TQuery>
    where TEntity : class
    where TDetail : class
    where TCreate : class
    where TUpdate : class
    where TQuery : QueryParams, new()
{
    protected readonly IHttpClient Http;

    /// <summary>
    /// The base API path for this resource (e.g., "api/v1/project-management/projects").
    /// </summary>
    protected abstract string BasePath { get; }

    /// <summary>
    /// Initializes a new instance of the API client.
    /// </summary>
    protected ApiClientBase(IHttpClient http)
    {
        Http = http;
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetAllAsync(TQuery? query = null)
    {
        var q = query ?? new TQuery();
        var queryString = BuildQueryString(q);
        var response = await Http.GetAsync<ApiV2ListResponse<TDetail>>($"{BasePath}{queryString}");
        return MapToEntityList(response.Data) ?? new List<TEntity>();
    }

    /// <inheritdoc/>
    public virtual Task<TDetail> GetAsync(int id)
    {
        return Http.GetAsync<TDetail>($"{BasePath}/{id}");
    }

    /// <inheritdoc/>
    public virtual Task<TDetail> CreateAsync(TCreate request)
    {
        return Http.PostAsync<TDetail>(BasePath, request);
    }

    /// <inheritdoc/>
    public virtual Task<TDetail> UpdateAsync(int id, TUpdate request)
    {
        return Http.PutAsync<TDetail>($"{BasePath}/{id}", request);
    }

    /// <inheritdoc/>
    public virtual Task DeleteAsync(int id)
    {
        return Http.DeleteAsync($"{BasePath}/{id}");
    }

    /// <summary>
    /// Builds a query string from the query parameters.
    /// Override to add custom query parameters.
    /// </summary>
    protected virtual string BuildQueryString(TQuery query)
    {
        var queryParams = new List<string>();

        // Standard pagination
        var page = query.Skip > 0 && query.Take.HasValue && query.Take.Value > 0
            ? (query.Skip / query.Take.Value) + 1
            : 1;
        var pageSize = query.Take ?? 50;
        queryParams.Add($"page={page}");
        queryParams.Add($"pageSize={pageSize}");

        // Search
        if (!string.IsNullOrEmpty(query.Search))
            queryParams.Add($"search={HttpUtility.UrlEncode(query.Search)}");

        // Sort
        if (!string.IsNullOrEmpty(query.Sort))
            queryParams.Add($"sort={HttpUtility.UrlEncode(query.Sort)}");

        // Add custom query params from derived classes
        var customParams = GetCustomQueryParams(query);
        queryParams.AddRange(customParams);

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
    }

    /// <summary>
    /// Override to add custom query parameters specific to the resource.
    /// </summary>
    protected virtual IEnumerable<string> GetCustomQueryParams(TQuery query)
    {
        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Maps a list of detail entities to summary entities.
    /// Override if TEntity != TDetail.
    /// </summary>
    protected virtual List<TEntity>? MapToEntityList(List<TDetail>? details)
    {
        // Default implementation assumes TEntity == TDetail
        return details as List<TEntity>;
    }
}

/// <summary>
/// Extended base class with bulk operations support.
/// </summary>
public abstract class BulkApiClientBase<TEntity, TDetail, TCreate, TUpdate, TQuery, TBulkCreate, TBulkUpdate, TBulkDelete>
    : ApiClientBase<TEntity, TDetail, TCreate, TUpdate, TQuery>,
      IBulkApiClient<TEntity, TDetail, TCreate, TUpdate, TQuery, TBulkCreate, TBulkUpdate, TBulkDelete>
    where TEntity : class
    where TDetail : class
    where TCreate : class
    where TUpdate : class
    where TQuery : QueryParams, new()
    where TBulkCreate : class
    where TBulkUpdate : class
    where TBulkDelete : class
{
    protected BulkApiClientBase(IHttpClient http) : base(http) { }

    /// <inheritdoc/>
    public virtual Task<BulkResponse<TDetail>> CreateBulkAsync(TBulkCreate request)
    {
        return Http.PostAsync<BulkResponse<TDetail>>($"{BasePath}/bulk", request);
    }

    /// <inheritdoc/>
    public virtual Task<BulkResponse<TDetail>> UpdateBulkAsync(TBulkUpdate request)
    {
        return Http.PutAsync<BulkResponse<TDetail>>($"{BasePath}/bulk", request);
    }

    /// <inheritdoc/>
    public virtual Task<BulkResponse<TDetail>> DeleteBulkAsync(TBulkDelete request)
    {
        return Http.DeleteWithBodyAsync<BulkResponse<TDetail>>($"{BasePath}/bulk", request);
    }
}

/// <summary>
/// Base class for read-only API clients.
/// </summary>
public abstract class ReadOnlyApiClientBase<TEntity, TQuery> : IReadOnlyApiClient<TEntity, TQuery>
    where TEntity : class
    where TQuery : QueryParams, new()
{
    protected readonly IHttpClient Http;

    /// <summary>
    /// The base API path for this resource.
    /// </summary>
    protected abstract string BasePath { get; }

    protected ReadOnlyApiClientBase(IHttpClient http)
    {
        Http = http;
    }

    /// <inheritdoc/>
    public virtual async Task<List<TEntity>> GetAllAsync(TQuery? query = null)
    {
        var q = query ?? new TQuery();
        var queryString = BuildQueryString(q);
        var response = await Http.GetAsync<ApiV2ListResponse<TEntity>>($"{BasePath}{queryString}");
        return response.Data ?? new List<TEntity>();
    }

    /// <inheritdoc/>
    public virtual Task<TEntity> GetAsync(int id)
    {
        return Http.GetAsync<TEntity>($"{BasePath}/{id}");
    }

    /// <summary>
    /// Builds a query string from the query parameters.
    /// </summary>
    protected virtual string BuildQueryString(TQuery query)
    {
        var queryParams = new List<string>();

        var page = query.Skip > 0 && query.Take.HasValue && query.Take.Value > 0
            ? (query.Skip / query.Take.Value) + 1
            : 1;
        var pageSize = query.Take ?? 50;
        queryParams.Add($"page={page}");
        queryParams.Add($"pageSize={pageSize}");

        if (!string.IsNullOrEmpty(query.Search))
            queryParams.Add($"search={HttpUtility.UrlEncode(query.Search)}");

        var customParams = GetCustomQueryParams(query);
        queryParams.AddRange(customParams);

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
    }

    protected virtual IEnumerable<string> GetCustomQueryParams(TQuery query)
    {
        return Enumerable.Empty<string>();
    }
}
