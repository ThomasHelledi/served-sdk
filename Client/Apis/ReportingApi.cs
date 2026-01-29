using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Dashboards;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for reporting resources including dashboards and datasources.
/// </summary>
public class ReportingApi : ApiModuleBase
{
    protected override string ModulePath => "reporting";

    /// <summary>
    /// Access to dashboard resources.
    /// </summary>
    public DashboardsResource Dashboards { get; }

    /// <summary>
    /// Access to datasource resources.
    /// </summary>
    public DatasourcesResource Datasources { get; }

    public ReportingApi(IHttpClient http) : base(http)
    {
        Dashboards = new DashboardsResource(http, this);
        Datasources = new DatasourcesResource(http, this);
    }

    #region Dashboards Resource

    /// <summary>
    /// Resource client for dashboard operations.
    /// </summary>
    public class DashboardsResource
    {
        private readonly IHttpClient _http;
        private readonly ReportingApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/dashboards";

        internal DashboardsResource(IHttpClient http, ReportingApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all dashboards.
        /// </summary>
        public async Task<List<DashboardViewModel>> GetAllAsync(int? workspaceId = null)
        {
            var query = workspaceId.HasValue ? $"?workspaceId={workspaceId}" : "";
            var response = await _http.GetAsync<ApiV2ListResponse<DashboardViewModel>>($"{BasePath}{query}");
            return response.Data ?? new List<DashboardViewModel>();
        }

        /// <summary>
        /// Gets a dashboard by ID.
        /// </summary>
        public Task<DashboardViewModel> GetAsync(int id)
        {
            return _http.GetAsync<DashboardViewModel>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Creates a new dashboard.
        /// </summary>
        public Task<DashboardViewModel> CreateAsync(CreateDashboardRequest request)
        {
            return _http.PostAsync<DashboardViewModel>(BasePath, request);
        }

        /// <summary>
        /// Updates a dashboard.
        /// </summary>
        public Task<DashboardViewModel> UpdateAsync(int id, UpdateDashboardRequest request)
        {
            return _http.PutAsync<DashboardViewModel>($"{BasePath}/{id}", request);
        }

        /// <summary>
        /// Deletes a dashboard.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.DeleteAsync($"{BasePath}/{id}");
        }
    }

    #endregion

    #region Datasources Resource

    /// <summary>
    /// Resource client for datasource operations.
    /// </summary>
    public class DatasourcesResource
    {
        private readonly IHttpClient _http;
        private readonly ReportingApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/datasources";

        internal DatasourcesResource(IHttpClient http, ReportingApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all available entities for querying.
        /// </summary>
        public async Task<List<EntityViewModel>> GetEntitiesAsync()
        {
            var response = await _http.GetAsync<ApiV2ListResponse<EntityViewModel>>($"{BasePath}/entities");
            return response.Data ?? new List<EntityViewModel>();
        }

        /// <summary>
        /// Gets entity metadata.
        /// </summary>
        public Task<EntityMetadataViewModel> GetEntityMetadataAsync(string entityName)
        {
            return _http.GetAsync<EntityMetadataViewModel>($"{BasePath}/entities/{entityName}");
        }

        /// <summary>
        /// Executes a dynamic query.
        /// </summary>
        public Task<DatasourceQueryResult> ExecuteQueryAsync(DatasourceQueryRequest request)
        {
            return _http.PostAsync<DatasourceQueryResult>($"{BasePath}/query", request);
        }
    }

    #endregion
}
