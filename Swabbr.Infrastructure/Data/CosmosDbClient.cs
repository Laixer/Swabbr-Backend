// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.using System

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;


namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Client for a ComsosDb database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CosmosDbClient<T> : ICosmosDbClient<T>
    {
        private readonly string _tableName;
        private readonly CloudTableClient _client;

        public CosmosDbClient(string tableName, CloudTableClient client)
        {
            // TODO: Null check?
            _tableName = tableName;
            _client = client;
        }

        public Task<T> DeleteEntityAsync(string partitionKey, string rowKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> InsertEntityAsync(T item)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<T>> QueryTableAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<T> RetrieveEntityAsync(string partitionKey, string rowKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> UpdateEntityAsync(TableEntity item)
        {
            throw new System.NotImplementedException();
        }

        /*
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
                 */

    }
}
