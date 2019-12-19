using Microsoft.Azure.Cosmos.Table;
using Swabbr.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public sealed class DbClientFactory : IDbClientFactory
    {
        private readonly List<StorageTableInfo> _tables;
        private readonly CloudTableClient _client;

        public DbClientFactory(List<StorageTableInfo> tables, CloudStorageAccount storageAccount)
        {
            _tables = tables ?? throw new ArgumentNullException(nameof(tables));
            _client = storageAccount.CreateCloudTableClient();
        }

        IDbClient<T> IDbClientFactory.GetClient<T>(string tableName)
        {
            return new DbClient<T>(tableName, _client);
        }

        /// <summary>
        /// Ensure all specified tables exist in the database.
        /// </summary>
        public async Task EnsureDbSetupAsync()
        {
            // Create the tables if they do not already exist within the database.
            foreach (var table in _tables)
            {
                var createdTable = _client.GetTableReference(table.Id);
                await createdTable.CreateIfNotExistsAsync();
            }
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

        Task IDbClientFactory.DeleteAllTables()
        {
            throw new NotImplementedException();
        }
    }
}