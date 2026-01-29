using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Customers;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for customer management operations.
/// </summary>
public interface ICustomerClient
{
    #region CRUD Operations

    /// <summary>
    /// Gets all customers with optional filtering and pagination.
    /// </summary>
    /// <param name="query">Optional query parameters.</param>
    /// <returns>List of customer summaries.</returns>
    Task<List<CustomerSummary>> GetAllAsync(CustomerQueryParams? query = null);

    /// <summary>
    /// Gets a customer by ID.
    /// </summary>
    /// <param name="id">Customer ID.</param>
    /// <returns>Customer details.</returns>
    Task<CustomerDetail> GetAsync(int id);

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="request">The customer creation request.</param>
    /// <returns>The created customer.</returns>
    Task<CustomerDetail> CreateAsync(CreateCustomerRequest request);

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">Customer ID.</param>
    /// <param name="request">The customer update request.</param>
    /// <returns>The updated customer.</returns>
    Task<CustomerDetail> UpdateAsync(int id, UpdateCustomerRequest request);

    /// <summary>
    /// Deletes a customer by ID.
    /// </summary>
    /// <param name="id">The customer ID to delete.</param>
    Task DeleteAsync(int id);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Creates multiple customers in a single operation.
    /// </summary>
    /// <param name="request">Bulk creation request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<CustomerDetail>> CreateBulkAsync(BulkCreateCustomersRequest request);

    /// <summary>
    /// Updates multiple customers in a single operation.
    /// </summary>
    /// <param name="request">Bulk update request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<CustomerDetail>> UpdateBulkAsync(BulkUpdateCustomersRequest request);

    /// <summary>
    /// Deletes multiple customers in a single operation.
    /// </summary>
    /// <param name="request">Bulk delete request.</param>
    /// <returns>Bulk operation response.</returns>
    Task<BulkResponse<CustomerDetail>> DeleteBulkAsync(BulkDeleteCustomersRequest request);

    #endregion

    #region Query Operations

    /// <summary>
    /// Gets multiple customers by their IDs.
    /// </summary>
    /// <param name="ids">List of customer IDs to retrieve.</param>
    /// <returns>List of customer details.</returns>
    Task<List<CustomerDetail>> GetRangeAsync(List<int> ids);

    /// <summary>
    /// Searches customers by name or email.
    /// </summary>
    /// <param name="searchTerm">Search term.</param>
    /// <param name="take">Maximum results to return.</param>
    /// <returns>Matching customers.</returns>
    Task<List<CustomerSummary>> SearchAsync(string searchTerm, int take = 20);

    #endregion
}
