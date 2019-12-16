using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Interface for the Cosmos Db client.
    /// </summary>
    public interface IDbClient<T> where T : TableEntity
    {
        /// <summary>
        /// Insert an entity of type <typeparamref name="T"/> into the database.
        /// </summary>
        /// <param name="item">The new entity to store</param>
        Task<T> InsertEntityAsync(T item);

        /// <summary>
        /// Insert (or merges if already exists) an entity of type <typeparamref name="T"/> into the database.
        /// </summary>
        /// <param name="item">The new entity to store</param>
        Task<T> InsertOrMergeEntityAsync(T item);

        /// <summary>
        /// Retrieve a single entity of type <typeparamref name="T"/> from the database.
        /// </summary>
        /// <param name="partitionKey">Partition key of the entity.</param>
        /// <param name="rowKey">Row key (primary key) of the entity.</param>
        Task<T> RetrieveEntityAsync(string partitionKey, string rowKey);

        /// <summary>
        /// Update a single entity of type <typeparamref name="T"/> to the database.
        /// </summary>
        Task<T> UpdateEntityAsync(T item);

        /// <summary>
        /// Deletes a single entity at the specified index of the database.
        /// </summary>
        /// <param name="item">The entity to delete</param>
        Task<T> DeleteEntityAsync(T item);

        /// <summary>
        /// Used to get a reference to the type specific entity table of type <typeparamref name="T"/>
        /// </summary>
        CloudTable CloudTableReference { get; }
    }
}