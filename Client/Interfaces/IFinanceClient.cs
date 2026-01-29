using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Finance;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for finance and invoice operations.
/// </summary>
public interface IFinanceClient
{
    /// <summary>
    /// Gets a list of invoices.
    /// </summary>
    /// <param name="limit">Maximum number of invoices to return (default: 20).</param>
    /// <returns>List of invoice view models.</returns>
    Task<List<InvoiceViewModel>> GetInvoicesAsync(int limit = 20);
}
