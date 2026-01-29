using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Finance;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for finance and invoice operations.
/// </summary>
public class FinanceClient : IFinanceClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinanceClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public FinanceClient(IServedClient client)
    {
        _client = client;
    }

    /// <inheritdoc/>
    public Task<List<InvoiceViewModel>> GetInvoicesAsync(int limit = 20)
    {
        var filter = new RequestFilter { Take = limit };
        return ClientHelpers.FetchViaKeysAndCache<InvoiceViewModel>(_client,
            "api/finance/Invoice/GetKeys",
            "api/finance/Invoice/GetRange",
            filter);
    }
}
