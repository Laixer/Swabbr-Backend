using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using System;
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
                throw new EntityAlreadyExistsException();

                throw;
            }
        }

        //TODO For update and delete: Return updated ?
        public async Task UpdateAsync(TModel entity)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);
                await client.UpdateEntityAsync(Map(entity));
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