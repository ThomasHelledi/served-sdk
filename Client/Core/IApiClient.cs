using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Core;

/// <summary>
/// Generic interface for standard CRUD API operations.
/// </summary>
/// <typeparam name="TEntity">The entity/summary type returned from list operations.</typeparam>
/// <typeparam name="TDetail">The detailed entity type returned from get/create/update operations.</typeparam>
/// <typeparam name="TCreate">The request type for creating entities.</typeparam>
/// <typeparam name="TUpdate">The request type for updating entities.</typeparam>
/// <typeparam name="TQuery">The query parameters type for filtering.</typeparam>
public interface IApiClient<TEntity, TDetail, TCreate, TUpdate, TQuery>
    where TEntity : class
    where TDetail : class
    where TCreate : class
    where TUpdate : class
    where TQuery : QueryParams, new()
{
    /// <summary>
    /// Gets all entities with optional filtering.
    /// </summary>
    Task<List<TEntity>> GetAllAsync(TQuery? query = null);

    /// <summary>
    /// Gets a single entity by ID.
    /// </summary>
    Task<TDetail> GetAsync(int id);

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    Task<TDetail> CreateAsync(TCreate request);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task<TDetail> UpdateAsync(int id, TUpdate request);

    /// <summary>
    /// Deletes an entity by ID.
    /// </summary>
    Task DeleteAsync(int id);
}

/// <summary>
/// Extended interface with bulk operations support.
/// </summary>
public interface IBulkApiClient<TEntity, TDetail, TCreate, TUpdate, TQuery, TBulkCreate, TBulkUpdate, TBulkDelete>
    : IApiClient<TEntity, TDetail, TCreate, TUpdate, TQuery>
    where TEntity : class
    where TDetail : class
    where TCreate : class
    where TUpdate : class
    where TQuery : QueryParams, new()
    where TBulkCreate : class
    where TBulkUpdate : class
    where TBulkDelete : class
{
    /// <summary>
    /// Creates multiple entities in a single operation.
    /// </summary>
    Task<BulkResponse<TDetail>> CreateBulkAsync(TBulkCreate request);

    /// <summary>
    /// Updates multiple entities in a single operation.
    /// </summary>
    Task<BulkResponse<TDetail>> UpdateBulkAsync(TBulkUpdate request);

    /// <summary>
    /// Deletes multiple entities in a single operation.
    /// </summary>
    Task<BulkResponse<TDetail>> DeleteBulkAsync(TBulkDelete request);
}

/// <summary>
/// Simplified interface for read-only API access.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TQuery">The query parameters type.</typeparam>
public interface IReadOnlyApiClient<TEntity, TQuery>
    where TEntity : class
    where TQuery : QueryParams, new()
{
    /// <summary>
    /// Gets all entities with optional filtering.
    /// </summary>
    Task<List<TEntity>> GetAllAsync(TQuery? query = null);

    /// <summary>
    /// Gets a single entity by ID.
    /// </summary>
    Task<TEntity> GetAsync(int id);
}
