// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.using System

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Azure.Cosmos;
using System.Linq;

namespace Swabbr.Infrastructure.Data
{
    public class CosmosDbClientFactory : ICosmosDbClientFactory
    {
        private readonly string _databaseId;
        private readonly List<ContainerProperties> _collectionInfo;
        private readonly CosmosClient _client;

        public CosmosDbClientFactory(string databaseName, List<ContainerProperties> collectionInfo, CosmosClient client)
        {
            _databaseId = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            _collectionInfo = collectionInfo ?? throw new ArgumentNullException(nameof(collectionInfo));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Returns a Cosmos Db client for a single entity container of type <see cref="T"/>.
        /// </summary>
        /// <param name="containerId">Name of the container</param>
        /// <returns></returns>
        public ICosmosDbClient<T> GetClient<T>(string containerId)
        {
            if (!_collectionInfo.Any(col => col.Name.Equals(containerId)))
            {
                throw new ArgumentException($"Container not found/specified: {containerId}");
            }

            var collection = _collectionInfo.Where(c => c.Name.Equals(containerId)).First();

            return new CosmosDbClient<T>(_databaseId, containerId, _client);
        }

        /// <summary>
        /// Ensure all specified collections exist in the database.
        /// </summary>
        public async Task EnsureDbSetupAsync()
        {
            // Create database if it doesn't exist.
            var dbResult = await _client.CreateDatabaseIfNotExistsAsync(_databaseId);

            // Create the specified containers in the database if they do not exist.
            foreach (var collection in _collectionInfo)
            {
                await dbResult.Database.CreateContainerIfNotExistsAsync( 
                    new Microsoft.Azure.Cosmos.ContainerProperties
                    {
                        Id = collection.Name,
                        PartitionKeyPath = collection.PartitionKey
                    }
                );
            }
        }
    }
}
