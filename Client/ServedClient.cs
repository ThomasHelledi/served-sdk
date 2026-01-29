using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Served.SDK.Client.Interfaces;
using Served.SDK.Client.Resources;
using Served.SDK.Client.Core;
using Served.SDK.Client.Apis;
using Served.SDK.Models.Users;
using Served.SDK.Exceptions;
using Served.SDK.Tracing;

namespace Served.SDK.Client;

/// <summary>
/// A strongly-typed client for the Served API.
/// Provides both module-based access (recommended) and legacy direct access patterns.
/// </summary>
/// <example>
/// // Module-based access (recommended):
/// var projects = await client.ProjectManagement.Projects.GetAllAsync();
/// var invoices = await client.Finance.Invoices.GetAllAsync();
///
/// // Legacy access (still supported):
/// var projects = await client.Projects.GetAllAsync();
/// </example>
public class ServedClient : IServedClient, IHttpClient, IDisposable
{
    /// <summary>
    /// Default API base URL for UnifiedHQ infrastructure.
    /// Can be overridden via SERVED_API_URL environment variable.
    /// </summary>
    public const string DefaultApiUrl = "https://apis.unifiedhq.ai";

    /// <summary>
    /// Alternative API base URL (served.dk infrastructure).
    /// </summary>
    public const string AlternativeApiUrl = "https://apis.served.dk";

    /// <summary>
    /// Gets the configured API URL from environment or returns the default.
    /// Checks SERVED_API_URL environment variable first.
    /// </summary>
    public static string GetConfiguredApiUrl()
    {
        var envUrl = Environment.GetEnvironmentVariable("SERVED_API_URL");
        return !string.IsNullOrEmpty(envUrl) ? envUrl : DefaultApiUrl;
    }

    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly string _tenant;
    private IServedTracer? _tracer;
    private bool _disposed;

    /// <summary>
    /// Gets the tracer for observability. Returns null if tracing is not enabled.
    /// </summary>
    public IServedTracer? Tracer => _tracer;

    /// <summary>
    /// Check if tracing is enabled on this client.
    /// </summary>
    public bool IsTracingEnabled => _tracer?.IsEnabled ?? false;

    #region Module-Based API Access (Recommended)

    /// <summary>
    /// Access to Project Management APIs (projects, tasks, sprints).
    /// </summary>
    public ProjectManagementApi ProjectManagement { get; }

    /// <summary>
    /// Access to Finance APIs (invoices, billing).
    /// </summary>
    public FinanceApi FinanceModule { get; }

    /// <summary>
    /// Access to DevOps APIs (repositories, pull requests, pipelines).
    /// </summary>
    public DevOpsApi DevOpsModule { get; }

    /// <summary>
    /// Access to Sales CRM APIs (pipelines, deals).
    /// </summary>
    public SalesApi SalesModule { get; }

    /// <summary>
    /// Access to Registration APIs (time registrations).
    /// </summary>
    public RegistrationApi Registration { get; }

    /// <summary>
    /// Access to Companies APIs (customers).
    /// </summary>
    public CompaniesApi Companies { get; }

    /// <summary>
    /// Access to Identity APIs (users, API keys).
    /// </summary>
    public IdentityApi Identity { get; }

    /// <summary>
    /// Access to Calendar APIs (agreements).
    /// </summary>
    public CalendarApi Calendar { get; }

    /// <summary>
    /// Access to Board APIs (boards, sheets).
    /// </summary>
    public BoardApi BoardModule { get; }

    /// <summary>
    /// Access to Reporting APIs (dashboards, datasources).
    /// </summary>
    public ReportingApi Reporting { get; }

    /// <summary>
    /// Access to Tenant APIs (tenants, workspaces).
    /// </summary>
    public TenantApi TenantModule { get; }

    /// <summary>
    /// Access to Bootstrap APIs (initialization data).
    /// </summary>
    public BootstrapApi BootstrapModule { get; }

    /// <summary>
    /// Access to Health APIs (system verification).
    /// Use this to verify services are running before claiming success.
    /// </summary>
    public HealthApi Health { get; }

    #endregion

    #region Legacy API Access (Backwards Compatibility)

    /// <inheritdoc/>
    public IProjectClient Projects { get; }

    /// <inheritdoc/>
    public ITaskClient Tasks { get; }

    /// <inheritdoc/>
    public IAgreementClient Agreements { get; }

    /// <inheritdoc/>
    public ICustomerClient Customers { get; }

    /// <inheritdoc/>
    public ITimeRegistrationClient TimeRegistrations { get; }

    /// <inheritdoc/>
    public IEmployeeClient Employees { get; }

    /// <inheritdoc/>
    public IFinanceClient Finance { get; }

    /// <inheritdoc/>
    public IApiKeyClient ApiKeys { get; }

