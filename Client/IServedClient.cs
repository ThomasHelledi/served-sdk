using System;
using System.Net.Http;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Client.Apis;
using Served.SDK.Models.Users;

namespace Served.SDK.Client;

/// <summary>
/// Interface for the Served API client providing access to all API resources.
/// Supports both module-based access (recommended) and legacy direct access patterns.
/// </summary>
/// <example>
/// // Module-based access (recommended):
/// var projects = await client.ProjectManagement.Projects.GetAllAsync();
///
/// // Legacy access (still supported):
/// var projects = await client.Projects.GetAllAsync();
/// </example>
public interface IServedClient
{
    #region Module-Based API Access (Recommended)

    /// <summary>
    /// Access to Project Management APIs (projects, tasks, sprints).
    /// </summary>
    ProjectManagementApi ProjectManagement { get; }

    /// <summary>
    /// Access to Finance APIs (invoices, billing).
    /// </summary>
    FinanceApi FinanceModule { get; }

    /// <summary>
    /// Access to DevOps APIs (repositories, pull requests, pipelines).
    /// </summary>
    DevOpsApi DevOpsModule { get; }

    /// <summary>
    /// Access to Sales CRM APIs (pipelines, deals).
    /// </summary>
    SalesApi SalesModule { get; }

    /// <summary>
    /// Access to Registration APIs (time registrations).
    /// </summary>
    RegistrationApi Registration { get; }

    /// <summary>
    /// Access to Companies APIs (customers).
    /// </summary>
    CompaniesApi Companies { get; }

    /// <summary>
    /// Access to Identity APIs (users, API keys).
    /// </summary>
    IdentityApi Identity { get; }

    /// <summary>
    /// Access to Calendar APIs (agreements).
    /// </summary>
    CalendarApi Calendar { get; }

    /// <summary>
    /// Access to Board APIs (boards, sheets).
    /// </summary>
    BoardApi BoardModule { get; }

    /// <summary>
    /// Access to Reporting APIs (dashboards, datasources).
    /// </summary>
    ReportingApi Reporting { get; }

    /// <summary>
    /// Access to Tenant APIs (tenants, workspaces).
    /// </summary>
    TenantApi TenantModule { get; }

    /// <summary>
    /// Access to Bootstrap APIs (initialization data).
    /// </summary>
    BootstrapApi BootstrapModule { get; }

    #endregion

    #region Legacy API Access (Backwards Compatibility)

    /// <summary>
    /// [Legacy] Access to Project Management resources.
    /// Consider using ProjectManagement.Projects instead.
    /// </summary>
    IProjectClient Projects { get; }

    /// <summary>
    /// [Legacy] Access to Task Management resources.
    /// Consider using ProjectManagement.Tasks instead.
    /// </summary>
    ITaskClient Tasks { get; }

    /// <summary>
    /// [Legacy] Access to Agreement/Appointment resources.
    /// Consider using Calendar.Agreements instead.
    /// </summary>
    IAgreementClient Agreements { get; }

    /// <summary>
    /// [Legacy] Access to Customer resources.
    /// Consider using Companies.Customers instead.
    /// </summary>
    ICustomerClient Customers { get; }

    /// <summary>
    /// [Legacy] Access to Time Registration resources.
    /// Consider using Registration.TimeRegistrations instead.
    /// </summary>
    ITimeRegistrationClient TimeRegistrations { get; }

    /// <summary>
    /// [Legacy] Access to Employee resources.
    /// Consider using Identity.Employees instead.
    /// </summary>
    IEmployeeClient Employees { get; }

    /// <summary>
    /// [Legacy] Access to Finance resources.
    /// Consider using FinanceModule.Invoices instead.
    /// </summary>
    IFinanceClient Finance { get; }

    /// <summary>
    /// [Legacy] Access to API Key Management resources.
    /// Consider using Identity.ApiKeys instead.
    /// </summary>
    IApiKeyClient ApiKeys { get; }

