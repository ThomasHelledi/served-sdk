using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.DevOps;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for DevOps resources including repositories, pull requests, and pipelines.
/// </summary>
public class DevOpsApi : ApiModuleBase
{
    protected override string ModulePath => "devops";

    /// <summary>
    /// Access to repository resources.
    /// </summary>
    public RepositoriesResource Repositories { get; }

    /// <summary>
    /// Access to pull request resources.
    /// </summary>
    public PullRequestsResource PullRequests { get; }

    /// <summary>
    /// Access to pipeline resources.
    /// </summary>
    public PipelinesResource Pipelines { get; }

    /// <summary>
    /// Access to unified release resources.
    /// </summary>
    public ReleasesResource Releases { get; }

    public DevOpsApi(IHttpClient http) : base(http)
    {
        Repositories = new RepositoriesResource(http, this);
        PullRequests = new PullRequestsResource(http, this);
        Pipelines = new PipelinesResource(http, this);
        Releases = new ReleasesResource(http, this);
    }

    #region Repositories Resource

    /// <summary>
    /// Resource client for repository operations.
    /// </summary>
    public class RepositoriesResource
    {
        private readonly IHttpClient _http;
        private readonly DevOpsApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/repositories";

        internal RepositoriesResource(IHttpClient http, DevOpsApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all repositories.
        /// </summary>
        public async Task<List<DevOpsRepositoryViewModel>> GetAllAsync(bool activeOnly = true)
        {
            var response = await _http.GetAsync<ApiListResponse<DevOpsRepositoryViewModel>>(
                $"{BasePath}?activeOnly={activeOnly}");
            return response.Data ?? new List<DevOpsRepositoryViewModel>();
        }

        /// <summary>
        /// Gets a repository by ID.
        /// </summary>
        public Task<DevOpsRepositoryViewModel> GetAsync(int id)
        {
            return _http.GetAsync<DevOpsRepositoryViewModel>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Creates a new repository.
        /// </summary>
        public Task<DevOpsRepositoryViewModel> CreateAsync(CreateRepositoryRequest request)
        {
            return _http.PostAsync<DevOpsRepositoryViewModel>(BasePath, request);
        }

        /// <summary>
        /// Updates a repository.
        /// </summary>
        public Task<DevOpsRepositoryViewModel> UpdateAsync(UpdateRepositoryRequest request)
        {
            return _http.PutAsync<DevOpsRepositoryViewModel>(BasePath, request);
        }

        /// <summary>
        /// Deletes a repository.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.DeleteAsync($"{BasePath}/{id}");
        }

        /// <summary>
        /// Gets pull requests for this repository.
        /// </summary>
        public async Task<List<PullRequestViewModel>> GetPullRequestsAsync(int repositoryId, string? state = null, int limit = 50)
        {
            var query = $"{BasePath}/{repositoryId}/pullrequests?limit={limit}";
            if (!string.IsNullOrEmpty(state))
                query += $"&state={state}";

            var response = await _http.GetAsync<ApiListResponse<PullRequestViewModel>>(query);
            return response.Data ?? new List<PullRequestViewModel>();
        }

        /// <summary>
        /// Gets pipeline runs for this repository.
        /// </summary>
        public async Task<List<PipelineRunViewModel>> GetPipelineRunsAsync(int repositoryId, int limit = 50)
        {
            var response = await _http.GetAsync<ApiListResponse<PipelineRunViewModel>>(
                $"{BasePath}/{repositoryId}/runs?limit={limit}");
            return response.Data ?? new List<PipelineRunViewModel>();
        }
    }

    #endregion

    #region Pull Requests Resource

    /// <summary>
    /// Resource client for pull request operations.
    /// </summary>
    public class PullRequestsResource
    {
        private readonly IHttpClient _http;
        private readonly DevOpsApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/pullrequests";

