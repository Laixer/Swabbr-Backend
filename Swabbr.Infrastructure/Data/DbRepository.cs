﻿using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public abstract class DbRepository<TModel, TDto> : IRepository<TModel>, ITableContext
        where TModel : EntityBase
        where TDto : TableEntity
    {
        private readonly IDbClientFactory _factory;

        protected DbRepository(IDbClientFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Retrieves an entity by providing the key (PartitionKey/RowKey)
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task<TModel> RetrieveAsync(string partitionKey, string rowKey)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);
                var item = await client.RetrieveEntityAsync(partitionKey, rowKey);

                if (item == null)
                {
                    throw new EntityNotFoundException();
                }

                return Map(item);
            }
            catch
            {
                throw;
            }
        }

        public async Task<TModel> CreateAsync(TModel entity)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);
                var insertEntity = Map(entity);

                insertEntity.PartitionKey = ResolvePartitionKey(insertEntity);
                insertEntity.RowKey = ResolveRowKey(insertEntity);

                var item = await client.InsertEntityAsync(insertEntity);
                return Map(item);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //TODO For update and delete: Return updated ?
        public async Task UpdateAsync(TModel entity)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);
                await client.ReplaceEntityAsync(Map(entity));
            }
            catch (Exception)
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task DeleteAsync(TModel entity)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);

                // Map model to dto
                var e = Map(entity);

                // Perform delete
                await client.DeleteEntityAsync(e);
            }
            catch (Exception)
            {
                throw new EntityNotFoundException();

                throw;
            }
        }

        /// <summary>
        /// Returns the count of entities matching the given table query within the storage table.
        /// </summary>
        /// <param name="tableQuery">The query to match entities on.</param>
        /// <returns>The amount of entities in the table matching the table query.</returns>
        public async Task<int> GetEntityCount(TableQuery<DynamicTableEntity> tableQuery)
        {
            // Select only the partition key from the entity.
            tableQuery = tableQuery.Select(new string[] { "PartitionKey" });

            CloudTable cloudTable = _factory.GetClient<TDto>(TableName).CloudTableReference;

            EntityResolver<string> resolver = (pk, rk, ts, props, etag) => props.ContainsKey("PartitionKey") ? props["PartitionKey"].StringValue : null;

            List<string> entities = new List<string>();

            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<string> tableQueryResult =
                    await cloudTable.ExecuteQuerySegmentedAsync(tableQuery, resolver, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;
                entities.AddRange(tableQueryResult.Results);
            }
            while (continuationToken != null);

            return entities.Count;
        }

        /// <summary>
        /// Method for converting an entity of type <typeparamref name="TModel"/> (Domain Entity) to
        /// a <typeparamref name="TDto"/> (Table Entity).
        /// </summary>
        public abstract TDto Map(TModel entity);

        /// <summary>
        /// Method for converting a <typeparamref name="TDto"/> (Table Entity) to a <typeparamref
        /// name="TModel"/> (Domain Entity)
        /// </summary>
        public abstract TModel Map(TDto entity);

        /// <summary>
        /// Used to determine the partition key of the entity for a table.
        /// </summary>
        public abstract string ResolvePartitionKey(TDto entity);

        /// <summary>
        /// Used to determine the row key (primary key) of the entity for a table.
        /// </summary>
        public abstract string ResolveRowKey(TDto entity);

        /// <summary>
        /// Name of the table
        /// </summary>
        public abstract string TableName { get; }
    }
}