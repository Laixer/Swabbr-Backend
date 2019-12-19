using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Client for a single ComsosDb database table.
    /// </summary>
    public class DbClient<T> : IDbClient<T> where T : TableEntity
    {
        private readonly string _tableName;
        private readonly CloudTableClient _client;

        public DbClient(string tableName, CloudTableClient client)
        {
            // TODO: Null check?
            _tableName = tableName;
            _client = client;
        }

        public CloudTable CloudTableReference => _client.GetTableReference(_tableName);

        public async Task<T> DeleteEntityAsync(T item)
        {
            var table = _client.GetTableReference(_tableName);
            TableOperation operation = TableOperation.Delete(item);
            var result = await table.ExecuteAsync(operation);

            return result.Result as T;
        }

        public async Task<T> InsertEntityAsync(T item)
        {
            var table = _client.GetTableReference(_tableName);
            TableOperation operation = TableOperation.Insert(item, true);
            var result = await table.ExecuteAsync(operation);
            return result.Result as T;
        }

        public async Task<T> InsertOrMergeEntityAsync(T item)
        {
            var table = _client.GetTableReference(_tableName);
            TableOperation operation = TableOperation.InsertOrMerge(item);
            var result = await table.ExecuteAsync(operation);
            return result.Result as T;
        }

        public async Task<T> RetrieveEntityAsync(string partitionKey, string rowKey)
        {
            // TODO: STORE TABLE REFERENCE IN PRIVATE PROPERTY? instead of continuosly creating it from the client?
            var table = _client.GetTableReference(_tableName);
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(operation);
            var entity = result.Result as T;
            return entity;
        }

        public async Task<T> ReplaceEntityAsync(T item)
        {
            var table = _client.GetTableReference(_tableName);
            TableOperation operation = TableOperation.Replace(item);
            var result = await table.ExecuteAsync(operation);
            return result.Result as T;
        }

        public async Task<T> MergeEntityAsync(T item)
        {
            var table = _client.GetTableReference(_tableName);
            TableOperation operation = TableOperation.Merge(item);
            var result = await table.ExecuteAsync(operation);
            return result.Result as T;
        }
    }
}