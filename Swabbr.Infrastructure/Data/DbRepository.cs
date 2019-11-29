using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public abstract class DbRepository<T> : IRepository<T>, ITableContext<T> where T : TableEntity
    {
        private readonly IDbClientFactory _factory;

        protected DbRepository(IDbClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<T> GetByIdAsync(string partitionKey, string rowKey)
        {
            try
            {
                var client = _factory.GetClient<T>(TableName);
                var item = await client.RetrieveEntityAsync(partitionKey, rowKey);
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
                var client = _factory.GetClient<T>(TableName);
                var item = await client.InsertEntityAsync(entity);
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
                var client = _factory.GetClient<T>(TableName);
                await client.UpdateEntityAsync(entity);
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
                var client = _factory.GetClient<T>(TableName);
                await client.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
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

        public abstract string GenerateId(T entity);
        public abstract string ResolvePartitionKey(T entityId);
        public abstract string ResolveRowKey(T entityId);

        // TODO: .................
        /// <summary>
        /// Name of the table
        /// </summary>
        public abstract string TableName { get; }
    }
}
