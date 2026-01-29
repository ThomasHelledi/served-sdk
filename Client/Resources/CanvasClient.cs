using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Served.SDK.Client.Interfaces;
using Served.SDK.Models.Canvas;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Client for Canvas operations (Obsidian-style infinite canvas).
/// </summary>
public class CanvasClient : ICanvasClient
{
    private readonly IServedClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CanvasClient"/> class.
    /// </summary>
    public CanvasClient(IServedClient client)
    {
        _client = client;
    }

    #region Canvas CRUD

    /// <inheritdoc/>
    public async Task<List<CanvasListViewModel>> GetAllByWorkspaceAsync(int workspaceId)
    {
        return await _client.GetAsync<List<CanvasListViewModel>>($"api/canvas?workspaceId={workspaceId}");
    }

    /// <inheritdoc/>
    public async Task<CanvasDetailViewModel> GetAsync(int id)
    {
        return await _client.GetAsync<CanvasDetailViewModel>($"api/canvas/{id}");
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(CanvasCreateRequest request)
    {
        var result = await _client.PostAsync<CreateResult>("api/canvas", request);
        return result.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(int id, CanvasUpdateRequest request)
    {
        await _client.PutAsync<object>($"api/canvas/{id}", request);
    }

    /// <inheritdoc/>
    public async Task UpdateContentAsync(int id, CanvasContentRequest content)
    {
        await _client.PutAsync<object>($"api/canvas/{id}/content", content);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        await _client.DeleteAsync($"api/canvas/{id}");
    }

    /// <inheritdoc/>
    public async Task<int> DuplicateAsync(int id)
    {
        var result = await _client.PostAsync<CreateResult>($"api/canvas/{id}/duplicate", new { });
        return result.Id;
    }

    #endregion

    #region Node Operations

    /// <inheritdoc/>
    public async Task<string> AddNodeAsync(int canvasId, CanvasNodeCreateRequest request)
    {
        var result = await _client.PostAsync<NodeCreateResult>($"api/canvas/{canvasId}/nodes", request);
        return result.NodeId;
    }

    /// <inheritdoc/>
    public async Task UpdateNodeAsync(int canvasId, string nodeId, CanvasNodeUpdateRequest request)
    {
        await _client.PutAsync<object>($"api/canvas/{canvasId}/nodes/{nodeId}", request);
    }

    /// <inheritdoc/>
    public async Task DeleteNodeAsync(int canvasId, string nodeId)
    {
        await _client.DeleteAsync($"api/canvas/{canvasId}/nodes/{nodeId}");
    }

    /// <inheritdoc/>
    public async Task BulkUpdateNodePositionsAsync(int canvasId, List<NodePositionRequest> positions)
    {
        await _client.PutAsync<object>($"api/canvas/{canvasId}/nodes/positions", positions);
    }

    #endregion

    #region Edge Operations

    /// <inheritdoc/>
    public async Task<string> AddEdgeAsync(int canvasId, CanvasEdgeCreateRequest request)
    {
        var result = await _client.PostAsync<EdgeCreateResult>($"api/canvas/{canvasId}/edges", request);
        return result.EdgeId;
    }

    /// <inheritdoc/>
    public async Task UpdateEdgeAsync(int canvasId, string edgeId, CanvasEdgeUpdateRequest request)
    {
        await _client.PutAsync<object>($"api/canvas/{canvasId}/edges/{edgeId}", request);
    }

    /// <inheritdoc/>
    public async Task DeleteEdgeAsync(int canvasId, string edgeId)
    {
        await _client.DeleteAsync($"api/canvas/{canvasId}/edges/{edgeId}");
    }

    #endregion

    #region Import/Export

    /// <inheritdoc/>
    public async Task<string> ExportAsJsonCanvasAsync(int id)
    {
        return await _client.GetAsync<string>($"api/canvas/{id}/export");
    }

    /// <inheritdoc/>
    public async Task<int> ImportFromJsonCanvasAsync(int workspaceId, string name, string jsonCanvas)
    {
        var result = await _client.PostAsync<CreateResult>(
            $"api/canvas/import?workspaceId={workspaceId}&name={Uri.EscapeDataString(name)}",
            jsonCanvas);
        return result.Id;
    }

    #endregion

    #region Helper Classes

    private class CreateResult
    {
        public int Id { get; set; }
    }

    private class NodeCreateResult
    {
        public string NodeId { get; set; } = "";
    }

    private class EdgeCreateResult
    {
        public string EdgeId { get; set; } = "";
    }

    #endregion
}

/// <summary>
/// Interface for Canvas client operations.
/// </summary>
public interface ICanvasClient
{
    /// <summary>
    /// Get all canvases in a workspace.
    /// </summary>
    Task<List<CanvasListViewModel>> GetAllByWorkspaceAsync(int workspaceId);

    /// <summary>
    /// Get canvas by ID with nodes and edges.
    /// </summary>
    Task<CanvasDetailViewModel> GetAsync(int id);

    /// <summary>
    /// Create a new canvas.
    /// </summary>
    Task<int> CreateAsync(CanvasCreateRequest request);

    /// <summary>
    /// Update canvas metadata.
    /// </summary>
    Task UpdateAsync(int id, CanvasUpdateRequest request);

    /// <summary>
    /// Update canvas content (nodes and edges).
    /// </summary>
    Task UpdateContentAsync(int id, CanvasContentRequest content);

    /// <summary>
    /// Delete a canvas.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Duplicate a canvas.
    /// </summary>
    Task<int> DuplicateAsync(int id);

    /// <summary>
    /// Add a node to a canvas.
    /// </summary>
    Task<string> AddNodeAsync(int canvasId, CanvasNodeCreateRequest request);

    /// <summary>
    /// Update a node.
    /// </summary>
    Task UpdateNodeAsync(int canvasId, string nodeId, CanvasNodeUpdateRequest request);

    /// <summary>
    /// Delete a node.
    /// </summary>
    Task DeleteNodeAsync(int canvasId, string nodeId);

    /// <summary>
    /// Bulk update node positions.
    /// </summary>
    Task BulkUpdateNodePositionsAsync(int canvasId, List<NodePositionRequest> positions);

    /// <summary>
    /// Add an edge between nodes.
    /// </summary>
    Task<string> AddEdgeAsync(int canvasId, CanvasEdgeCreateRequest request);

    /// <summary>
    /// Update an edge.
    /// </summary>
    Task UpdateEdgeAsync(int canvasId, string edgeId, CanvasEdgeUpdateRequest request);

    /// <summary>
    /// Delete an edge.
    /// </summary>
    Task DeleteEdgeAsync(int canvasId, string edgeId);

    /// <summary>
    /// Export canvas as JSON Canvas format.
    /// </summary>
    Task<string> ExportAsJsonCanvasAsync(int id);

    /// <summary>
    /// Import canvas from JSON Canvas format.
    /// </summary>
    Task<int> ImportFromJsonCanvasAsync(int workspaceId, string name, string jsonCanvas);
}