    /// <summary>
    /// [Legacy] Access to DevOps resources.
    /// Consider using DevOpsModule instead.
    /// </summary>
    IDevOpsClient DevOps { get; }

    /// <summary>
    /// [Legacy] Access to Board and Sheet resources.
    /// Consider using BoardModule instead.
    /// </summary>
    IBoardClient Boards { get; }

    /// <summary>
    /// [Legacy] Access to Sales CRM resources.
    /// Consider using SalesModule instead.
    /// </summary>
    ISalesClient Sales { get; }

    /// <summary>
    /// [Legacy] Access to Dashboard resources.
    /// Consider using Reporting.Dashboards instead.
    /// </summary>
    IDashboardClient Dashboards { get; }

    /// <summary>
    /// [Legacy] Access to Datasource resources.
    /// Consider using Reporting.Datasources instead.
    /// </summary>
    IDatasourceClient Datasource { get; }

    /// <summary>
    /// [Legacy] Access to Tenant resources.
    /// Consider using TenantModule instead.
    /// </summary>
    ITenantClient Tenants { get; }

    /// <summary>
    /// [Legacy] Access to Bootstrap resources.
    /// Consider using BootstrapModule instead.
    /// </summary>
    IBootstrapClient Bootstrap { get; }

    #endregion

    /// <summary>
    /// Gets the user bootstrap data including tenant and workspace information.
    /// </summary>
    /// <returns>User bootstrap view model containing user context data.</returns>
    Task<UserBootstrapViewModel> GetUserBootstrapAsync();

    /// <summary>
    /// Performs a GET request to the specified URI.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The relative resource URI.</param>
    /// <returns>The deserialized response.</returns>
    Task<T> GetAsync<T>(string uri);

    /// <summary>
    /// Performs a POST request to the specified URI.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The relative resource URI.</param>
    /// <param name="data">The request body to serialize as JSON.</param>
    /// <returns>The deserialized response.</returns>
    Task<T> PostAsync<T>(string uri, object data);

    /// <summary>
    /// Performs a POST request without expecting a return value.
    /// </summary>
    /// <param name="uri">The relative resource URI.</param>
    /// <param name="data">The request body to serialize as JSON.</param>
    Task PostAsync(string uri, object data);

    /// <summary>
    /// Performs a PUT request to the specified URI.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The relative resource URI.</param>
    /// <param name="data">The request body to serialize as JSON.</param>
    /// <returns>The deserialized response.</returns>
    Task<T> PutAsync<T>(string uri, object data);

    /// <summary>
    /// Performs a DELETE request with a JSON body.
    /// </summary>
    /// <param name="uri">The relative resource URI.</param>
    /// <param name="data">The request body to serialize as JSON.</param>
    Task DeleteWithBodyAsync(string uri, object data);

    /// <summary>
    /// Performs a DELETE request with a JSON body and returns a typed response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The relative resource URI.</param>
    /// <param name="data">The request body to serialize as JSON.</param>
    /// <returns>The deserialized response.</returns>
    Task<T> DeleteWithBodyAsync<T>(string uri, object data);

    /// <summary>
    /// Performs a simple DELETE request without a body.
    /// </summary>
    /// <param name="uri">The relative resource URI.</param>
    Task DeleteAsync(string uri);

    /// <summary>
    /// Performs a PATCH request to the specified URI.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="uri">The relative resource URI.</param>
    /// <param name="data">The request body to serialize as JSON.</param>
    /// <returns>The deserialized response.</returns>
    Task<T> PatchAsync<T>(string uri, object data);

    /// <summary>
    /// Resolves a tenant slug to its integer ID.
    /// </summary>
    /// <param name="tenantSlug">The tenant/organization slug.</param>
    /// <returns>The tenant ID.</returns>
    /// <exception cref="ArgumentException">Thrown when the tenant slug is null or empty.</exception>
    /// <exception cref="Exceptions.ServedApiException">Thrown when the tenant is not found.</exception>
    Task<int> GetTenantIdAsync(string tenantSlug);
}
