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
        /// <summary>
        /// Insert an entity of type <typeparamref name="T"/> into the database table.
        /// </summary>
        /// <param name="item">The new entity to store</param>
        Task<T> InsertEntityAsync(T item);

        /// <summary>
        /// Retrieve a single entity of type <typeparamref name="T"/> from the database table.
        /// </summary>
        /// <param name="partitionKey">Partition key of the entity.</param>
        /// <param name="rowKey">Row key (primary key) of the entity.</param>
        Task<T> RetrieveEntityAsync(string partitionKey, string rowKey);

        /// <summary>
        /// Update a single entity of type <typeparamref name="T"/> to the database table.
        /// </summary>
        Task<T> UpdateEntityAsync(T item);

        /// <summary>
        /// Deletes a single entity at the specified index of the database table.
        /// </summary>
        /// <param name="partitionKey">Partition key of the entity.</param>
        /// <param name="rowKey">Row key (primary key) of the entity.</param>
        Task<T> DeleteEntityAsync(string partitionKey, string rowKey);

        /// <summary>
        /// Returns a collection of entities of type <typeparamref name="T"/> based on the given search query.
        /// </summary>
        Task<IEnumerable<T>> QueryTableAsync(/*  TODO  what parameters??? */);
    }
}