    /// <inheritdoc/>
    public IDevOpsClient DevOps { get; }

    /// <inheritdoc/>
    public IBoardClient Boards { get; }

    /// <inheritdoc/>
    public ISalesClient Sales { get; }

    /// <inheritdoc/>
    public IDashboardClient Dashboards { get; }

    /// <inheritdoc/>
    public IDatasourceClient Datasource { get; }

    /// <inheritdoc/>
    public ITenantClient Tenants { get; }

    /// <inheritdoc/>
    public IBootstrapClient Bootstrap { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ServedClient"/>.
    /// </summary>
    /// <param name="baseUrl">The base API URL (e.g. https://api.served.dk)</param>
    /// <param name="token">Bearer token for authentication</param>
    /// <param name="tenant">Optional tenant ID/slug for headers</param>
    /// <param name="httpClient">Optional external HttpClient</param>
    public ServedClient(string baseUrl, string? token = null, string? tenant = null, HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        _token = token ?? string.Empty;
        _tenant = tenant ?? string.Empty;

        if (!string.IsNullOrEmpty(_token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        if (!string.IsNullOrEmpty(_tenant))
        {
            // Adding multiple headers for compatibility
            if (!_httpClient.DefaultRequestHeaders.Contains("Served-Tenant"))
            {
               _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Served-Tenant", _tenant);
            }
        }

        // Initialize module-based APIs (new pattern)
        ProjectManagement = new ProjectManagementApi(this);
        FinanceModule = new FinanceApi(this);
        DevOpsModule = new DevOpsApi(this);
        SalesModule = new SalesApi(this);
        Registration = new RegistrationApi(this);
        Companies = new CompaniesApi(this);
        Identity = new IdentityApi(this);
        Calendar = new CalendarApi(this);
        BoardModule = new BoardApi(this);
        Reporting = new ReportingApi(this);
        TenantModule = new TenantApi(this);
        BootstrapModule = new BootstrapApi(this);
        Health = new HealthApi(this);

        // Initialize legacy clients (backwards compatibility)
        Projects = new ProjectClient(this);
        Tasks = new TaskClient(this);
        Agreements = new AgreementClient(this);
        Customers = new CustomerClient(this);
        TimeRegistrations = new TimeRegistrationClient(this);
        Employees = new EmployeeClient(this);
        Finance = new FinanceClient(this);
        ApiKeys = new ApiKeyClient(this);
        DevOps = new DevOpsClient(this);
        Boards = new BoardClient(this);
        Sales = new SalesClient(this);
        Dashboards = new DashboardClient(this);
        Datasource = new DatasourceClient(this);
        Tenants = new TenantClient(this);
        Bootstrap = new BootstrapClient(this);

        // Auto-register session in background
        _ = RegisterSessionInBackgroundAsync();
    }

    private bool _sessionRegistered = false;

    private async Task RegisterSessionInBackgroundAsync()
    {
        if (_sessionRegistered) return;
        _sessionRegistered = true;

        try
        {
            var payload = new
            {
                hostname = Environment.MachineName,
                workingDirectory = Directory.GetCurrentDirectory(),
                source = "sdk",
                version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0"
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Fire and forget - don't wait for response
            await _httpClient.PostAsync("/api/agents/coordination/RegisterClaudeSession", content);
        }
        catch
        {
            // Session registration should never fail the client
        }
    }

    /// <summary>
    /// Manually register a session with the coordination API
    /// </summary>
    public async Task<SessionInfo?> RegisterSessionAsync(int? taskId = null, string? taskName = null)
    {
        try
        {
            var payload = new
            {
                hostname = Environment.MachineName,
                workingDirectory = Directory.GetCurrentDirectory(),
                source = "sdk-manual",
                taskId,
                taskName
            };

            var response = await PostAsync<SessionInfo>("/api/agents/coordination/RegisterClaudeSession", payload);
            return response;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Session information returned from registration
    /// </summary>
    public class SessionInfo
    {
        public string? SessionId { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public List<ConflictWarning>? ConflictWarnings { get; set; }
    }

    public class ConflictWarning
    {
        public string? Description { get; set; }
        public string? FilePath { get; set; }
    }

    /// <inheritdoc/>
    public Task<UserBootstrapViewModel> GetUserBootstrapAsync()
    {
        return GetAsync<UserBootstrapViewModel>("api/core/bootstrap/user");
    }

    /// <inheritdoc/>
    public async Task<T> GetAsync<T>(string uri)
    {
        var response = await _httpClient.GetAsync(uri);
        return await HandleResponse<T>(response);
    }

    /// <inheritdoc/>
    public async Task<T> PostAsync<T>(string uri, object data)
    {
        var content = CreateJsonContent(data);
        var response = await _httpClient.PostAsync(uri, content);
        return await HandleResponse<T>(response);
    }

    /// <inheritdoc/>
    public async Task PostAsync(string uri, object data)
    {
        var content = CreateJsonContent(data);
        var response = await _httpClient.PostAsync(uri, content);
        if (!response.IsSuccessStatusCode)
        {
             var error = await response.Content.ReadAsStringAsync();
             throw new ServedApiException($"API Error: {response.StatusCode} - {error}", (int)response.StatusCode, error);
        }
    }

    /// <inheritdoc/>
    public async Task<T> PutAsync<T>(string uri, object data)
    {
        var content = CreateJsonContent(data);
        var response = await _httpClient.PutAsync(uri, content);
        return await HandleResponse<T>(response);
    }

    /// <inheritdoc/>
    public async Task DeleteWithBodyAsync(string uri, object data)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri)
        {
            Content = CreateJsonContent(data)
        };

        var response = await _httpClient.SendAsync(request);
        await HandleResponse<object>(response);
    }

    /// <inheritdoc/>
    public async Task<T> DeleteWithBodyAsync<T>(string uri, object data)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri)
        {
            Content = CreateJsonContent(data)
        };

        var response = await _httpClient.SendAsync(request);
        return await HandleResponse<T>(response);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string uri)
    {
        var response = await _httpClient.DeleteAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ServedApiException($"API Error: {response.StatusCode} - {error}", (int)response.StatusCode, error);
        }
    }

    /// <inheritdoc/>
    public async Task<T> PatchAsync<T>(string uri, object data)
    {
        var content = CreateJsonContent(data);
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
        {
            Content = content
        };
        var response = await _httpClient.SendAsync(request);
        return await HandleResponse<T>(response);
    }

    private StringContent CreateJsonContent(object data)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };
        var json = JsonConvert.SerializeObject(data, settings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ServedApiException($"API Error: {response.StatusCode} - {error}", (int)response.StatusCode, error);
        }

        // Handle 204 NoContent - return default(T)
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return default!;
        }

