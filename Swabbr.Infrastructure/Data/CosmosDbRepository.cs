using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Models;

namespace Swabbr.Infrastructure.Data
{
    public abstract class CosmosDbRepository<T> : IRepository<T> where T : TableEntity
    {
        private readonly ICosmosDbClientFactory _cosmosDbClientFactory;

        protected CosmosDbRepository(ICosmosDbClientFactory cosmosDbClientFactory)
        {
            _cosmosDbClientFactory = cosmosDbClientFactory;
        }

        public async Task<T> GetByIdAsync(string partitionKey, string rowKey)
        {
            try
            {
                var cosmosDbClient = _cosmosDbClientFactory.GetClient<T>(TableName);
                var item = await cosmosDbClient.RetrieveEntityAsync(partitionKey, rowKey);
                return item;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException();
                }

                throw;
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                var cosmosDbClient = _cosmosDbClientFactory.GetClient<T>(TableName);
                var item = await cosmosDbClient.InsertEntityAsync(entity);
                return item;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new EntityAlreadyExistsException();
                }

                throw;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                var cosmosDbClient = _cosmosDbClientFactory.GetClient<T>(TableName);
                await cosmosDbClient.UpdateEntityAsync(entity);
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException();
                }

                throw;
            }
        }

        public async Task DeleteAsync(T entity)
        {
            try
            {
                var cosmosDbClient = _cosmosDbClientFactory.GetClient<T>(TableName);
                await cosmosDbClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new EntityNotFoundException();
                }

                throw;
            }
        }

        // TODO: .................
        /// <summary>
        /// Name of the table
        /// </summary>
        public abstract string TableName { get; }
    }
}