        internal PullRequestsResource(IHttpClient http, DevOpsApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all pull requests.
        /// </summary>
        public async Task<List<PullRequestViewModel>> GetAllAsync(string? state = null, int limit = 50)
        {
            var query = $"{BasePath}?limit={limit}";
            if (!string.IsNullOrEmpty(state))
                query += $"&state={state}";

            var response = await _http.GetAsync<ApiListResponse<PullRequestViewModel>>(query);
            return response.Data ?? new List<PullRequestViewModel>();
        }

        /// <summary>
        /// Gets pull requests by task ID.
        /// </summary>
        public async Task<List<PullRequestViewModel>> GetByTaskAsync(int taskId)
        {
            var response = await _http.GetAsync<ApiListResponse<PullRequestViewModel>>(
                $"api/{_module.Version}/{_module.ModulePath}/tasks/{taskId}/pullrequests");
            return response.Data ?? new List<PullRequestViewModel>();
        }

        /// <summary>
        /// Gets pull requests by session ID.
        /// </summary>
        public async Task<List<PullRequestViewModel>> GetBySessionAsync(int sessionId)
        {
            var response = await _http.GetAsync<ApiListResponse<PullRequestViewModel>>(
                $"api/{_module.Version}/{_module.ModulePath}/sessions/{sessionId}/pullrequests");
            return response.Data ?? new List<PullRequestViewModel>();
        }

        /// <summary>
        /// Gets pipeline runs for a pull request.
        /// </summary>
        public async Task<List<PipelineRunViewModel>> GetPipelineRunsAsync(int pullRequestId)
        {
            var response = await _http.GetAsync<ApiListResponse<PipelineRunViewModel>>(
                $"{BasePath}/{pullRequestId}/runs");
            return response.Data ?? new List<PipelineRunViewModel>();
        }

        /// <summary>
        /// Gets the latest pipeline run for a pull request.
        /// </summary>
        public async Task<PipelineRunViewModel?> GetLatestPipelineRunAsync(int pullRequestId)
        {
            try
            {
                return await _http.GetAsync<PipelineRunViewModel>(
                    $"{BasePath}/{pullRequestId}/runs/latest");
            }
            catch
            {
                return null;
            }
        }
    }

    #endregion

    #region Pipelines Resource

    /// <summary>
    /// Resource client for pipeline operations.
    /// </summary>
    public class PipelinesResource
    {
        private readonly IHttpClient _http;
        private readonly DevOpsApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/pipelines";

        internal PipelinesResource(IHttpClient http, DevOpsApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all pipeline runs.
        /// </summary>
        public async Task<List<PipelineRunViewModel>> GetAllRunsAsync(int limit = 50)
        {
            var response = await _http.GetAsync<ApiListResponse<PipelineRunViewModel>>(
                $"{BasePath}/runs?limit={limit}");
            return response.Data ?? new List<PipelineRunViewModel>();
        }

        /// <summary>
        /// Gets a specific pipeline run.
        /// </summary>
        public Task<PipelineRunViewModel> GetRunAsync(int runId)
        {
            return _http.GetAsync<PipelineRunViewModel>($"{BasePath}/runs/{runId}");
        }
    }

    #endregion

    #region Releases Resource

    /// <summary>
    /// Resource client for unified release operations.
    /// Supports full release lifecycle: create, build, test, deploy, verify, publish, rollback.
    /// </summary>
    public class ReleasesResource
    {
        private readonly IHttpClient _http;
        private readonly DevOpsApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/releases";

