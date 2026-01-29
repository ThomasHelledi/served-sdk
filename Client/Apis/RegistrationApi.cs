using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.TimeRegistration;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for time registration resources.
/// </summary>
public class RegistrationApi : ApiModuleBase
{
    protected override string ModulePath => "registration";

    /// <summary>
    /// Access to time registration resources.
    /// </summary>
    public TimeRegistrationsResource TimeRegistrations { get; }

    public RegistrationApi(IHttpClient http) : base(http)
    {
        TimeRegistrations = new TimeRegistrationsResource(http, this);
    }

    #region TimeRegistrations Resource

    /// <summary>
    /// Resource client for time registration operations.
    /// </summary>
    public class TimeRegistrationsResource : BulkApiClientBase<
        TimeRegistrationSummary,
        TimeRegistrationDetail,
        CreateTimeRegistrationRequest,
        UpdateTimeRegistrationRequest,
        TimeRegistrationQueryParams,
        BulkCreateTimeRegistrationsRequest,
        BulkUpdateTimeRegistrationsRequest,
        BulkDeleteTimeRegistrationsRequest>
    {
        private readonly RegistrationApi _module;

        // Use legacy path for backwards compatibility
        protected override string BasePath => "api/timeregistrations";

        internal TimeRegistrationsResource(IHttpClient http, RegistrationApi module) : base(http)
        {
            _module = module;
        }

        protected override IEnumerable<string> GetCustomQueryParams(TimeRegistrationQueryParams query)
        {
            var @params = new List<string>();

            if (query.ProjectId.HasValue)
                @params.Add($"projectId={query.ProjectId.Value}");
            if (query.TaskId.HasValue)
                @params.Add($"taskId={query.TaskId.Value}");
            if (query.EmployeeId.HasValue)
                @params.Add($"employeeId={query.EmployeeId.Value}");
            if (!string.IsNullOrEmpty(query.StartDate))
                @params.Add($"startDate={query.StartDate}");
            if (!string.IsNullOrEmpty(query.EndDate))
                @params.Add($"endDate={query.EndDate}");

            return @params;
        }

        protected override List<TimeRegistrationSummary>? MapToEntityList(List<TimeRegistrationDetail>? details)
        {
            return details?.Select(d => new TimeRegistrationSummary
            {
                Id = d.Id,
                Date = d.Date,
                Hours = d.Hours,
                Description = d.Description,
                ProjectId = d.ProjectId,
                ProjectName = d.ProjectName,
                TaskId = d.TaskId,
                TaskName = d.TaskName,
                EmployeeId = d.EmployeeId,
                EmployeeName = d.EmployeeName
            }).ToList();
        }

        /// <summary>
        /// Gets time registrations for a specific project.
        /// </summary>
        public Task<List<TimeRegistrationSummary>> GetByProjectAsync(int projectId, int take = 100)
        {
            return GetAllAsync(new TimeRegistrationQueryParams { ProjectId = projectId, Take = take });
        }

        /// <summary>
        /// Gets time registrations for a specific task.
        /// </summary>
        public Task<List<TimeRegistrationSummary>> GetByTaskAsync(int taskId, int take = 100)
        {
            return GetAllAsync(new TimeRegistrationQueryParams { TaskId = taskId, Take = take });
        }

        /// <summary>
        /// Gets time registrations for a specific employee.
        /// </summary>
        public Task<List<TimeRegistrationSummary>> GetByEmployeeAsync(int employeeId, int take = 100)
        {
            return GetAllAsync(new TimeRegistrationQueryParams { EmployeeId = employeeId, Take = take });
        }

        /// <summary>
        /// Gets time registrations for a date range.
        /// </summary>
        public Task<List<TimeRegistrationSummary>> GetByDateRangeAsync(string startDate, string endDate, int take = 100)
        {
            return GetAllAsync(new TimeRegistrationQueryParams
            {
                StartDate = startDate,
                EndDate = endDate,
                Take = take
            });
        }
    }

    #endregion
}
