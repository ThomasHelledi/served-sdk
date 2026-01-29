using System;
using System.Collections.Generic;
using Served.SDK.Models.Tenants;

namespace Served.SDK.Models.Bootstrap;

/// <summary>
/// User bootstrap data returned on authentication.
/// </summary>
public class UserBootstrap
{
    /// <summary>
    /// The authenticated user's ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// User first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// User last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// User initials.
    /// </summary>
    public string? Initials { get; set; }

    /// <summary>
    /// User logo/avatar GUID.
    /// </summary>
    public Guid? Logo { get; set; }

    /// <summary>
    /// Date when the user account was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Last activity timestamp.
    /// </summary>
    public DateTime? LastActivity { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// User preferences (key-value pairs).
    /// </summary>
    public Dictionary<string, string>? Preferences { get; set; }

    /// <summary>
    /// System-level permissions.
    /// </summary>
    public Dictionary<string, bool>? SystemPermissions { get; set; }

    /// <summary>
    /// Tenants the user has access to.
    /// </summary>
    public List<TenantSummary>? Tenants { get; set; }

    /// <summary>
    /// Workspaces the user has access to.
    /// </summary>
    public List<WorkspaceSummary>? Workspaces { get; set; }

    /// <summary>
    /// Physical locations the user has access to.
    /// </summary>
    public List<LocationSummary>? Locations { get; set; }
}

/// <summary>
/// Tenant bootstrap data for organization context.
/// </summary>
public class TenantBootstrap
{
    /// <summary>
    /// The tenant/organization data.
    /// </summary>
    public TenantSummary? Tenant { get; set; }

    /// <summary>
    /// Organization-level permissions.
    /// </summary>
    public List<PermissionSection>? Permissions { get; set; }

    /// <summary>
    /// Organization settings.
    /// </summary>
    public List<Setting>? Settings { get; set; }

    /// <summary>
    /// Enabled features.
    /// </summary>
    public List<Feature>? Features { get; set; }

    /// <summary>
    /// Employee list (limited for performance).
    /// </summary>
    public List<EmployeeSummary>? Employees { get; set; }

    /// <summary>
    /// Available boards.
    /// </summary>
    public List<BoardSummary>? Boards { get; set; }

    /// <summary>
    /// Category IDs for quick lookup.
    /// </summary>
    public CategoryKeys? CategoryKeys { get; set; }

    /// <summary>
    /// Available currencies.
    /// </summary>
    public List<CurrencySummary>? Currencies { get; set; }

    /// <summary>
    /// AI models configured for the tenant.
    /// </summary>
    public List<AiModelGroup>? AiModels { get; set; }

    /// <summary>
    /// Custom field sections.
    /// </summary>
    public List<CustomFieldSection>? CustomFieldSections { get; set; }

    /// <summary>
    /// Custom field definitions.
    /// </summary>
    public List<CustomFieldDefinition>? CustomFieldDefinitions { get; set; }
}

/// <summary>
/// Workspace bootstrap data.
/// </summary>
public class WorkspaceBootstrap : TenantBootstrap
{
    /// <summary>
    /// The workspace data.
    /// </summary>
    public WorkspaceSummary? Workspace { get; set; }

    /// <summary>
    /// Sales pipelines in the workspace.
    /// </summary>
    public List<PipelineSummary>? Pipelines { get; set; }
}

#region Supporting Types

/// <summary>
/// Workspace summary.
/// </summary>
public class WorkspaceSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? Logo { get; set; }
}

/// <summary>
/// Location summary.
/// </summary>
public class LocationSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Permission section with claims.
/// </summary>
public class PermissionSection
{
    public string SectionName { get; set; } = string.Empty;
    public Dictionary<string, bool>? Claims { get; set; }
}

/// <summary>
/// Organization setting.
/// </summary>
public class Setting
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}

/// <summary>
/// Enabled feature.
/// </summary>
public class Feature
{
    public string Key { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

/// <summary>
/// Employee summary.
/// </summary>
public class EmployeeSummary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Initials { get; set; }
    public Guid? Logo { get; set; }
}

/// <summary>
/// Board summary.
/// </summary>
public class BoardSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Category IDs for project/task types, statuses, etc.
/// </summary>
public class CategoryKeys
{
    public List<int>? ProjectTypes { get; set; }
    public List<int>? ProjectStatuses { get; set; }
    public List<int>? ProjectStages { get; set; }
    public List<int>? TaskTypes { get; set; }
    public List<int>? TaskStates { get; set; }
    public List<int>? TaskPriorities { get; set; }
    public List<int>? ProjectTags { get; set; }
    public List<int>? TaskTags { get; set; }
}

/// <summary>
/// Currency summary.
/// </summary>
public class CurrencySummary
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool IsDefault { get; set; }
    public bool IsEnabled { get; set; }
    public decimal CurrentRate { get; set; }
    public DateTime? RateDate { get; set; }
}

/// <summary>
/// AI model group (by provider/account).
/// </summary>
public class AiModelGroup
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string ProviderSlug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public List<AiModel>? Models { get; set; }
}

/// <summary>
/// AI model option.
/// </summary>
public class AiModel
{
    public int Id { get; set; }
    public string ModelId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool SupportsVision { get; set; }
    public bool SupportsFunctionCalling { get; set; }
}

/// <summary>
/// Custom field section.
/// </summary>
public class CustomFieldSection
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string DomainType { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

/// <summary>
/// Custom field definition.
/// </summary>
public class CustomFieldDefinition
{
    public int Id { get; set; }
    public string StringId { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = string.Empty;
    public string DomainType { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
}

/// <summary>
/// Pipeline summary.
/// </summary>
public class PipelineSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

#endregion

#region View Models for New API

/// <summary>
/// Tenant bootstrap view model.
/// </summary>
public class TenantBootstrapViewModel : TenantBootstrap
{
}

/// <summary>
/// Workspace bootstrap view model.
/// </summary>
public class WorkspaceBootstrapViewModel : WorkspaceBootstrap
{
}

/// <summary>
/// User permissions view model.
/// </summary>
public class UserPermissionsViewModel
{
    /// <summary>User ID.</summary>
    public int UserId { get; set; }

    /// <summary>System-level permissions.</summary>
    public Dictionary<string, bool>? SystemPermissions { get; set; }

    /// <summary>Tenant-specific permissions.</summary>
    public Dictionary<int, Dictionary<string, bool>>? TenantPermissions { get; set; }

    /// <summary>Workspace-specific permissions.</summary>
    public Dictionary<int, Dictionary<string, bool>>? WorkspacePermissions { get; set; }
}

#endregion
