using Microsoft.Azure.Cosmos.Table;

namespace Swabbr.Infrastructure.Data.Interfaces
{
    /// <summary>
    /// Represents an Azure CosmosDb Table
    /// </summary>
    /// <typeparam name="T">The entity type of the item container</typeparam>
    public interface ITableContext<in T> where T : TableEntity
    {
        /// <summary>
        /// The name of the entity table.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Generates a unique id based on the entity.
        /// </summary>
        /// <param name="entity">The table entity to generate a unique Id for.</param>
        string GenerateId(T entity);

        string ResolvePartitionKey(T entity);

        string ResolveRowKey(T entity);
    }
}
