using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Interfaces
{
    /// <summary>
    /// Interface for the Cosmos Db client.
    /// </summary>
    public interface IDbClient<T> where T: TableEntity
    {
        // TODO: Documentation
        Task<T> InsertEntityAsync(T item);

        Task<T> RetrieveEntityAsync(string partitionKey, string rowKey);

        Task<T> UpdateEntityAsync(T item);

        Task<T> DeleteEntityAsync(string partitionKey, string rowKey);

        Task<IEnumerable<T>> QueryTableAsync(/*  TODO  what parameters??? */);
    }
}