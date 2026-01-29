using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Dashboards;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for dashboard and widget management operations.
/// </summary>
public class DashboardClient : IDashboardClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public DashboardClient(IServedClient client)
    {
        _client = client;
    }

    #region Dashboard CRUD

    /// <inheritdoc/>
    public async Task<List<DashboardSummary>> GetAllAsync()
    {
        var response = await _client.GetAsync<DashboardListResponse<DashboardSummary>>("api/dashboards");
        return response.Data;
    }

    /// <inheritdoc/>
    public Task<DashboardDetail> GetAsync(int id)
    {
        return _client.GetAsync<DashboardDetail>($"api/dashboards/{id}");
    }

    /// <inheritdoc/>
    public async Task<DashboardDetail?> GetDefaultAsync()
    {
        try
        {
            return await _client.GetAsync<DashboardDetail>("api/dashboards/default");
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public Task<DashboardDetail> CreateAsync(CreateDashboardRequest request)
    {
        return _client.PostAsync<DashboardDetail>("api/dashboards", request);
    }

    /// <inheritdoc/>
    public Task<DashboardDetail> UpdateAsync(int id, UpdateDashboardRequest request)
    {
        return _client.PutAsync<DashboardDetail>($"api/dashboards/{id}", request);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(int id)
    {
        return _client.DeleteAsync($"api/dashboards/{id}");
    }

    /// <inheritdoc/>
    public Task<DashboardDetail> DuplicateAsync(int id, string? newName = null)
    {
        object body = newName != null ? new { newName } : new { };
        return _client.PostAsync<DashboardDetail>($"api/dashboards/{id}/duplicate", body);
    }

    /// <inheritdoc/>
    public Task SetDefaultAsync(int id)
    {
        return _client.PostAsync($"api/dashboards/{id}/set-default", new { });
    }

    #endregion

    #region Widget CRUD

    /// <inheritdoc/>
    public async Task<List<WidgetDetail>> GetWidgetsAsync(int dashboardId)
    {
        var response = await _client.GetAsync<DashboardListResponse<WidgetDetail>>($"api/dashboards/{dashboardId}/widgets");
        return response.Data;
    }

    /// <inheritdoc/>
    public Task<WidgetDetail> GetWidgetAsync(int dashboardId, int widgetId)
    {
        return _client.GetAsync<WidgetDetail>($"api/dashboards/{dashboardId}/widgets/{widgetId}");
    }

    /// <inheritdoc/>
    public Task<WidgetDetail> AddWidgetAsync(int dashboardId, CreateWidgetRequest request)
    {
        return _client.PostAsync<WidgetDetail>($"api/dashboards/{dashboardId}/widgets", request);
    }

    /// <inheritdoc/>
    public Task<WidgetDetail> UpdateWidgetAsync(int dashboardId, int widgetId, UpdateWidgetRequest request)
    {
        return _client.PutAsync<WidgetDetail>($"api/dashboards/{dashboardId}/widgets/{widgetId}", request);
    }

    /// <inheritdoc/>
    public Task DeleteWidgetAsync(int dashboardId, int widgetId)
    {
        return _client.DeleteAsync($"api/dashboards/{dashboardId}/widgets/{widgetId}");
    }

    /// <inheritdoc/>
    public Task UpdateWidgetLayoutAsync(int dashboardId, List<WidgetLayoutItem> layouts)
    {
        return _client.PutAsync<object>($"api/dashboards/{dashboardId}/widgets/layout", new { layouts });
    }

    #endregion

    #region Bulk Operations

    /// <inheritdoc/>
    public Task<BulkResponse<WidgetDetail>> CreateWidgetsBulkAsync(int dashboardId, BulkCreateWidgetsRequest request)
    {
        return _client.PostAsync<BulkResponse<WidgetDetail>>($"api/dashboards/{dashboardId}/widgets/bulk", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<WidgetDetail>> UpdateWidgetsBulkAsync(int dashboardId, BulkUpdateWidgetsRequest request)
    {
        return _client.PutAsync<BulkResponse<WidgetDetail>>($"api/dashboards/{dashboardId}/widgets/bulk", request);
    }

    #endregion
}
