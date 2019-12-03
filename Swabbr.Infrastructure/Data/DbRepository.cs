using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public abstract class DbRepository<TModel, TDto> : IRepository<TModel>, ITableContext where TDto: TableEntity
    {
        private readonly IDbClientFactory _factory;

        protected DbRepository(IDbClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<TModel> GetByIdAsync(string partitionKey, string rowKey)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);
                var item = await client.RetrieveEntityAsync(partitionKey, rowKey);
                return Map(item);
            }
            catch (Exception e)
            {
                throw new EntityNotFoundException();

                throw;
            }
        }

        public async Task<TModel> AddAsync(TModel entity)
        {
            try
            {
                var client = _factory.GetClient<TDto>(TableName);
                var insertEntity = Map(entity);
                var item = await client.InsertEntityAsync(Map(entity));
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

        // TODO Update Summaries
        /// <summary>
        /// Method for converting an entity of type <typeparamref name="TModel"/> to an <see cref="TableEntity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract TDto Map(TModel entity);

        /// <summary>
        /// Method for converting an <see cref="TableEntity"/> to an entity of type <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract TModel Map(TDto entity);

        // TODO: .................
        /// <summary>
        /// Name of the table
        /// </summary>
        public abstract string TableName { get; }
    }
}