        internal ReleasesResource(IHttpClient http, DevOpsApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all releases with optional filtering.
        /// </summary>
        public async Task<List<UnifiedReleaseViewModel>> GetAllAsync(
            ReleaseStage? stage = null,
            int limit = 50)
        {
            var query = $"{BasePath}?limit={limit}";
            if (stage.HasValue)
                query += $"&stage={stage.Value}";

            var response = await _http.GetAsync<ApiListResponse<UnifiedReleaseViewModel>>(query);
            return response.Data ?? new List<UnifiedReleaseViewModel>();
        }

        /// <summary>
        /// Gets a release by ID.
        /// </summary>
        public Task<UnifiedReleaseViewModel> GetAsync(int id)
        {
            return _http.GetAsync<UnifiedReleaseViewModel>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Gets a release by version.
        /// </summary>
        public Task<UnifiedReleaseViewModel> GetByVersionAsync(string version)
        {
            return _http.GetAsync<UnifiedReleaseViewModel>($"{BasePath}/version/{version}");
        }

        /// <summary>
        /// Gets the current (latest published) release.
        /// </summary>
        public async Task<UnifiedReleaseViewModel?> GetCurrentAsync()
        {
            try
            {
                return await _http.GetAsync<UnifiedReleaseViewModel>($"{BasePath}/current");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets release summary for dashboard.
        /// </summary>
        public Task<ReleaseSummaryViewModel> GetSummaryAsync()
        {
            return _http.GetAsync<ReleaseSummaryViewModel>($"{BasePath}/summary");
        }

        /// <summary>
        /// Creates a new release.
        /// </summary>
        public Task<UnifiedReleaseViewModel> CreateAsync(CreateUnifiedReleaseRequest request)
        {
            return _http.PostAsync<UnifiedReleaseViewModel>(BasePath, request);
        }

        /// <summary>
        /// Updates the stage of a release.
        /// </summary>
        public Task<UnifiedReleaseViewModel> UpdateStageAsync(int id, UpdateReleaseStageRequest request)
        {
            return _http.PutAsync<UnifiedReleaseViewModel>($"{BasePath}/{id}/stage", request);
        }

        /// <summary>
        /// Triggers deploy for a release.
        /// </summary>
        public Task<UnifiedReleaseViewModel> DeployAsync(int id, List<string>? targets = null)
        {
            var request = new { Targets = targets };
            return _http.PostAsync<UnifiedReleaseViewModel>($"{BasePath}/{id}/deploy", request);
        }

        /// <summary>
        /// Triggers verification (health checks) for a release.
        /// </summary>
        public Task<UnifiedReleaseViewModel> VerifyAsync(int id)
        {
            return _http.PostAsync<UnifiedReleaseViewModel>($"{BasePath}/{id}/verify", new { });
        }

        /// <summary>
        /// Publishes a release (makes it live, publishes packages).
        /// </summary>
        public Task<UnifiedReleaseViewModel> PublishAsync(int id)
        {
            return _http.PostAsync<UnifiedReleaseViewModel>($"{BasePath}/{id}/publish", new { });
        }

        /// <summary>
        /// Rolls back to a previous release.
        /// </summary>
        public Task<UnifiedReleaseViewModel> RollbackAsync(int id, RollbackReleaseRequest? request = null)
        {
            return _http.PostAsync<UnifiedReleaseViewModel>(
                $"{BasePath}/{id}/rollback",
                request ?? new RollbackReleaseRequest());
        }

        /// <summary>
        /// Cancels an in-progress release.
        /// </summary>
        public Task CancelAsync(int id)
        {
            return _http.PostAsync<object>($"{BasePath}/{id}/cancel", new { });
        }

        /// <summary>
        /// Gets deployments for a specific release.
        /// </summary>
        public async Task<List<ReleaseDeploymentViewModel>> GetDeploymentsAsync(int releaseId)
        {
            var response = await _http.GetAsync<ApiListResponse<ReleaseDeploymentViewModel>>(
                $"{BasePath}/{releaseId}/deployments");
            return response.Data ?? new List<ReleaseDeploymentViewModel>();
        }

        /// <summary>
        /// Gets health check results for a release.
        /// </summary>
        public async Task<List<ReleaseHealthCheckViewModel>> GetHealthChecksAsync(int releaseId)
        {
            var response = await _http.GetAsync<ApiListResponse<ReleaseHealthCheckViewModel>>(
                $"{BasePath}/{releaseId}/health");
            return response.Data ?? new List<ReleaseHealthCheckViewModel>();
        }

        /// <summary>
        /// Gets packages published in a release.
        /// </summary>
        public async Task<List<ReleasePackageViewModel>> GetPackagesAsync(int releaseId)
        {
            var response = await _http.GetAsync<ApiListResponse<ReleasePackageViewModel>>(
                $"{BasePath}/{releaseId}/packages");
            return response.Data ?? new List<ReleasePackageViewModel>();
        }

        /// <summary>
        /// Calculates the next version number (CalVer).
        /// </summary>
        public Task<string> GetNextVersionAsync()
        {
            return _http.GetAsync<string>($"{BasePath}/next-version");
        }

        /// <summary>
        /// Gets release history with statistics.
        /// </summary>
        public async Task<List<UnifiedReleaseViewModel>> GetHistoryAsync(
            DateTime? from = null,
            DateTime? to = null,
            int limit = 100)
        {
            var query = $"{BasePath}/history?limit={limit}";
            if (from.HasValue)
                query += $"&from={from.Value:O}";
            if (to.HasValue)
                query += $"&to={to.Value:O}";

            var response = await _http.GetAsync<ApiListResponse<UnifiedReleaseViewModel>>(query);
            return response.Data ?? new List<UnifiedReleaseViewModel>();
        }
    }

    #endregion
}
