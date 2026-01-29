using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Customers;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for company resources including customers.
/// </summary>
public class CompaniesApi : ApiModuleBase
{
    protected override string ModulePath => "companies";

    /// <summary>
    /// Access to customer resources.
    /// </summary>
    public CustomersResource Customers { get; }

    public CompaniesApi(IHttpClient http) : base(http)
    {
        Customers = new CustomersResource(http, this);
    }

    #region Customers Resource

    /// <summary>
    /// Resource client for customer operations.
    /// </summary>
    public class CustomersResource : BulkApiClientBase<
        CustomerSummary,
        CustomerDetail,
        CreateCustomerRequest,
        UpdateCustomerRequest,
        CustomerQueryParams,
        BulkCreateCustomersRequest,
        BulkUpdateCustomersRequest,
        BulkDeleteCustomersRequest>
    {
        private readonly CompaniesApi _module;

        // Use legacy path for backwards compatibility
        protected override string BasePath => "api/customers";

        internal CustomersResource(IHttpClient http, CompaniesApi module) : base(http)
        {
            _module = module;
        }

        protected override IEnumerable<string> GetCustomQueryParams(CustomerQueryParams query)
        {
            var @params = new List<string>();

            if (query.IsActive.HasValue)
                @params.Add($"isActive={query.IsActive.Value}");
            if (query.CustomerTypeId.HasValue)
                @params.Add($"customerTypeId={query.CustomerTypeId.Value}");

            return @params;
        }

        protected override List<CustomerSummary>? MapToEntityList(List<CustomerDetail>? details)
        {
            return details?.Select(d => new CustomerSummary
            {
                Id = d.Id,
                Name = d.Name,
                CustomerNo = d.CustomerNo,
                Email = d.Email,
                Phone = d.Phone,
                IsActive = d.IsActive
            }).ToList();
        }

        /// <summary>
        /// Searches customers by term.
        /// </summary>
        public Task<List<CustomerSummary>> SearchAsync(string searchTerm, int take = 20)
        {
            return GetAllAsync(new CustomerQueryParams
            {
                Search = searchTerm,
                Take = take,
                IsActive = true
            });
        }

        /// <summary>
        /// Gets multiple customers by IDs.
        /// </summary>
        public async Task<List<CustomerDetail>> GetRangeAsync(List<int> ids)
        {
            var customers = new List<CustomerDetail>();
            foreach (var id in ids)
            {
                try
                {
                    var customer = await GetAsync(id);
                    if (customer != null)
                        customers.Add(customer);
                }
                catch
                {
                    // Skip customers that fail to load
                }
            }
            return customers;
        }
    }

    #endregion
}
