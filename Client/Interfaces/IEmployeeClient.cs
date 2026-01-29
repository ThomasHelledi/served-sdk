using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Users;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for employee management operations.
/// </summary>
public interface IEmployeeClient
{
    /// <summary>
    /// Gets detailed employee information by user ID.
    /// </summary>
    /// <param name="userId">The user ID of the employee.</param>
    /// <returns>Detailed employee model with full information.</returns>
    Task<DetailedEmployeeModel> GetDetailedAsync(int userId);

    /// <summary>
    /// Lists employees with optional search and pagination.
    /// </summary>
    /// <param name="search">Optional search term to filter employees.</param>
    /// <param name="limit">Maximum number of employees to return (default: 50).</param>
    /// <returns>List of employee list view models.</returns>
    Task<List<EmployeeListViewModel>> ListAsync(string? search = null, int limit = 50);
}
