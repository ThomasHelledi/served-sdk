using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Tenants;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for tenant/organization resources.
/// </summary>
public class TenantApi : ApiModuleBase
{
    protected override string ModulePath => "tenant";

    /// <summary>
    /// Access to tenant resources.
    /// </summary>
    public TenantsResource Tenants { get; }

    /// <summary>
    /// Access to workspace resources.
    /// </summary>
    public WorkspacesResource Workspaces { get; }

    public TenantApi(IHttpClient http) : base(http)
    {
        Tenants = new TenantsResource(http, this);
        Workspaces = new WorkspacesResource(http, this);
    }

    #region Tenants Resource

    /// <summary>
    /// Resource client for tenant operations.
    /// </summary>
    public class TenantsResource
    {
        private readonly IHttpClient _http;
        private readonly TenantApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/tenants";

        internal TenantsResource(IHttpClient http, TenantApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all tenants for the current user.
        /// </summary>
        public async Task<List<TenantDetail>> GetAllAsync()
        {
            var response = await _http.GetAsync<ApiV2ListResponse<TenantDetail>>(BasePath);
            return response.Data ?? new List<TenantDetail>();
        }

        /// <summary>
        /// Gets a tenant by ID.
        /// </summary>
        public Task<TenantDetail> GetAsync(int id)
        {
            return _http.GetAsync<TenantDetail>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Gets a tenant by slug.
        /// </summary>
        public Task<TenantDetail> GetBySlugAsync(string slug)
        {
            return _http.GetAsync<TenantDetail>($"{BasePath}/slug/{slug}");
        }

        /// <summary>
        /// Updates a tenant.
        /// </summary>
        public Task<TenantDetail> UpdateAsync(int id, UpdateTenantRequest request)
        {
            return _http.PutAsync<TenantDetail>($"{BasePath}/{id}", request);
        }
    }

    #endregion

    #region Workspaces Resource

    /// <summary>
    /// Resource client for workspace operations.
    /// </summary>
    public class WorkspacesResource
    {
        private readonly IHttpClient _http;
        private readonly TenantApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/workspaces";

        internal WorkspacesResource(IHttpClient http, TenantApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all workspaces for a tenant.
        /// </summary>
        public async Task<List<WorkspaceDetail>> GetAllAsync(int? tenantId = null)
        {
            var query = tenantId.HasValue ? $"?tenantId={tenantId}" : "";
            var response = await _http.GetAsync<ApiV2ListResponse<WorkspaceDetail>>($"{BasePath}{query}");
            return response.Data ?? new List<WorkspaceDetail>();
        }

        /// <summary>
        /// Gets a workspace by ID.
        /// </summary>
        public Task<WorkspaceDetail> GetAsync(int id)
        {
            return _http.GetAsync<WorkspaceDetail>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Creates a new workspace.
        /// </summary>
        public Task<WorkspaceDetail> CreateAsync(CreateWorkspaceRequest request)
        {
            return _http.PostAsync<WorkspaceDetail>(BasePath, request);
        }

        /// <summary>
        /// Updates a workspace.
        /// </summary>
        public Task<WorkspaceDetail> UpdateAsync(int id, UpdateWorkspaceRequest request)
        {
            return _http.PutAsync<WorkspaceDetail>($"{BasePath}/{id}", request);
        }

        /// <summary>
        /// Deletes a workspace.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.DeleteAsync($"{BasePath}/{id}");
        }
    }

    #endregion
}
