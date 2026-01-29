using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Finance;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for finance resources including invoices and billing.
/// </summary>
public class FinanceApi : ApiModuleBase
{
    protected override string ModulePath => "finance";

    /// <summary>
    /// Access to invoice resources.
    /// </summary>
    public InvoicesResource Invoices { get; }

    public FinanceApi(IHttpClient http) : base(http)
    {
        Invoices = new InvoicesResource(http, this);
    }

    #region Invoices Resource

    /// <summary>
    /// Resource client for invoice operations.
    /// Uses cache pattern (GetKeys + GetRange).
    /// </summary>
    public class InvoicesResource
    {
        private readonly IHttpClient _http;
        private readonly FinanceApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/invoices";

        internal InvoicesResource(IHttpClient http, FinanceApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets invoices using the cache pattern.
        /// </summary>
        public Task<List<InvoiceViewModel>> GetAllAsync(int limit = 20)
        {
            var filter = new RequestFilter { Take = limit };
            return FetchViaKeysAndCacheAsync(filter);
        }

        /// <summary>
        /// Gets invoices for a specific period.
        /// </summary>
        public Task<List<InvoiceViewModel>> GetByPeriodAsync(string startsAt, string endsAt, int limit = 100)
        {
            var filter = new RequestFilter
            {
                Take = limit,
                Period = new PeriodModel { StartsAt = startsAt, EndsAt = endsAt }
            };
            return FetchViaKeysAndCacheAsync(filter);
        }

        /// <summary>
        /// Gets invoices for a specific customer.
        /// </summary>
        public async Task<List<InvoiceViewModel>> GetByCustomerAsync(int customerId, int limit = 100)
        {
            var keys = await _http.PostAsync<List<int>>(
                $"{BasePath}/GetKeys",
                new { customerId, take = limit });

            if (keys == null || !keys.Any())
                return new List<InvoiceViewModel>();

            return await GetRangeAsync(keys);
        }

        /// <summary>
        /// Gets invoices by their IDs.
        /// </summary>
        public async Task<List<InvoiceViewModel>> GetRangeAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<InvoiceViewModel>();

            var cacheItems = await _http.PostAsync<List<CacheDataItem<InvoiceViewModel>>>(
                $"{BasePath}/GetRange", ids);

            return cacheItems
                .Where(x => x.Data != null)
                .Select(x => x.Data!)
                .ToList();
        }

        /// <summary>
        /// Gets a single invoice by ID.
        /// </summary>
        public Task<InvoiceViewModel> GetAsync(int id)
        {
            return _http.GetAsync<InvoiceViewModel>($"{BasePath}/{id}");
        }

        private async Task<List<InvoiceViewModel>> FetchViaKeysAndCacheAsync(RequestFilter filter)
        {
            var keys = await _http.PostAsync<List<int>>($"{BasePath}/GetKeys", filter);

            if (keys == null || !keys.Any())
                return new List<InvoiceViewModel>();

            return await GetRangeAsync(keys);
        }
    }

    #endregion
}
