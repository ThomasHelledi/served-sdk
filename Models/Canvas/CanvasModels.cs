using System;
using System.Collections.Generic;

namespace Served.SDK.Models.Canvas;

#region Enums

/// <summary>
/// Storage mode for canvas data
/// </summary>
public enum CanvasStorageMode
{
    Database = 0,
    Synced = 1,
    GitLab = 2
}

/// <summary>
/// Type of canvas node
/// </summary>
public enum CanvasNodeType
{
    Text = 0,
    File = 1,
    Link = 2,
    Group = 3,
    Entity = 4
}

/// <summary>
/// Side of a node that an edge connects to
/// </summary>
public enum CanvasEdgeSide
{
    Top = 0,
    Right = 1,
    Bottom = 2,
    Left = 3
}

/// <summary>
/// End style for canvas edges
/// </summary>
public enum CanvasEdgeEnd
{
    None = 0,
    Arrow = 1
}

#endregion

#region Canvas ViewModels

/// <summary>
/// Canvas list item for workspace overview
/// </summary>
public class CanvasListViewModel
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ColorTheme { get; set; }
    public CanvasStorageMode StorageMode { get; set; }
    public bool IsPersonal { get; set; }
    public bool IsTemplate { get; set; }
    public bool IsArchived { get; set; }
    public bool IsPinned { get; set; }
    public int? ParentFolderId { get; set; }
    public string? ParentFolderName { get; set; }
    public int SortOrder { get; set; }
    public int NodeCount { get; set; }
    public int EdgeCount { get; set; }
    public Guid? ThumbnailGuid { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

/// <summary>
/// Full canvas detail with nodes and edges
/// </summary>
public class CanvasDetailViewModel
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ColorTheme { get; set; }
    public CanvasStorageMode StorageMode { get; set; }
    public string? GitLabPath { get; set; }
    public string? SyncedFilePath { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public double ViewportX { get; set; }
    public double ViewportY { get; set; }
    public double ViewportZoom { get; set; }
    public bool IsPersonal { get; set; }
    public bool IsTemplate { get; set; }
    public bool IsArchived { get; set; }
    public bool IsPinned { get; set; }
    public List<CanvasNodeViewModel> Nodes { get; set; } = new();
    public List<CanvasEdgeViewModel> Edges { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int CreatedById { get; set; }
    public string? CreatedByName { get; set; }
}

/// <summary>
/// Canvas node
/// </summary>
public class CanvasNodeViewModel
{
    public string Id { get; set; } = "";
    public CanvasNodeType Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int ZIndex { get; set; }
    public string? Color { get; set; }
    public string? TextContent { get; set; }
    public string? FilePath { get; set; }
    public string? FileSubpath { get; set; }
    public string? LinkUrl { get; set; }
    public string? GroupLabel { get; set; }
    public string? GroupBackground { get; set; }
    public string? GroupBackgroundStyle { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? EntityDisplayMode { get; set; }
    public List<string>? EntityShowFields { get; set; }
    public bool? EntityAllowEdit { get; set; }
    public bool? EntityLiveSync { get; set; }
}

/// <summary>
/// Canvas edge (connection between nodes)
/// </summary>
public class CanvasEdgeViewModel
{
    public string Id { get; set; } = "";
    public string FromNodeId { get; set; } = "";
    public CanvasEdgeSide? FromSide { get; set; }
    public CanvasEdgeEnd? FromEnd { get; set; }
    public string ToNodeId { get; set; } = "";
    public CanvasEdgeSide? ToSide { get; set; }
    public CanvasEdgeEnd? ToEnd { get; set; }
    public string? Color { get; set; }
    public string? Label { get; set; }
}

#endregion

#region Request Models

/// <summary>
/// Create canvas request
/// </summary>
public class CanvasCreateRequest
{
    public int WorkspaceId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ColorTheme { get; set; }
    public int? ParentFolderId { get; set; }
    public bool IsPersonal { get; set; }
    public bool IsTemplate { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Update canvas request
/// </summary>
public class CanvasUpdateRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ColorTheme { get; set; }
    public int? ParentFolderId { get; set; }
    public bool? IsPersonal { get; set; }
    public bool? IsArchived { get; set; }
    public bool? IsPinned { get; set; }
    public int? SortOrder { get; set; }
    public double? ViewportX { get; set; }
    public double? ViewportY { get; set; }
    public double? ViewportZoom { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Canvas content (nodes and edges) for bulk update
/// </summary>
public class CanvasContentRequest
{
    public List<CanvasNodeViewModel> Nodes { get; set; } = new();
    public List<CanvasEdgeViewModel> Edges { get; set; } = new();
}

/// <summary>
/// Create node request
/// </summary>
public class CanvasNodeCreateRequest
{
    public CanvasNodeType Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 300;
    public double Height { get; set; } = 200;
    public string? Color { get; set; }
    public string? TextContent { get; set; }
    public string? FilePath { get; set; }
    public string? LinkUrl { get; set; }
    public string? GroupLabel { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? EntityDisplayMode { get; set; }
    public List<string>? EntityShowFields { get; set; }
}

/// <summary>
/// Update node request
/// </summary>
public class CanvasNodeUpdateRequest
{
    public double? X { get; set; }
    public double? Y { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public int? ZIndex { get; set; }
    public string? Color { get; set; }
    public string? TextContent { get; set; }
    public string? GroupLabel { get; set; }
    public string? EntityDisplayMode { get; set; }
    public List<string>? EntityShowFields { get; set; }
}

/// <summary>
/// Node position for bulk update
/// </summary>
public class NodePositionRequest
{
    public string NodeId { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
}

/// <summary>
/// Create edge request
/// </summary>
public class CanvasEdgeCreateRequest
{
    public string FromNodeId { get; set; } = "";
    public CanvasEdgeSide? FromSide { get; set; }
    public CanvasEdgeEnd? FromEnd { get; set; }
    public string ToNodeId { get; set; } = "";
    public CanvasEdgeSide? ToSide { get; set; }
    public CanvasEdgeEnd? ToEnd { get; set; }
    public string? Color { get; set; }
    public string? Label { get; set; }
}

/// <summary>
/// Update edge request
/// </summary>
public class CanvasEdgeUpdateRequest
{
    public CanvasEdgeSide? FromSide { get; set; }
    public CanvasEdgeEnd? FromEnd { get; set; }
    public CanvasEdgeSide? ToSide { get; set; }
    public CanvasEdgeEnd? ToEnd { get; set; }
    public string? Color { get; set; }
    public string? Label { get; set; }
}

#endregion
