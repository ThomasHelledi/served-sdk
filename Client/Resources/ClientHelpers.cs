using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Models;
using Served.SDK.Models.Common;

namespace Served.SDK.Client.Resources;

/// <summary>
/// Internal helper methods for SDK client operations.
/// </summary>
internal static class ClientHelpers
{
    /// <summary>
    /// Fetches entities using the two-step keys/cache pattern.
    /// First retrieves entity IDs from a keys endpoint, then fetches full entities from cache.
    /// </summary>
    /// <typeparam name="T">The type of entity to fetch.</typeparam>
    /// <param name="client">The Served client instance.</param>
    /// <param name="keysUri">The API endpoint URI for retrieving entity keys.</param>
    /// <param name="cacheUri">The API endpoint URI for retrieving cached entities by keys.</param>
    /// <param name="keysRequest">The request object for the keys endpoint.</param>
    /// <returns>List of entities matching the keys request.</returns>
    internal static async Task<List<T>> FetchViaKeysAndCache<T>(IServedClient client, string keysUri, string cacheUri, object keysRequest)
    {
        var keys = await client.PostAsync<List<int>>(keysUri, keysRequest);

        if (keys == null || !keys.Any())
            return new List<T>();

        var cacheItems = await client.PostAsync<List<CacheDataItem<T>>>(cacheUri, keys);

        return cacheItems
            .Where(x => x.Data != null)
            .Select(x => x.Data!)
            .ToList();
    }
}
