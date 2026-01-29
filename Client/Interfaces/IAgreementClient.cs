using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Agreements;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for agreement/appointment management operations.
/// </summary>
public interface IAgreementClient
{
    #region CRUD Operations

    /// <summary>
    /// Gets all agreements with optional filtering.
    /// </summary>
    /// <param name="query">Optional query parameters.</param>
    /// <returns>List of agreement summaries.</returns>
    Task<List<AgreementSummary>> GetAllAsync(AgreementQueryParams? query = null);

    /// <summary>
    /// Gets an agreement by ID.
    /// </summary>
    /// <param name="id">Agreement ID.</param>
    /// <returns>Agreement details.</returns>
    Task<AgreementDetail> GetAsync(int id);

    /// <summary>
    /// Creates a new agreement.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>The created agreement.</returns>
    Task<AgreementDetail> CreateAsync(CreateAgreementRequest request);

    /// <summary>
    /// Updates an existing agreement.
    /// </summary>
    /// <param name="id">Agreement ID.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The updated agreement.</returns>
    Task<AgreementDetail> UpdateAsync(int id, UpdateAgreementRequest request);

    /// <summary>
    /// Deletes an agreement by ID.
    /// </summary>
    /// <param name="id">Agreement ID to delete.</param>
    Task DeleteAsync(int id);

    #endregion

    #region Query Operations

    /// <summary>
    /// Gets agreements for a specific customer.
    /// </summary>
    /// <param name="customerId">Customer ID.</param>
    /// <param name="take">Maximum results.</param>
    /// <returns>List of agreements.</returns>
    Task<List<AgreementSummary>> GetByCustomerAsync(int customerId, int take = 100);

    /// <summary>
    /// Gets agreements within a date range.
    /// </summary>
    /// <param name="start">Start date.</param>
    /// <param name="end">End date.</param>
    /// <param name="take">Maximum results.</param>
    /// <returns>List of agreements.</returns>
    Task<List<AgreementSummary>> GetByDateRangeAsync(DateTime start, DateTime end, int take = 100);

    #endregion
}
