using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Common;
using Served.SDK.Models.Sales;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for Sales CRM operations.
/// </summary>
public class SalesClient : ISalesClient
{
    private readonly IServedClient _client;

    public SalesClient(IServedClient client)
    {
        _client = client;
    }

    #region Pipelines

    /// <inheritdoc/>
    public async Task<List<PipelineListViewModel>> GetPipelinesAsync(int workspaceId)
    {
        var response = await _client.PostAsync<ApiListResponse<PipelineListViewModel>>(
            "api/sales/Pipeline/GetPipelines", new { workspaceId });
        return response.Data ?? new List<PipelineListViewModel>();
    }

    /// <inheritdoc/>
    public Task<PipelineViewModel?> GetPipelineAsync(int id)
    {
        return _client.PostAsync<PipelineViewModel?>("api/sales/Pipeline/GetPipeline", new { id });
    }

    /// <inheritdoc/>
    public Task<PipelineViewModel> CreatePipelineAsync(CreatePipelineRequest request)
    {
        return _client.PostAsync<PipelineViewModel>("api/sales/Pipeline/Create", request);
    }

    /// <inheritdoc/>
    public Task<PipelineViewModel> UpdatePipelineAsync(int id, UpdatePipelineRequest request)
    {
        return _client.PostAsync<PipelineViewModel>($"api/sales/Pipeline/Update?id={id}", request);
    }

    /// <inheritdoc/>
    public Task DeletePipelineAsync(int id)
    {
        return _client.DeleteAsync($"api/sales/Pipeline/Delete?id={id}");
    }

    #endregion

    #region Deals

    /// <inheritdoc/>
    public async Task<List<DealListViewModel>> SearchDealsAsync(DealSearchRequest request)
    {
        var response = await _client.PostAsync<ApiListResponse<DealListViewModel>>(
            "api/sales/Deal/SearchDeals", request);
        return response.Data ?? new List<DealListViewModel>();
    }

    /// <inheritdoc/>
    public Task<DealViewModel?> GetDealAsync(int id)
    {
        return _client.PostAsync<DealViewModel?>("api/sales/Deal/GetDeal", new { id });
    }

    /// <inheritdoc/>
    public Task<DealViewModel> CreateDealAsync(CreateDealRequest request)
    {
        return _client.PostAsync<DealViewModel>("api/sales/Deal/Create", request);
    }

    /// <inheritdoc/>
    public Task<DealViewModel> UpdateDealAsync(int id, UpdateDealRequest request)
    {
        return _client.PostAsync<DealViewModel>($"api/sales/Deal/Update?id={id}", request);
    }

    /// <inheritdoc/>
    public Task DeleteDealAsync(int id)
    {
        return _client.DeleteAsync($"api/sales/Deal/Delete?id={id}");
    }

    /// <inheritdoc/>
    public Task<DealViewModel> MoveDealAsync(int id, int stageId)
    {
        return _client.PostAsync<DealViewModel>($"api/sales/Deal/MoveDeal?id={id}", new MoveDealRequest { StageId = stageId });
    }

    /// <inheritdoc/>
    public Task<DealViewModel> MarkWonAsync(int id, WonDealRequest request)
    {
        return _client.PostAsync<DealViewModel>($"api/sales/Deal/MarkWon?id={id}", request);
    }

    /// <inheritdoc/>
    public Task<DealViewModel> MarkLostAsync(int id, LostDealRequest request)
    {
        return _client.PostAsync<DealViewModel>($"api/sales/Deal/MarkLost?id={id}", request);
    }

    /// <inheritdoc/>
    public Task<DealViewModel> ReopenDealAsync(int id)
    {
        return _client.PostAsync<DealViewModel>($"api/sales/Deal/ReopenDeal?id={id}", new { });
    }

    #endregion

    #region Analytics

    /// <inheritdoc/>
    public Task<PipelineAnalyticsViewModel> GetPipelineAnalyticsAsync(int pipelineId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return _client.PostAsync<PipelineAnalyticsViewModel>(
            $"api/sales/Deal/GetPipelineAnalytics?pipelineId={pipelineId}&fromDate={fromDate}&toDate={toDate}",
            new { });
    }

    /// <inheritdoc/>
    public Task<SalesForecastViewModel> GetForecastAsync(int pipelineId, string period = "month")
    {
        return _client.PostAsync<SalesForecastViewModel>(
            $"api/sales/Deal/GetForecast?pipelineId={pipelineId}&period={period}",
            new { });
    }

    #endregion
}
