using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Sales;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for sales CRM resources including pipelines and deals.
/// </summary>
public class SalesApi : ApiModuleBase
{
    protected override string ModulePath => "sales";

    /// <summary>
    /// Access to pipeline resources.
    /// </summary>
    public PipelinesResource Pipelines { get; }

    /// <summary>
    /// Access to deal resources.
    /// </summary>
    public DealsResource Deals { get; }

    public SalesApi(IHttpClient http) : base(http)
    {
        Pipelines = new PipelinesResource(http, this);
        Deals = new DealsResource(http, this);
    }

    #region Pipelines Resource

    /// <summary>
    /// Resource client for sales pipeline operations.
    /// </summary>
    public class PipelinesResource
    {
        private readonly IHttpClient _http;
        private readonly SalesApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/pipelines";

        internal PipelinesResource(IHttpClient http, SalesApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all pipelines for a workspace.
        /// </summary>
        public async Task<List<PipelineListViewModel>> GetAllAsync(int workspaceId)
        {
            var response = await _http.PostAsync<ApiListResponse<PipelineListViewModel>>(
                $"{BasePath}/list",
                new { workspaceId });
            return response.Data ?? new List<PipelineListViewModel>();
        }

        /// <summary>
        /// Gets a pipeline by ID.
        /// </summary>
        public Task<PipelineViewModel> GetAsync(int id)
        {
            return _http.PostAsync<PipelineViewModel>(
                $"{BasePath}/get",
                new { id });
        }

        /// <summary>
        /// Creates a new pipeline.
        /// </summary>
        public Task<PipelineViewModel> CreateAsync(CreatePipelineRequest request)
        {
            return _http.PostAsync<PipelineViewModel>($"{BasePath}/create", request);
        }

        /// <summary>
        /// Updates a pipeline.
        /// </summary>
        public Task<PipelineViewModel> UpdateAsync(UpdatePipelineRequest request)
        {
            return _http.PostAsync<PipelineViewModel>($"{BasePath}/update", request);
        }

        /// <summary>
        /// Deletes a pipeline.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.PostAsync($"{BasePath}/delete", new { id });
        }
    }

    #endregion

    #region Deals Resource

    /// <summary>
    /// Resource client for sales deal operations.
    /// </summary>
    public class DealsResource
    {
        private readonly IHttpClient _http;
        private readonly SalesApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/deals";

        internal DealsResource(IHttpClient http, SalesApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all deals for a pipeline.
        /// </summary>
        public async Task<List<DealViewModel>> GetByPipelineAsync(int pipelineId)
        {
            var response = await _http.PostAsync<ApiListResponse<DealViewModel>>(
                $"{BasePath}/list",
                new { pipelineId });
            return response.Data ?? new List<DealViewModel>();
        }

        /// <summary>
        /// Gets a deal by ID.
        /// </summary>
        public Task<DealViewModel> GetAsync(int id)
        {
            return _http.PostAsync<DealViewModel>(
                $"{BasePath}/get",
                new { id });
        }

        /// <summary>
        /// Creates a new deal.
        /// </summary>
        public Task<DealViewModel> CreateAsync(CreateDealRequest request)
        {
            return _http.PostAsync<DealViewModel>($"{BasePath}/create", request);
        }

        /// <summary>
        /// Updates a deal.
        /// </summary>
        public Task<DealViewModel> UpdateAsync(UpdateDealRequest request)
        {
            return _http.PostAsync<DealViewModel>($"{BasePath}/update", request);
        }

        /// <summary>
        /// Deletes a deal.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.PostAsync($"{BasePath}/delete", new { id });
        }

        /// <summary>
        /// Moves a deal to a different stage.
        /// </summary>
        public Task<DealViewModel> MoveToStageAsync(int dealId, int stageId)
        {
            return _http.PostAsync<DealViewModel>(
                $"{BasePath}/move",
                new { dealId, stageId });
        }
    }

    #endregion
}
