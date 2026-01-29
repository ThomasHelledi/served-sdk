using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Users;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for employee management operations.
/// </summary>
public class EmployeeClient : IEmployeeClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public EmployeeClient(IServedClient client)
    {
        _client = client;
    }

    /// <inheritdoc/>
    public Task<DetailedEmployeeModel> GetDetailedAsync(int userId)
    {
        return _client.GetAsync<DetailedEmployeeModel>($"api/administration/organization/employee/GetDetailed?userId={userId}");
    }

    /// <inheritdoc/>
    public Task<List<EmployeeListViewModel>> ListAsync(string? search = null, int limit = 50)
    {
        var req = new QueryParams
        {
            Search = search ?? "",
            Skip = 0,
            Take = limit
        };
        return _client.PostAsync<List<EmployeeListViewModel>>("api/administration/organization/employee/Get", req);
    }
}
