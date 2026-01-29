using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Customers;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for customer management operations.
/// </summary>
public class CustomerClient : ICustomerClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public CustomerClient(IServedClient client)
    {
        _client = client;
    }

    #region CRUD Operations

    /// <inheritdoc/>
    public async Task<List<CustomerSummary>> GetAllAsync(CustomerQueryParams? query = null)
    {
        // Use GetKeys + GetRange pattern to fetch customers
        var q = query ?? new CustomerQueryParams();
        var keys = await _client.PostAsync<List<int>>("api/calendar/customer/GetKeys", q);
        if (keys == null || keys.Count == 0)
            return new List<CustomerSummary>();

        // Limit keys if Take was specified
        if (q.Take.HasValue && keys.Count > q.Take.Value)
            keys = keys.Take(q.Take.Value).ToList();

        var details = await GetRangeAsync(keys);
        return details.Select(d => new CustomerSummary
        {
            Id = d.Id,
            Name = d.Name,
            CustomerNo = d.CustomerNo,
            Email = d.Email,
            Phone = d.Phone,
            IsActive = d.IsActive
        }).ToList();
    }

    /// <inheritdoc/>
    public Task<CustomerDetail> GetAsync(int id)
    {
        return _client.GetAsync<CustomerDetail>($"api/calendar/customer/Get?id={id}");
    }

    /// <inheritdoc/>
    public async Task<CustomerDetail> CreateAsync(CreateCustomerRequest request)
    {
        var result = await _client.PostAsync<int>("api/calendar/customer/Create", request);
        return await GetAsync(result);
    }

    /// <inheritdoc/>
    public Task<CustomerDetail> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var updateModel = new
        {
            id = id,
            name = request.Name,
            firstName = request.FirstName,
            lastName = request.LastName,
            email = request.Email,
            phone = request.Phone,
            website = request.Website,
            vatNumber = request.VatNumber,
            address = request.Address,
            city = request.City,
            postalCode = request.PostalCode,
            country = request.Country,
            isActive = request.IsActive,
            notes = request.Notes,
            paymentTermsDays = request.PaymentTermsDays,
            defaultHourlyRate = request.DefaultHourlyRate
        };
        return _client.PostAsync<CustomerDetail>("api/calendar/customer/Update", updateModel);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(int id)
    {
        return _client.DeleteAsync($"api/calendar/customer/Delete?customerId={id}");
    }

    #endregion

    #region Bulk Operations

    /// <inheritdoc/>
    public Task<BulkResponse<CustomerDetail>> CreateBulkAsync(BulkCreateCustomersRequest request)
    {
        return _client.PostAsync<BulkResponse<CustomerDetail>>("api/customers/bulk", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<CustomerDetail>> UpdateBulkAsync(BulkUpdateCustomersRequest request)
    {
        return _client.PutAsync<BulkResponse<CustomerDetail>>("api/customers/bulk", request);
    }

    /// <inheritdoc/>
    public Task<BulkResponse<CustomerDetail>> DeleteBulkAsync(BulkDeleteCustomersRequest request)
    {
        return _client.DeleteWithBodyAsync<BulkResponse<CustomerDetail>>("api/customers/bulk", request);
    }

    #endregion

    #region Query Operations

    /// <inheritdoc/>
    public async Task<List<CustomerDetail>> GetRangeAsync(List<int> ids)
    {
        var result = await _client.PostAsync<List<CacheDataItem<CustomerDetail>>>(
            "api/cache/CustomerProvider/GetRange", ids);

        var list = new List<CustomerDetail>();
        if (result != null)
        {
            foreach (var item in result)
            {
                if (item.Data != null) list.Add(item.Data);
            }
        }
        return list;
    }

    /// <inheritdoc/>
    public async Task<List<CustomerSummary>> SearchAsync(string searchTerm, int take = 20)
    {
        var query = new CustomerQueryParams
        {
            Search = searchTerm,
            Take = take,
            IsActive = true
        };
        return await GetAllAsync(query);
    }

    #endregion
}
