using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.TimeRegistration;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for time registration operations.
/// </summary>
public class TimeRegistrationClient : ITimeRegistrationClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeRegistrationClient"/> class.
    /// </summary>
    /// <param name="client">The Served client instance.</param>
    public TimeRegistrationClient(IServedClient client)
    {
        _client = client;
    }

    #region CRUD Operations

    /// <inheritdoc/>
    public Task<List<TimeRegistrationDetail>> GetAllAsync(TimeRegistrationQueryParams? query = null)
    {
        var q = query ?? new TimeRegistrationQueryParams();
        // Use RequestFilter format that the API expects
        var filter = new RequestFilter
        {
            Period = q.Start.HasValue && q.End.HasValue
                ? PeriodModel.FromDates(q.Start.Value, q.End.Value)
                : null,
            Skip = q.Skip ?? 0,
            Take = q.Take
        };
        return _client.PostAsync<List<TimeRegistrationDetail>>("api/registration/time-registration/Get", filter);
    }

    /// <inheritdoc/>
    public Task<TimeRegistrationDetail> GetAsync(int id)
    {
        return _client.PostAsync<TimeRegistrationDetail>($"api/registration/time-registration/Get?id={id}", new { });
    }

    /// <inheritdoc/>
    public Task<TimeRegistrationDetail> CreateAsync(CreateTimeRegistrationRequest request)
    {
        return _client.PostAsync<TimeRegistrationDetail>("api/registration/time-registration/Save", request);
    }

    /// <inheritdoc/>
    public Task<TimeRegistrationDetail> UpdateAsync(int id, UpdateTimeRegistrationRequest request)
    {
        var saveModel = new
        {
            id = id,
            start = request.Start,
            end = request.End,
            taskId = request.TaskId,
            projectId = request.ProjectId,
            description = request.Description,
            billable = request.Billable,
            minutes = request.Minutes
        };
        return _client.PostAsync<TimeRegistrationDetail>("api/registration/time-registration/Save", saveModel);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(int id)
    {
        var req = new DeleteRequest
        {
            TenantId = 1,
            Items = new List<DeleteItem>
            {
                new DeleteItem { Id = id, Version = 1, DomainType = (int)DomainType.TimeRegistration }
            }
        };
        return _client.DeleteWithBodyAsync("api/registration/time-registration/Delete", req);
    }

    #endregion

    #region Query Operations

    /// <inheritdoc/>
    public Task<List<TimeRegistrationDetail>> GetByDateRangeAsync(DateTime start, DateTime end, int take = 50)
    {
        var query = new TimeRegistrationQueryParams
        {
            Start = start,
            End = end,
            Take = take
        };
        return GetAllAsync(query);
    }

    /// <inheritdoc/>
    public Task<List<TimeRegistrationDetail>> GetByProjectAsync(int projectId, int take = 50)
    {
        var query = new TimeRegistrationQueryParams
        {
            ProjectId = projectId,
            Take = take
        };
        return GetAllAsync(query);
    }

    /// <inheritdoc/>
    public Task<List<TimeRegistrationDetail>> GetByTaskAsync(int taskId, int take = 50)
    {
        var query = new TimeRegistrationQueryParams
        {
            TaskId = taskId,
            Take = take
        };
        return GetAllAsync(query);
    }

    #endregion
}
