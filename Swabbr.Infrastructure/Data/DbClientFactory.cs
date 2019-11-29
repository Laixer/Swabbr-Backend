// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.using System

using Microsoft.Azure.Cosmos.Table;
using Swabbr.Infrastructure.Data.Entities;
using Swabbr.Infrastructure.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public class DbClientFactory : IDbClientFactory
    {
        private readonly List<TableProperties> _tables;
        private readonly CloudTableClient _client;

        public DbClientFactory(List<TableProperties> tables, CloudStorageAccount storageAccount)
        {
            _tables = tables ?? throw new ArgumentNullException(nameof(tables));
            _client = storageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Returns a Cosmos Db client for a single entity container of type <see cref="T"/>.
        /// </summary>
        /// <param name="tableName">Name of the container</param>
        /// <returns></returns>
        public IDbClient<T> GetClient<T>(string tableName)
        {
            // TODO
            ////if (!_containerInfo.Any(col => col.Name.Equals(tableName)))
            ////{
            ////    throw new ArgumentException($"Container not found/specified: {tableName}");
            ////}
            ////
            ////var collection = _containerInfo.Where(c => c.Name.Equals(tableName)).First();

            // TODO: Check if _tables contains the table with the given name?
            return new DbClient<T>(tableName, _client);
        }

        /// <summary>
        /// Ensure all specified tables exist in the database.
        /// </summary>
        public async Task EnsureDbSetupAsync(TableRequestOptions requestOptions = null)
        {
            // Create the tables if they do not already exist within the database.
            foreach(var table in _tables)
            {
                await _client.GetTableReference(table.Id).CreateIfNotExistsAsync();
            }

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

        public async Task DeleteAllTables()
        {
            //TODO: Remove, for temporary development purposes only
            // Delete all tables from the database to avoid unnecessary billing.
            foreach (var table in _tables)
            {
                await _client.GetTableReference(table.Id).DeleteIfExistsAsync();
            }
        }

    }
}
