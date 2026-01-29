using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Dashboards;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for dashboard management operations.
/// </summary>
public interface IDashboardClient
{
    #region Dashboard CRUD

    /// <summary>
    /// Gets all dashboards accessible by the current user.
    /// </summary>
    /// <returns>List of dashboard summaries.</returns>
    Task<List<DashboardSummary>> GetAllAsync();

    /// <summary>
    /// Gets a specific dashboard with all widgets.
    /// </summary>
    /// <param name="id">Dashboard ID.</param>
    /// <returns>Dashboard detail including widgets.</returns>
    Task<DashboardDetail> GetAsync(int id);

    /// <summary>
    /// Gets the default dashboard for the current user.
    /// </summary>
    /// <returns>Default dashboard detail, or null if none set.</returns>
    Task<DashboardDetail?> GetDefaultAsync();

    /// <summary>
    /// Creates a new dashboard.
    /// </summary>
    /// <param name="request">Dashboard creation request.</param>
    /// <returns>Created dashboard detail.</returns>
    Task<DashboardDetail> CreateAsync(CreateDashboardRequest request);

    /// <summary>
    /// Updates an existing dashboard.
    /// </summary>
    /// <param name="id">Dashboard ID.</param>
    /// <param name="request">Dashboard update request.</param>
    /// <returns>Updated dashboard detail.</returns>
    Task<DashboardDetail> UpdateAsync(int id, UpdateDashboardRequest request);

    /// <summary>
    /// Deletes a dashboard.
    /// </summary>
    /// <param name="id">Dashboard ID.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Duplicates a dashboard.
    /// </summary>
    /// <param name="id">Dashboard ID to duplicate.</param>
    /// <param name="newName">Optional new name for the duplicate.</param>
    /// <returns>The duplicated dashboard.</returns>
    Task<DashboardDetail> DuplicateAsync(int id, string? newName = null);

    /// <summary>
    /// Sets a dashboard as the user's default.
    /// </summary>
    /// <param name="id">Dashboard ID to set as default.</param>
    Task SetDefaultAsync(int id);

    #endregion

    #region Widget CRUD

    /// <summary>
    /// Gets all widgets for a dashboard.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <returns>List of widget details.</returns>
    Task<List<WidgetDetail>> GetWidgetsAsync(int dashboardId);

    /// <summary>
    /// Gets a specific widget.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <param name="widgetId">Widget ID.</param>
    /// <returns>Widget detail.</returns>
    Task<WidgetDetail> GetWidgetAsync(int dashboardId, int widgetId);

    /// <summary>
    /// Adds a widget to a dashboard.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <param name="request">Widget creation request.</param>
    /// <returns>Created widget detail.</returns>
    Task<WidgetDetail> AddWidgetAsync(int dashboardId, CreateWidgetRequest request);

    /// <summary>
    /// Updates an existing widget.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <param name="widgetId">Widget ID.</param>
    /// <param name="request">Widget update request.</param>
    /// <returns>Updated widget detail.</returns>
    Task<WidgetDetail> UpdateWidgetAsync(int dashboardId, int widgetId, UpdateWidgetRequest request);

    /// <summary>
    /// Deletes a widget from a dashboard.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <param name="widgetId">Widget ID.</param>
    Task DeleteWidgetAsync(int dashboardId, int widgetId);

    /// <summary>
    /// Updates the layout (positions/sizes) of multiple widgets.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <param name="layouts">List of widget layout updates.</param>
    Task UpdateWidgetLayoutAsync(int dashboardId, List<WidgetLayoutItem> layouts);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Creates multiple widgets in a single operation.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <param name="request">Bulk creation request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<WidgetDetail>> CreateWidgetsBulkAsync(int dashboardId, BulkCreateWidgetsRequest request);

    /// <summary>
    /// Updates multiple widgets in a single operation.
    /// </summary>
    /// <param name="dashboardId">Dashboard ID.</param>
    /// <param name="request">Bulk update request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<WidgetDetail>> UpdateWidgetsBulkAsync(int dashboardId, BulkUpdateWidgetsRequest request);

    #endregion
}
