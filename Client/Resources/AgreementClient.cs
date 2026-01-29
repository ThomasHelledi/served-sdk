using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Agreements;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for agreement/appointment management operations using API V2 endpoints.
/// </summary>
public class AgreementClient : IAgreementClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgreementClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public AgreementClient(IServedClient client)
    {
        _client = client;
    }

    #region CRUD Operations

    /// <inheritdoc/>
    public async Task<List<AgreementSummary>> GetAllAsync(AgreementQueryParams? query = null)
    {
        var q = query ?? new AgreementQueryParams();

        // Build query string for API V2 endpoint
        var queryParams = new List<string>();

        if (q.CustomerId.HasValue)
            queryParams.Add($"customerId={q.CustomerId.Value}");
        if (q.StartDate.HasValue)
            queryParams.Add($"startDate={HttpUtility.UrlEncode(q.StartDate.Value.ToString("o"))}");
        if (q.EndDate.HasValue)
            queryParams.Add($"endDate={HttpUtility.UrlEncode(q.EndDate.Value.ToString("o"))}");

        var page = q.Skip.HasValue && q.Take.HasValue ? (q.Skip.Value / q.Take.Value) + 1 : 1;
        var pageSize = q.Take ?? 50;
        queryParams.Add($"page={page}");
        queryParams.Add($"pageSize={pageSize}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var response = await _client.GetAsync<ApiV2ListResponse<AgreementDetail>>($"api/agreements{queryString}");

        return response.Data?.Select(d => new AgreementSummary
        {
            Id = d.Id,
            Title = d.Title,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            CustomerId = d.CustomerId,
            Description = d.Description
        }).ToList() ?? new List<AgreementSummary>();
    }

    /// <inheritdoc/>
    public Task<AgreementDetail> GetAsync(int id)
    {
        return _client.GetAsync<AgreementDetail>($"api/agreements/{id}");
    }

    /// <inheritdoc/>
    public Task<AgreementDetail> CreateAsync(CreateAgreementRequest request)
    {
        return _client.PostAsync<AgreementDetail>("api/agreements", request);
    }

    /// <inheritdoc/>
    public Task<AgreementDetail> UpdateAsync(int id, UpdateAgreementRequest request)
    {
        return _client.PutAsync<AgreementDetail>($"api/agreements/{id}", request);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(int id)
    {
        return _client.DeleteAsync($"api/agreements/{id}");
    }

    #endregion

    #region Query Operations

    /// <inheritdoc/>
    public async Task<List<AgreementSummary>> GetByCustomerAsync(int customerId, int take = 100)
    {
        var response = await _client.GetAsync<ApiV2ListResponse<AgreementDetail>>($"api/agreements/by-customer/{customerId}?pageSize={take}");

        return response.Data?.Select(d => new AgreementSummary
        {
            Id = d.Id,
            Title = d.Title,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            CustomerId = d.CustomerId,
            Description = d.Description
        }).ToList() ?? new List<AgreementSummary>();
    }

    /// <inheritdoc/>
    public async Task<List<AgreementSummary>> GetByDateRangeAsync(DateTime start, DateTime end, int take = 100)
    {
        var startStr = HttpUtility.UrlEncode(start.ToString("o"));
        var endStr = HttpUtility.UrlEncode(end.ToString("o"));
        var response = await _client.GetAsync<ApiV2ListResponse<AgreementDetail>>($"api/agreements/by-date-range?startDate={startStr}&endDate={endStr}&pageSize={take}");

        return response.Data?.Select(d => new AgreementSummary
        {
            Id = d.Id,
            Title = d.Title,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            CustomerId = d.CustomerId,
            Description = d.Description
        }).ToList() ?? new List<AgreementSummary>();
    }

    #endregion
}
