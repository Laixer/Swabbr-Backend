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

        public async Task<TModel> GetAsync(string partitionKey, string rowKey)
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

        public async Task<TModel> AddAsync(TModel entity)
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
            catch (Exception e)
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
            catch (Exception e)
            {
                throw new EntityNotFoundException();

                throw;
            }
        }

        public async Task DeleteAsync(TModel entity)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);

                //TODO
                var e = Map(entity);

                await client.DeleteEntityAsync(e.PartitionKey, e.RowKey);
            }
            catch (Exception e)
            {
                throw new EntityNotFoundException();

                throw;
            }
        }

        // TODO Where should this be placed?
        /// <summary>
        /// Generates a new unique identifier for an entity.
        /// </summary>
        public static Guid GenerateEntityId()
        {
            return Guid.NewGuid();
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