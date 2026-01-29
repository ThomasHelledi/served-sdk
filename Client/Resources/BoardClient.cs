using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Board;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for Board and Sheet operations.
/// </summary>
public class BoardClient : IBoardClient
{
    private readonly IServedClient _client;

    public BoardClient(IServedClient client)
    {
        _client = client;
    }

    #region Boards

    /// <inheritdoc/>
    public Task<List<BoardViewModel>> GetBoardsAsync()
    {
        return _client.PostAsync<List<BoardViewModel>>("api/board/Board/GetAll", new { });
    }

    /// <inheritdoc/>
    public Task<BoardViewModel?> GetBoardAsync(int id)
    {
        return _client.PostAsync<BoardViewModel?>("api/board/Board/Get", new { id });
    }

    /// <inheritdoc/>
    public Task<List<int>> GetBoardKeysAsync()
    {
        return _client.PostAsync<List<int>>("api/board/Board/GetKeys", new { });
    }

    /// <inheritdoc/>
    public Task<BoardViewModel> CreateBoardAsync(BoardViewModel board)
    {
        return _client.PostAsync<BoardViewModel>("api/board/Board/Create", board);
    }

    /// <inheritdoc/>
    public Task UpdateBoardAsync(BoardViewModel board)
    {
        return _client.PostAsync("api/board/Board/Update", board);
    }

    /// <inheritdoc/>
    public Task DeleteBoardAsync(int id)
    {
        return _client.PostAsync("api/board/Board/Delete", new { id });
    }

    #endregion

    #region Sheets

    /// <inheritdoc/>
    public async Task<List<SheetViewModel>> GetSheetsAsync()
    {
        return await _client.GetAsync<List<SheetViewModel>>("api/board/Sheet/GetAll");
    }

    /// <inheritdoc/>
    public Task<SheetViewModel?> GetSheetAsync(int id)
    {
        return _client.GetAsync<SheetViewModel?>($"api/board/Sheet/Get?id={id}");
    }

    /// <inheritdoc/>
    public Task<SheetViewModel?> GetSheetByClaimIdAsync(Guid claimId)
    {
        return _client.GetAsync<SheetViewModel?>($"api/board/Sheet/GetByClaimId?claimId={claimId}");
    }

    /// <inheritdoc/>
    public Task<SheetViewModel> CreateSheetAsync(CreateSheetRequest request)
    {
        return _client.PostAsync<SheetViewModel>("api/board/Sheet/Create", request);
    }

    /// <inheritdoc/>
    public Task<SheetViewModel> UpdateSheetAsync(UpdateSheetRequest request)
    {
        return _client.PostAsync<SheetViewModel>("api/board/Sheet/Update", request);
    }

    /// <inheritdoc/>
    public Task DeleteSheetAsync(int id)
    {
        return _client.DeleteAsync($"api/board/Sheet/Delete?id={id}");
    }

    #endregion

    #region Sheet Columns

    /// <inheritdoc/>
    public Task<List<SheetColumnViewModel>> GetColumnsAsync(int sheetId)
    {
        return _client.GetAsync<List<SheetColumnViewModel>>($"api/board/Sheet/GetColumns?sheetId={sheetId}");
    }

    /// <inheritdoc/>
    public Task<SheetColumnViewModel> AddColumnAsync(int sheetId, CreateSheetColumnRequest column)
    {
        var request = new
        {
            sheetId,
            column.Name,
            column.Description,
            column.ColumnType,
            column.Width,
            column.IsRequired,
            column.DefaultValue,
            column.ValidationPattern,
            column.AllowedValuesJson,
            column.IsAiSkillColumn,
            column.AiSkillConfigJson
        };
        return _client.PostAsync<SheetColumnViewModel>("api/board/Sheet/AddColumn", request);
    }

    /// <inheritdoc/>
    public Task DeleteColumnAsync(int columnId)
    {
        return _client.DeleteAsync($"api/board/Sheet/DeleteColumn?columnId={columnId}");
    }

    /// <inheritdoc/>
    public Task ReorderColumnsAsync(int sheetId, IEnumerable<int> columnIds)
    {
        return _client.PostAsync($"api/board/Sheet/ReorderColumns?sheetId={sheetId}", columnIds);
    }

    #endregion

    #region Sheet Rows

    /// <inheritdoc/>
    public Task<List<SheetRowViewModel>> GetRowsAsync(int sheetId, int skip = 0, int take = 100)
    {
        return _client.GetAsync<List<SheetRowViewModel>>($"api/board/Sheet/GetRows?sheetId={sheetId}&skip={skip}&take={take}");
    }

    /// <inheritdoc/>
    public Task<SheetRowViewModel> AddRowAsync(CreateSheetRowRequest request)
    {
        return _client.PostAsync<SheetRowViewModel>("api/board/Sheet/AddRow", request);
    }

    /// <inheritdoc/>
    public Task DeleteRowAsync(int rowId, int? sheetId = null)
    {
        var url = $"api/board/Sheet/DeleteRow?rowId={rowId}";
        if (sheetId.HasValue)
            url += $"&sheetId={sheetId}";
        return _client.DeleteAsync(url);
    }

    /// <inheritdoc/>
    public Task ReorderRowsAsync(int sheetId, IEnumerable<int> rowIds)
    {
        return _client.PostAsync($"api/board/Sheet/ReorderRows?sheetId={sheetId}", rowIds);
    }

    #endregion

    #region Sheet Cells

    /// <inheritdoc/>
    public Task<SheetCellViewModel> GetCellAsync(int rowId, int columnId)
    {
        return _client.GetAsync<SheetCellViewModel>($"api/board/Sheet/GetCell?rowId={rowId}&columnId={columnId}");
    }

    /// <inheritdoc/>
    public Task<List<SheetCellViewModel>> GetCellsForRowAsync(int rowId)
    {
        return _client.GetAsync<List<SheetCellViewModel>>($"api/board/Sheet/GetCellsForRow?rowId={rowId}");
    }

    /// <inheritdoc/>
    public Task<List<SheetCellViewModel>> GetCellsForColumnAsync(int columnId)
    {
        return _client.GetAsync<List<SheetCellViewModel>>($"api/board/Sheet/GetCellsForColumn?columnId={columnId}");
    }

    /// <inheritdoc/>
    public Task<SheetCellViewModel> SetCellValueAsync(int rowId, int columnId, object? value)
    {
        return _client.PostAsync<SheetCellViewModel>("api/board/Sheet/SetCellValue", new { rowId, columnId, value });
    }

    #endregion

    #region Sheet Views

    /// <inheritdoc/>
    public Task<List<SheetViewViewModel>> GetViewsAsync(int sheetId)
    {
        return _client.GetAsync<List<SheetViewViewModel>>($"api/board/Sheet/GetViews?sheetId={sheetId}");
    }

    /// <inheritdoc/>
    public Task<SheetViewViewModel> AddViewAsync(CreateSheetViewRequest request)
    {
        return _client.PostAsync<SheetViewViewModel>("api/board/Sheet/AddView", request);
    }

    /// <inheritdoc/>
    public Task DeleteViewAsync(int viewId)
    {
        return _client.DeleteAsync($"api/board/Sheet/DeleteView?viewId={viewId}");
    }

    #endregion
}
