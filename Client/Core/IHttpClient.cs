using System.Threading.Tasks;

namespace Served.SDK.Client.Core;

/// <summary>
/// Internal interface for HTTP operations used by API clients.
/// </summary>
public interface IHttpClient
{
    /// <summary>
    /// Performs a GET request.
    /// </summary>
    Task<T> GetAsync<T>(string uri);

    /// <summary>
    /// Performs a POST request with a typed response.
    /// </summary>
    Task<T> PostAsync<T>(string uri, object data);

    /// <summary>
    /// Performs a POST request without a response body.
    /// </summary>
    Task PostAsync(string uri, object data);

    /// <summary>
    /// Performs a PUT request.
    /// </summary>
    Task<T> PutAsync<T>(string uri, object data);

    /// <summary>
    /// Performs a PATCH request.
    /// </summary>
    Task<T> PatchAsync<T>(string uri, object data);

    /// <summary>
    /// Performs a DELETE request.
    /// </summary>
    Task DeleteAsync(string uri);

    /// <summary>
    /// Performs a DELETE request with a body.
    /// </summary>
    Task DeleteWithBodyAsync(string uri, object data);

    /// <summary>
    /// Performs a DELETE request with a body and typed response.
    /// </summary>
    Task<T> DeleteWithBodyAsync<T>(string uri, object data);
}
