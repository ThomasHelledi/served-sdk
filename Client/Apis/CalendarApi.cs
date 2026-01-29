using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Agreements;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for calendar resources including agreements/appointments.
/// </summary>
public class CalendarApi : ApiModuleBase
{
    protected override string ModulePath => "calendar";

    /// <summary>
    /// Access to agreement/appointment resources.
    /// </summary>
    public AgreementsResource Agreements { get; }

    public CalendarApi(IHttpClient http) : base(http)
    {
        Agreements = new AgreementsResource(http, this);
    }

    #region Agreements Resource

    /// <summary>
    /// Resource client for agreement/appointment operations.
    /// </summary>
    public class AgreementsResource : BulkApiClientBase<
        AgreementSummary,
        AgreementDetail,
        CreateAgreementRequest,
        UpdateAgreementRequest,
        AgreementQueryParams,
        BulkCreateAgreementsRequest,
        BulkUpdateAgreementsRequest,
        BulkDeleteAgreementsRequest>
    {
        private readonly CalendarApi _module;

        // Use legacy path for backwards compatibility
        protected override string BasePath => "api/agreements";

        internal AgreementsResource(IHttpClient http, CalendarApi module) : base(http)
        {
            _module = module;
        }

        protected override IEnumerable<string> GetCustomQueryParams(AgreementQueryParams query)
        {
            var @params = new List<string>();

            if (query.CustomerId.HasValue)
                @params.Add($"customerId={query.CustomerId.Value}");
            if (query.ProjectId.HasValue)
                @params.Add($"projectId={query.ProjectId.Value}");
            if (query.EmployeeId.HasValue)
                @params.Add($"employeeId={query.EmployeeId.Value}");
            if (query.StartDate.HasValue)
                @params.Add($"startDate={query.StartDate.Value:o}");
            if (query.EndDate.HasValue)
                @params.Add($"endDate={query.EndDate.Value:o}");

            return @params;
        }

        protected override List<AgreementSummary>? MapToEntityList(List<AgreementDetail>? details)
        {
            return details?.Select(d => new AgreementSummary
            {
                Id = d.Id,
                Title = d.Title,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                CustomerId = d.CustomerId,
                CustomerName = d.CustomerName,
                ProjectId = d.ProjectId,
                ProjectName = d.ProjectName
            }).ToList();
        }

        /// <summary>
        /// Gets agreements for a date range.
        /// </summary>
        public Task<List<AgreementSummary>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int take = 100)
        {
            return GetAllAsync(new AgreementQueryParams
            {
                StartDate = startDate,
                EndDate = endDate,
                Take = take
            });
        }

        /// <summary>
        /// Gets agreements for a specific customer.
        /// </summary>
        public Task<List<AgreementSummary>> GetByCustomerAsync(int customerId, int take = 100)
        {
            return GetAllAsync(new AgreementQueryParams { CustomerId = customerId, Take = take });
        }

        /// <summary>
        /// Gets agreements for a specific employee.
        /// </summary>
        public Task<List<AgreementSummary>> GetByEmployeeAsync(int employeeId, int take = 100)
        {
            return GetAllAsync(new AgreementQueryParams { EmployeeId = employeeId, Take = take });
        }
    }

    #endregion
}
