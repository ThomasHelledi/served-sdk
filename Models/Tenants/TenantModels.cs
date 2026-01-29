using System;

namespace Served.SDK.Models.Tenants;

/// <summary>
/// Summary information for a tenant.
/// </summary>
public class TenantSummary
{
    /// <summary>
    /// Tenant ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tenant name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tenant slug (URL-friendly identifier).
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Tenant logo GUID.
    /// </summary>
    public Guid? Logo { get; set; }

    /// <summary>
    /// Date when the tenant was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }
}

/// <summary>
/// Detailed tenant information.
/// </summary>
public class TenantDetail : TenantSummary
{
    /// <summary>
    /// Tenant description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Billing email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Last activity timestamp.
    /// </summary>
    public DateTime LastActivity { get; set; }
}

/// <summary>
/// Request to create a new tenant.
/// </summary>
public class CreateTenantRequest
{
    /// <summary>
    /// Tenant name (required, max 255 characters).
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Tenant slug (required, max 16 characters, lowercase alphanumeric and hyphens only).
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// Tenant logo GUID (optional).
    /// </summary>
    public Guid? Logo { get; set; }

    /// <summary>
    /// Billing email address (optional).
    /// </summary>
    public string? BillingEmail { get; set; }
}

/// <summary>
/// Request to update an existing tenant.
/// </summary>
public class UpdateTenantRequest : CreateTenantRequest
{
    /// <summary>
    /// Tenant ID to update.
    /// </summary>
    public int TenantId { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }
}

#region Workspace Models

/// <summary>
/// Summary information for a workspace.
/// </summary>
public class WorkspaceSummary
{
    /// <summary>Workspace ID.</summary>
    public int Id { get; set; }

    /// <summary>Workspace name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Workspace slug.</summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>Workspace type.</summary>
    public string WorkspaceType { get; set; } = string.Empty;
}

/// <summary>
/// Detailed workspace information.
/// </summary>
public class WorkspaceDetail : WorkspaceSummary
{
    /// <summary>Tenant ID this workspace belongs to.</summary>
    public int TenantId { get; set; }

    /// <summary>Workspace description.</summary>
    public string? Description { get; set; }

    /// <summary>Workspace icon.</summary>
    public string? Icon { get; set; }

    /// <summary>Workspace color.</summary>
    public string? Color { get; set; }

    /// <summary>Whether the workspace is active.</summary>
    public bool IsActive { get; set; }

    /// <summary>Date when the workspace was created.</summary>
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Request to create a new workspace.
/// </summary>
public class CreateWorkspaceRequest
{
    /// <summary>Workspace name (required).</summary>
    public required string Name { get; set; }

    /// <summary>Workspace slug (required).</summary>
    public required string Slug { get; set; }

    /// <summary>Workspace type.</summary>
    public string WorkspaceType { get; set; } = "default";

    /// <summary>Workspace description.</summary>
    public string? Description { get; set; }

    /// <summary>Workspace icon.</summary>
    public string? Icon { get; set; }

    /// <summary>Workspace color.</summary>
    public string? Color { get; set; }
}

/// <summary>
/// Request to update a workspace.
/// </summary>
public class UpdateWorkspaceRequest
{
    /// <summary>Workspace name.</summary>
    public string? Name { get; set; }

    /// <summary>Workspace description.</summary>
    public string? Description { get; set; }

    /// <summary>Workspace icon.</summary>
    public string? Icon { get; set; }

    /// <summary>Workspace color.</summary>
    public string? Color { get; set; }

    /// <summary>Whether the workspace is active.</summary>
    public bool? IsActive { get; set; }
}

#endregion
