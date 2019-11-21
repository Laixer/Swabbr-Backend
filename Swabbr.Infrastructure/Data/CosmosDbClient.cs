// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.using System

using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Client for a ComsosDb database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CosmosDbClient<T> : ICosmosDbClient<T>
    {
        private readonly Database _database;
        private readonly Container _container;
        private readonly CosmosClient _client;

        public CosmosDbClient(string databaseId, string containerId, CosmosClient client)
        {
            // TODO: Null check? + try catch for cosmosclient operations
            _client = client;
            _database = _client.GetDatabase(databaseId);
            _container = _client.GetContainer(databaseId, containerId);
        }

        public async Task<ItemResponse<T>> ReadItemAsync(string id, ItemRequestOptions options = null, CancellationToken cancellationToken = default)
        {
            return await _container.ReadItemAsync<T>(
                id,
                new PartitionKey(id),
                options,
                cancellationToken);
        }

        public async Task<ItemResponse<T>> CreateItemAsync(T item, ItemRequestOptions options = null, CancellationToken cancellationToken = default)
        {
            return await _container.CreateItemAsync(item);
        }

        public async Task<ItemResponse<T>> ReplaceItemAsync(string id, T item, ItemRequestOptions options = null, CancellationToken cancellationToken = default)
        {
            return await _container.ReplaceItemAsync(
                item,
                id,
                requestOptions: options,
                cancellationToken: cancellationToken);
        }

        public async Task<ItemResponse<T>> DeleteItemAsync(string id, ItemRequestOptions options = null, CancellationToken cancellationToken = default)
        {
            return await _container.DeleteItemAsync<T>(
                id,
                new PartitionKey(id),
                requestOptions: options,
                cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<T>> QueryAsync(QueryDefinition query, ItemRequestOptions options = null, CancellationToken cancellationToken = default)
        {
            FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(query);

            var results = new List<T>();

            while(feedIterator.HasMoreResults)
            {
                FeedResponse<T> currentResultSet = await feedIterator.ReadNextAsync(cancellationToken);

                foreach(T item in currentResultSet)
                {
                    results.Add(item);
                }
            }
            return results;
        }

    }
}
