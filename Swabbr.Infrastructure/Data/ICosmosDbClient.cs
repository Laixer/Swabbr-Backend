// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Interface for the Cosmos Db client.
    /// </summary>
    /// <typeparam name="T">The entity type of the item container</typeparam>
    public interface ICosmosDbClient<T>
    {
        Task<ItemResponse<T>> ReadItemAsync(
            string id,
            ItemRequestOptions options = null,
            CancellationToken cancellationToken = default);

        Task<ItemResponse<T>> CreateItemAsync(
            T item,
            ItemRequestOptions options = null,
            CancellationToken cancellationToken = default);

        Task<ItemResponse<T>> ReplaceItemAsync(
            string id,
            T item,
            ItemRequestOptions options = null,
            CancellationToken cancellationToken = default);

        Task<ItemResponse<T>> DeleteItemAsync(
            string id,
            ItemRequestOptions options = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> QueryAsync(
            QueryDefinition query,
            ItemRequestOptions options = null,
            CancellationToken cancellationToken = default);
    }
}