        var json = await response.Content.ReadAsStringAsync();

        // Handle empty response body
        if (string.IsNullOrWhiteSpace(json))
        {
            return default!;
        }

        var contentType = response.Content.Headers.ContentType?.MediaType;
        if (contentType != null && !contentType.Contains("json"))
        {
             // If we get HTML (e.g. login page redirect), treat as error
             var contentSnippet = json;
             if (contentSnippet.Length > 200) contentSnippet = contentSnippet.Substring(0, 200) + "...";
             throw new ServedApiException($"API Returned Non-JSON Content ({contentType}): {contentSnippet}", (int)response.StatusCode, contentSnippet);
        }

        // If T is just string, return the raw json or content
        if (typeof(T) == typeof(string))
        {
             return (T)(object)json;
        }

        // Handle primitive types (int, bool, etc.)
        if (typeof(T).IsPrimitive || typeof(T) == typeof(int) || typeof(T) == typeof(long))
        {
            // Try to parse as the primitive type
            var trimmed = json.Trim('"');
            return (T)Convert.ChangeType(trimmed, typeof(T));
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(json) ?? default!;
        }
        catch (JsonException ex)
        {
            var snippet = json.Length > 200 ? json.Substring(0, 200) + "..." : json;
            throw new Exception($"JSON Parse Error: {ex.Message}. Content: {snippet}");
        }
    }

    private int? _cachedTenantId;

    /// <inheritdoc/>
    public async Task<int> GetTenantIdAsync(string tenantSlug)
    {
        if (_cachedTenantId.HasValue) return _cachedTenantId.Value;

        if (string.IsNullOrEmpty(tenantSlug))
             throw new ArgumentException("Tenant slug cannot be null or empty", nameof(tenantSlug));

        try
        {
            // Note: This relies on the current token having access to user bootstrap
            var bootstrap = await GetAsync<UserBootstrapViewModel>("/api/core/bootstrap/user");
            var tenant = bootstrap.Tenants.FirstOrDefault(t => t.Slug.Equals(tenantSlug, StringComparison.OrdinalIgnoreCase));

            if (tenant == null)
                throw new ServedApiException($"Tenant '{tenantSlug}' not found in user context.", 404, null);

            _cachedTenantId = tenant.Id;
            return tenant.Id;
        }
        catch (ServedApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to resolve Tenant ID for '{tenantSlug}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Set the tracer for observability.
    /// </summary>
    /// <param name="tracer">The tracer instance to use.</param>
    internal void SetTracer(IServedTracer tracer)
    {
        _tracer?.Dispose();
        _tracer = tracer;

        // Set tenant context if available
        if (!string.IsNullOrEmpty(_tenant))
        {
            _tracer.SetContext("served.tenant", _tenant);
        }
    }

    /// <summary>
    /// Disposes the client and releases resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the client and releases resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _tracer?.Dispose();
        }

        _disposed = true;
    }
}
