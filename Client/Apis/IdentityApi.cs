using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Users;
using Served.SDK.Models.ApiKeys;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for identity resources including users and API keys.
/// </summary>
public class IdentityApi : ApiModuleBase
{
    protected override string ModulePath => "identity";

    /// <summary>
    /// Access to employee/user resources.
    /// </summary>
    public EmployeesResource Employees { get; }

    /// <summary>
    /// Access to API key resources.
    /// </summary>
    public ApiKeysResource ApiKeys { get; }

    public IdentityApi(IHttpClient http) : base(http)
    {
        Employees = new EmployeesResource(http, this);
        ApiKeys = new ApiKeysResource(http, this);
    }

    #region Employees Resource

    /// <summary>
    /// Resource client for employee/user operations.
    /// </summary>
    public class EmployeesResource
    {
        private readonly IHttpClient _http;
        private readonly IdentityApi _module;

        // Use legacy path for backwards compatibility
        private string BasePath => "api/users";

        internal EmployeesResource(IHttpClient http, IdentityApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all employees.
        /// </summary>
        public async Task<List<EmployeeSummary>> GetAllAsync(EmployeeQueryParams? query = null)
        {
            var q = query ?? new EmployeeQueryParams();
            var queryParams = new List<string>();

            if (q.IsActive.HasValue)
                queryParams.Add($"isActive={q.IsActive.Value}");
            if (q.Take.HasValue)
                queryParams.Add($"pageSize={q.Take.Value}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _http.GetAsync<ApiV2ListResponse<EmployeeDetail>>($"{BasePath}{queryString}");

            return response.Data?.Select(d => new EmployeeSummary
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                IsActive = d.IsActive
            }).ToList() ?? new List<EmployeeSummary>();
        }

        /// <summary>
        /// Gets an employee by ID.
        /// </summary>
        public Task<EmployeeDetail> GetAsync(int id)
        {
            return _http.GetAsync<EmployeeDetail>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Searches employees by term.
        /// </summary>
        public async Task<List<EmployeeSummary>> SearchAsync(string searchTerm, int take = 20)
        {
            var response = await _http.GetAsync<ApiV2ListResponse<EmployeeDetail>>(
                $"{BasePath}?search={System.Web.HttpUtility.UrlEncode(searchTerm)}&pageSize={take}&isActive=true");

            return response.Data?.Select(d => new EmployeeSummary
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                IsActive = d.IsActive
            }).ToList() ?? new List<EmployeeSummary>();
        }
    }

    #endregion

    #region API Keys Resource

    /// <summary>
    /// Resource client for API key operations.
    /// </summary>
    public class ApiKeysResource
    {
        private readonly IHttpClient _http;
        private readonly IdentityApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/api-keys";

        internal ApiKeysResource(IHttpClient http, IdentityApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all API keys.
        /// </summary>
        public async Task<List<ApiKeyViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync<ApiV2ListResponse<ApiKeyViewModel>>(BasePath);
            return response.Data ?? new List<ApiKeyViewModel>();
        }

        /// <summary>
        /// Gets an API key by ID.
        /// </summary>
        public Task<ApiKeyViewModel> GetAsync(int id)
        {
            return _http.GetAsync<ApiKeyViewModel>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Creates a new API key.
        /// </summary>
        public Task<ApiKeyCreatedViewModel> CreateAsync(CreateApiKeyRequest request)
        {
            return _http.PostAsync<ApiKeyCreatedViewModel>(BasePath, request);
        }

        /// <summary>
        /// Updates an API key.
        /// </summary>
        public Task<ApiKeyViewModel> UpdateAsync(int id, UpdateApiKeyRequest request)
        {
            return _http.PutAsync<ApiKeyViewModel>($"{BasePath}/{id}", request);
        }

        /// <summary>
        /// Deletes an API key.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.DeleteAsync($"{BasePath}/{id}");
        }

        /// <summary>
        /// Revokes an API key.
        /// </summary>
        public Task RevokeAsync(int id)
        {
            return _http.PostAsync($"{BasePath}/{id}/revoke", new { });
        }
    }

    #endregion
}
