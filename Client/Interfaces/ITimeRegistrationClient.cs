using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.TimeRegistration;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for time registration operations.
/// </summary>
public interface ITimeRegistrationClient
{
    #region CRUD Operations

    /// <summary>
    /// Gets all time registrations with optional filtering.
    /// </summary>
    /// <param name="query">Optional query parameters.</param>
    /// <returns>List of time registration details.</returns>
    Task<List<TimeRegistrationDetail>> GetAllAsync(TimeRegistrationQueryParams? query = null);

    /// <summary>
    /// Gets a time registration by ID.
    /// </summary>
    /// <param name="id">Time registration ID.</param>
    /// <returns>Time registration details.</returns>
    Task<TimeRegistrationDetail> GetAsync(int id);

    /// <summary>
    /// Creates a new time registration.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>The created time registration.</returns>
    Task<TimeRegistrationDetail> CreateAsync(CreateTimeRegistrationRequest request);

    /// <summary>
    /// Updates an existing time registration.
    /// </summary>
    /// <param name="id">Time registration ID.</param>
    /// <param name="request">The update request.</param>
    /// <returns>The updated time registration.</returns>
    Task<TimeRegistrationDetail> UpdateAsync(int id, UpdateTimeRegistrationRequest request);

    /// <summary>
    /// Deletes a time registration by ID.
    /// </summary>
    /// <param name="id">Time registration ID to delete.</param>
    Task DeleteAsync(int id);

    #endregion

    #region Query Operations

    /// <summary>
    /// Gets time registrations for a date range.
    /// </summary>
    /// <param name="start">Start date.</param>
    /// <param name="end">End date.</param>
    /// <param name="take">Maximum results.</param>
    /// <returns>List of time registrations.</returns>
    Task<List<TimeRegistrationDetail>> GetByDateRangeAsync(DateTime start, DateTime end, int take = 50);

    /// <summary>
    /// Gets time registrations for a specific project.
    /// </summary>
    /// <param name="projectId">Project ID.</param>
    /// <param name="take">Maximum results.</param>
    /// <returns>List of time registrations.</returns>
    Task<List<TimeRegistrationDetail>> GetByProjectAsync(int projectId, int take = 50);

    /// <summary>
    /// Gets time registrations for a specific task.
    /// </summary>
    /// <param name="taskId">Task ID.</param>
    /// <param name="take">Maximum results.</param>
    /// <returns>List of time registrations.</returns>
    Task<List<TimeRegistrationDetail>> GetByTaskAsync(int taskId, int take = 50);

    #endregion
}
