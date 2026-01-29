using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Board;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for board and sheet resources.
/// </summary>
public class BoardApi : ApiModuleBase
{
    protected override string ModulePath => "board";

    /// <summary>
    /// Access to board resources.
    /// </summary>
    public BoardsResource Boards { get; }

    /// <summary>
    /// Access to sheet resources.
    /// </summary>
    public SheetsResource Sheets { get; }

    public BoardApi(IHttpClient http) : base(http)
    {
        Boards = new BoardsResource(http, this);
        Sheets = new SheetsResource(http, this);
    }

    #region Boards Resource

    /// <summary>
    /// Resource client for board operations.
    /// </summary>
    public class BoardsResource
    {
        private readonly IHttpClient _http;
        private readonly BoardApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/boards";

        internal BoardsResource(IHttpClient http, BoardApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all boards.
        /// </summary>
        public async Task<List<BoardViewModel>> GetAllAsync(int? workspaceId = null)
        {
            var query = workspaceId.HasValue ? $"?workspaceId={workspaceId}" : "";
            var response = await _http.GetAsync<ApiV2ListResponse<BoardViewModel>>($"{BasePath}{query}");
            return response.Data ?? new List<BoardViewModel>();
        }

        /// <summary>
        /// Gets a board by ID.
        /// </summary>
        public Task<BoardViewModel> GetAsync(int id)
        {
            return _http.GetAsync<BoardViewModel>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Creates a new board.
        /// </summary>
        public Task<BoardViewModel> CreateAsync(CreateBoardRequest request)
        {
            return _http.PostAsync<BoardViewModel>(BasePath, request);
        }

        /// <summary>
        /// Updates a board.
        /// </summary>
        public Task<BoardViewModel> UpdateAsync(int id, UpdateBoardRequest request)
        {
            return _http.PutAsync<BoardViewModel>($"{BasePath}/{id}", request);
        }

        /// <summary>
        /// Deletes a board.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.DeleteAsync($"{BasePath}/{id}");
        }
    }

    #endregion

    #region Sheets Resource

    /// <summary>
    /// Resource client for sheet operations.
    /// </summary>
    public class SheetsResource
    {
        private readonly IHttpClient _http;
        private readonly BoardApi _module;
        private string BasePath => $"api/{_module.Version}/{_module.ModulePath}/sheets";

        internal SheetsResource(IHttpClient http, BoardApi module)
        {
            _http = http;
            _module = module;
        }

        /// <summary>
        /// Gets all sheets.
        /// </summary>
        public async Task<List<SheetViewModel>> GetAllAsync(int? workspaceId = null)
        {
            var query = workspaceId.HasValue ? $"?workspaceId={workspaceId}" : "";
            var response = await _http.GetAsync<ApiV2ListResponse<SheetViewModel>>($"{BasePath}{query}");
            return response.Data ?? new List<SheetViewModel>();
        }

        /// <summary>
        /// Gets a sheet by ID.
        /// </summary>
        public Task<SheetViewModel> GetAsync(int id)
        {
            return _http.GetAsync<SheetViewModel>($"{BasePath}/{id}");
        }

        /// <summary>
        /// Creates a new sheet.
        /// </summary>
        public Task<SheetViewModel> CreateAsync(CreateSheetRequest request)
        {
            return _http.PostAsync<SheetViewModel>(BasePath, request);
        }

        /// <summary>
        /// Updates a sheet.
        /// </summary>
        public Task<SheetViewModel> UpdateAsync(int id, UpdateSheetRequest request)
        {
            return _http.PutAsync<SheetViewModel>($"{BasePath}/{id}", request);
        }

        /// <summary>
        /// Deletes a sheet.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _http.DeleteAsync($"{BasePath}/{id}");
        }
    }

    #endregion
}
