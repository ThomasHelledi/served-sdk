using System;
using System.Collections.Generic;

namespace Served.SDK.Models.Boards;

#region Board Models

/// <summary>
/// Represents a dashboard/board.
/// </summary>
public class BoardViewModel
{
    public int Id { get; set; }
    public int Version { get; set; }
    public int? ParentId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<BoardViewViewModel>? Views { get; set; }
    public BoardConfigurationViewModel? Configuration { get; set; }
    public int? TenantId { get; set; }
}

/// <summary>
/// Board configuration settings.
/// </summary>
public class BoardConfigurationViewModel
{
    public int Id { get; set; }
    public string? ProjectsIds { get; set; }
    public int? DefaultViewId { get; set; }
    public string? SettingsJson { get; set; }
}

/// <summary>
/// A view within a board.
/// </summary>
public class BoardViewViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? ConfigurationJson { get; set; }
}

/// <summary>
/// Request to create a new board.
/// </summary>
public class CreateBoardRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public BoardConfigurationViewModel? Configuration { get; set; }
}

/// <summary>
/// Request to update a board.
/// </summary>
public class UpdateBoardRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public BoardConfigurationViewModel? Configuration { get; set; }
}

#endregion

#region Sheet Models

/// <summary>
/// Represents a spreadsheet-like data sheet.
/// </summary>
public class SheetViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<SheetColumnViewModel> Columns { get; set; } = new();
}

/// <summary>
/// Column definition for a sheet.
/// </summary>
public class SheetColumnViewModel
{
    public int Id { get; set; }
    public int SheetId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public SheetColumnType ColumnType { get; set; }
    public int? Width { get; set; }
    public int SortOrder { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationPattern { get; set; }
    public string? AllowedValuesJson { get; set; }
    public bool IsHidden { get; set; }
    public bool IsFrozen { get; set; }
    public bool IsAiSkillColumn { get; set; }
    public string? AiSkillConfigJson { get; set; }
}

/// <summary>
/// Column types for sheets.
/// </summary>
public enum SheetColumnType
{
    Text = 0,
    Number = 1,
    Date = 2,
    DateTime = 3,
    Boolean = 4,
    Select = 5,
    MultiSelect = 6,
    User = 7,
    Link = 8,
    Email = 9,
    Phone = 10,
    Currency = 11,
    Percent = 12,
    Rating = 13,
    Attachment = 14,
    Formula = 15,
    Lookup = 16,
    Rollup = 17,
    CreatedTime = 18,
    LastModifiedTime = 19,
    CreatedBy = 20,
    LastModifiedBy = 21,
    AutoNumber = 22,
    Barcode = 23,
    Button = 24,
    AiSkill = 25
}

/// <summary>
/// Request to create a new sheet.
/// </summary>
public class CreateSheetRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public List<CreateSheetColumnRequest>? Columns { get; set; }
}

/// <summary>
/// Request to create a sheet column.
/// </summary>
public class CreateSheetColumnRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public SheetColumnType ColumnType { get; set; }
    public int? Width { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationPattern { get; set; }
    public string? AllowedValuesJson { get; set; }
    public bool IsAiSkillColumn { get; set; }
    public string? AiSkillConfigJson { get; set; }
}

/// <summary>
/// Request to update a sheet.
/// </summary>
public class UpdateSheetRequest
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
}

/// <summary>
/// Represents a row in a sheet.
/// </summary>
public class SheetRowViewModel
{
    public int Id { get; set; }
    public int SheetId { get; set; }
    public int SortOrder { get; set; }
    public bool IsAiProcessing { get; set; }
    public DateTime? LastAiProcessedAt { get; set; }
    public List<SheetCellViewModel> Cells { get; set; } = new();
}

/// <summary>
/// Represents a cell in a sheet.
/// </summary>
public class SheetCellViewModel
{
    public int Id { get; set; }
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public string? TextValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateTime? DateValue { get; set; }
    public bool? BoolValue { get; set; }
    public string? JsonValue { get; set; }
    public bool IsAiGenerated { get; set; }
    public SheetCellAiStatus? AiStatus { get; set; }
    public string? AiErrorMessage { get; set; }
}

/// <summary>
/// AI processing status for a cell.
/// </summary>
public enum SheetCellAiStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

/// <summary>
/// Represents a view configuration for a sheet.
/// </summary>
public class SheetViewViewModel
{
    public int Id { get; set; }
    public int SheetId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsShared { get; set; }
    public int? OwnerId { get; set; }
    public string? ColumnConfigJson { get; set; }
    public string? FilterConfigJson { get; set; }
    public string? SortConfigJson { get; set; }
    public string? GroupConfigJson { get; set; }
}

/// <summary>
/// Request to create a sheet view.
/// </summary>
public class CreateSheetViewRequest
{
    public int SheetId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsShared { get; set; }
    public string? ColumnConfigJson { get; set; }
    public string? FilterConfigJson { get; set; }
    public string? SortConfigJson { get; set; }
    public string? GroupConfigJson { get; set; }
}

/// <summary>
/// Request to create a new row.
/// </summary>
public class CreateSheetRowRequest
{
    public int SheetId { get; set; }
    public Dictionary<int, object?> Values { get; set; } = new();
}

/// <summary>
/// Request to update a row.
/// </summary>
public class UpdateSheetRowRequest
{
    public int RowId { get; set; }
    public Dictionary<int, object?> Values { get; set; } = new();
}

/// <summary>
/// Request to update a single cell.
/// </summary>
public class UpdateCellRequest
{
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public object? Value { get; set; }
}

#endregion
