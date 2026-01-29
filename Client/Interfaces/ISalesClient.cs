using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Sales;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for Sales CRM operations (pipelines, deals).
/// </summary>
public interface ISalesClient
{
    #region Pipelines

    /// <summary>
    /// Gets all pipelines for a workspace.
    /// </summary>
    Task<List<PipelineListViewModel>> GetPipelinesAsync(int workspaceId);

    /// <summary>
    /// Gets a pipeline by ID.
    /// </summary>
    Task<PipelineViewModel?> GetPipelineAsync(int id);

    /// <summary>
    /// Creates a new pipeline.
    /// </summary>
    Task<PipelineViewModel> CreatePipelineAsync(CreatePipelineRequest request);

    /// <summary>
    /// Updates a pipeline.
    /// </summary>
    Task<PipelineViewModel> UpdatePipelineAsync(int id, UpdatePipelineRequest request);

    /// <summary>
    /// Deletes a pipeline.
    /// </summary>
    Task DeletePipelineAsync(int id);

    #endregion

    #region Deals

    /// <summary>
    /// Searches for deals.
    /// </summary>
    Task<List<DealListViewModel>> SearchDealsAsync(DealSearchRequest request);

    /// <summary>
    /// Gets a deal by ID.
    /// </summary>
    Task<DealViewModel?> GetDealAsync(int id);

    /// <summary>
    /// Creates a new deal.
    /// </summary>
    Task<DealViewModel> CreateDealAsync(CreateDealRequest request);

    /// <summary>
    /// Updates a deal.
    /// </summary>
    Task<DealViewModel> UpdateDealAsync(int id, UpdateDealRequest request);

    /// <summary>
    /// Deletes a deal.
    /// </summary>
    Task DeleteDealAsync(int id);

    /// <summary>
    /// Moves a deal to a different stage.
    /// </summary>
    Task<DealViewModel> MoveDealAsync(int id, int stageId);

    /// <summary>
    /// Marks a deal as won.
    /// </summary>
    Task<DealViewModel> MarkWonAsync(int id, WonDealRequest request);

    /// <summary>
    /// Marks a deal as lost.
    /// </summary>
    Task<DealViewModel> MarkLostAsync(int id, LostDealRequest request);

    /// <summary>
    /// Reopens a closed deal.
    /// </summary>
    Task<DealViewModel> ReopenDealAsync(int id);

    #endregion

    #region Analytics

    /// <summary>
    /// Gets pipeline analytics.
    /// </summary>
    Task<PipelineAnalyticsViewModel> GetPipelineAnalyticsAsync(int pipelineId, DateTime? fromDate = null, DateTime? toDate = null);

    /// <summary>
    /// Gets sales forecast.
    /// </summary>
    Task<SalesForecastViewModel> GetForecastAsync(int pipelineId, string period = "month");

    #endregion
}
