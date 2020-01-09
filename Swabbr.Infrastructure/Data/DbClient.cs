using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Client for a single ComsosDb database table.
    /// </summary>
    public class DbClient<T> : IDbClient<T> where T : TableEntity
    {
        private readonly CloudTableClient _client;
        private readonly CloudTable _table;

        public DbClient(string tableName, CloudTableClient client)
        {
            _client = client;

            // Store table reference in private property
            _table = client.GetTableReference(tableName);
        }

        public CloudTable TableReference => _table;

        public async Task<T> DeleteEntityAsync(T item)
        {
            TableOperation operation = TableOperation.Delete(item);
            var result = await _table.ExecuteAsync(operation);

            return result.Result as T;
        }

        public async Task<T> InsertEntityAsync(T item)
        {
            TableOperation operation = TableOperation.Insert(item, true);
            var result = await _table.ExecuteAsync(operation);
            return result.Result as T;
        }

        public async Task<T> InsertOrMergeEntityAsync(T item)
        {
            TableOperation operation = TableOperation.InsertOrMerge(item);
            var result = await _table.ExecuteAsync(operation);
            return result.Result as T;
        }

        public async Task<T> RetrieveEntityAsync(string partitionKey, string rowKey)
        {
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await _table.ExecuteAsync(operation);
            var entity = result.Result as T;
            return entity;
        }

        public async Task<T> ReplaceEntityAsync(T item)
        {
            TableOperation operation = TableOperation.Replace(item);
            var result = await _table.ExecuteAsync(operation);
            return result.Result as T;
        }

        public async Task<T> MergeEntityAsync(T item)
        {
            TableOperation operation = TableOperation.Merge(item);
            var result = await _table.ExecuteAsync(operation);
            return result.Result as T;
        }
    }
}