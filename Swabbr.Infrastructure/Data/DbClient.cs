using Microsoft.Azure.Cosmos.Table;
using Swabbr.Infrastructure.Data.Interfaces;
using System.Collections.Generic;
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

        public Task<T> DeleteEntityAsync(string partitionKey, string rowKey)
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> InsertEntityAsync(T item)
        {
            var table = _client.GetTableReference(_tableName);
            TableOperation operation = TableOperation.Insert(item, true);
            var result = await table.ExecuteAsync(operation);

            // TODO: Ensure this is functioning correctly and as expectled
            return (T)result.Result;
        }

        public Task<IEnumerable<T>> QueryTableAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<T> RetrieveEntityAsync(string partitionKey, string rowKey)
        {
            // TODO: STORE TABLE REFERENCE IN PRIVATE PROPERTY?
            var table = _client.GetTableReference(_tableName);
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(operation);
            
            //TODO ???
            var xxxxx = result.Result as T;
            return xxxxx;
        }

        public async Task<T> UpdateEntityAsync(T item)
        {
            var table = _client.GetTableReference(_tableName);
            //TODO: Replace or merge here?
            TableOperation operation = TableOperation.Replace(item);
            var result = await table.ExecuteAsync(operation);
            //TODO Potential
            return (T)result.Result;
        }

        /*
                 public async Task<IEnumerable<T>> QueryAsync(QueryDefinition query, ItemRequestOptions options = null, CancellationToken cancellationToken = default)
                 {
                     FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(query);

                     var results = new List<T>();

                     while(feedIterator.HasMoreResults)
                     {
                         FeedResponse<T> currentResultSet = await feedIterator.ReadNextAsync(cancellationToken);

                         foreach(T item in currentResultSet)
                         {
                             results.Add(item);
                         }
                     }
                     return results;
                 }
                 */
    }
}