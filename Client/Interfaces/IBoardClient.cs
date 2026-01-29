using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Models.Board;

namespace Served.SDK.Client.Interfaces;

/// <summary>
/// Interface for Board and Sheet operations.
/// </summary>
public interface IBoardClient
{
    #region Boards

    /// <summary>
    /// Gets all boards for the tenant.
    /// </summary>
    Task<List<BoardViewModel>> GetBoardsAsync();

    /// <summary>
    /// Gets a board by ID.
    /// </summary>
    Task<BoardViewModel?> GetBoardAsync(int id);

    /// <summary>
    /// Gets board keys for the tenant.
    /// </summary>
    Task<List<int>> GetBoardKeysAsync();

    /// <summary>
    /// Creates a new board.
    /// </summary>
    Task<BoardViewModel> CreateBoardAsync(BoardViewModel board);

    /// <summary>
    /// Updates a board.
    /// </summary>
    Task UpdateBoardAsync(BoardViewModel board);

    /// <summary>
    /// Deletes a board.
    /// </summary>
    Task DeleteBoardAsync(int id);

    #endregion

    #region Sheets

    /// <summary>
    /// Gets all sheets for the tenant.
    /// </summary>
    Task<List<SheetViewModel>> GetSheetsAsync();

    /// <summary>
    /// Gets a sheet by ID.
    /// </summary>
    Task<SheetViewModel?> GetSheetAsync(int id);

    /// <summary>
    /// Gets a sheet by claim ID.
    /// </summary>
    Task<SheetViewModel?> GetSheetByClaimIdAsync(Guid claimId);

    /// <summary>
    /// Creates a new sheet.
    /// </summary>
    Task<SheetViewModel> CreateSheetAsync(CreateSheetRequest request);

    /// <summary>
    /// Updates a sheet.
    /// </summary>
    Task<SheetViewModel> UpdateSheetAsync(UpdateSheetRequest request);

    /// <summary>
    /// Deletes a sheet.
    /// </summary>
    Task DeleteSheetAsync(int id);

    #endregion

    #region Sheet Columns

    /// <summary>
    /// Gets columns for a sheet.
    /// </summary>
    Task<List<SheetColumnViewModel>> GetColumnsAsync(int sheetId);

    /// <summary>
    /// Adds a column to a sheet.
    /// </summary>
    Task<SheetColumnViewModel> AddColumnAsync(int sheetId, CreateSheetColumnRequest column);

    /// <summary>
    /// Deletes a column.
    /// </summary>
    Task DeleteColumnAsync(int columnId);

    /// <summary>
    /// Reorders columns in a sheet.
    /// </summary>
    Task ReorderColumnsAsync(int sheetId, IEnumerable<int> columnIds);

    #endregion

    #region Sheet Rows

    /// <summary>
    /// Gets rows for a sheet.
    /// </summary>
    Task<List<SheetRowViewModel>> GetRowsAsync(int sheetId, int skip = 0, int take = 100);

    /// <summary>
    /// Adds a row to a sheet.
    /// </summary>
    Task<SheetRowViewModel> AddRowAsync(CreateSheetRowRequest request);

    /// <summary>
    /// Deletes a row.
    /// </summary>
    Task DeleteRowAsync(int rowId, int? sheetId = null);

    /// <summary>
    /// Reorders rows in a sheet.
    /// </summary>
    Task ReorderRowsAsync(int sheetId, IEnumerable<int> rowIds);

    #endregion

    #region Sheet Cells

    /// <summary>
    /// Gets a cell value.
    /// </summary>
    Task<SheetCellViewModel> GetCellAsync(int rowId, int columnId);

    /// <summary>
    /// Gets all cells for a row.
    /// </summary>
    Task<List<SheetCellViewModel>> GetCellsForRowAsync(int rowId);

    /// <summary>
    /// Gets all cells for a column.
    /// </summary>
    Task<List<SheetCellViewModel>> GetCellsForColumnAsync(int columnId);

    /// <summary>
    /// Sets a cell value.
    /// </summary>
    Task<SheetCellViewModel> SetCellValueAsync(int rowId, int columnId, object? value);

    #endregion

    #region Sheet Views

    /// <summary>
    /// Gets views for a sheet.
    /// </summary>
    Task<List<SheetViewViewModel>> GetViewsAsync(int sheetId);

    /// <summary>
    /// Adds a view to a sheet.
    /// </summary>
    Task<SheetViewViewModel> AddViewAsync(CreateSheetViewRequest request);

    /// <summary>
    /// Deletes a view.
    /// </summary>
    Task DeleteViewAsync(int viewId);

    #endregion
}
