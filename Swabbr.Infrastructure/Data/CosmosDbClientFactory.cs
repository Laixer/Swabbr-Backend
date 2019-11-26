// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.using System

using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace Swabbr.Infrastructure.Data
{
    public class CosmosDbClientFactory : ICosmosDbClientFactory
    {
        private readonly string _tableName;
        private readonly List<ContainerProperties> _containerInfo;
        private readonly CloudTableClient _client;

        public CosmosDbClientFactory(string tableName, List<ContainerProperties> containerInfo, CloudStorageAccount storageAccount)
        {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            _containerInfo = containerInfo ?? throw new ArgumentNullException(nameof(containerInfo));
            _client = storageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Returns a Cosmos Db client for a single entity container of type <see cref="T"/>.
        /// </summary>
        /// <param name="tableName">Name of the container</param>
        /// <returns></returns>
        public ICosmosDbClient<T> GetClient<T>(string tableName)
        {
            // TODO
            ////if (!_containerInfo.Any(col => col.Name.Equals(tableName)))
            ////{
            ////    throw new ArgumentException($"Container not found/specified: {tableName}");
            ////}
            ////
            ////var collection = _containerInfo.Where(c => c.Name.Equals(tableName)).First();

            return new CosmosDbClient<T>(_tableName, _client);
        }

        /// <summary>
        /// Ensure all specified collections exist in the database.
        /// </summary>
        public async Task EnsureDbSetupAsync(TableRequestOptions requestOptions)
        {
            // Create table if it doesn't exist.
            await _client.GetTableReference(_tableName).CreateIfNotExistsAsync();


            // TODO..... Clean up
            // Create the specified containers in the database if they do not exist.
            /////foreach (var collection in _containerInfo)
            /////{
            /////    await result.Database.CreateContainerIfNotExistsAsync(
            /////        new Microsoft.Azure.Cosmos.ContainerProperties
            /////        {
            /////            Id = collection.Name,
            /////            PartitionKeyPath = collection.PartitionKey
            /////        }
            /////    );
            /////}
        }
    }
}
