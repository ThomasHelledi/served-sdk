using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Served.SDK.Models.Common;

namespace Served.SDK.Models.Users;

public class UserBootstrapViewModel
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IEnumerable<TenantViewModel> Tenants { get; set; } = new List<TenantViewModel>();
    public IEnumerable<WorkspaceViewModel> Workspaces { get; set; } = new List<WorkspaceViewModel>();
}

public class EmployeeListViewModel
{
    [JsonProperty("employeeId")]
    public int EmployeeId { get; set; }
    
    [JsonProperty("firstName")]
    public string? FirstName { get; set; }
    
    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    public string? Initials { get; set; }
    public string? JobTitle { get; set; }
}

public class DetailedEmployeeModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int DepartmentId { get; set; }
}

#region Employee Models for New API

/// <summary>
/// Summary employee view for listings.
/// </summary>
public class EmployeeSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// Detailed employee view.
/// </summary>
public class EmployeeDetail : EmployeeSummary
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? Initials { get; set; }
    public Guid? Logo { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Query parameters for employees.
/// </summary>
public class EmployeeQueryParams : QueryParams
{
    /// <summary>Filter by active status.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Filter by department ID.</summary>
    public int? DepartmentId { get; set; }
}

#endregion
