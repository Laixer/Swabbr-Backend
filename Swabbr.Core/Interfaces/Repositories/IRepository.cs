using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Generic repository base for an entity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TPrimary">Entity primary key.</typeparam>
    public interface IRepository<TEntity, TPrimary>
        where TEntity : EntityBase<TPrimary>
    {
        /// <summary>
        ///     Create a new entity in our data store.
        /// </summary>
        /// <param name="entity">The populated entity to create.</param>
        /// <returns>The primary key of the created entity.</returns>
        Task<TPrimary> CreateAsync(TEntity entity);

        /// <summary>
        ///     Delete an entity from our data store.
        /// </summary>
        /// <param name="id">The internal entity id.</param>
        Task DeleteAsync(TPrimary id);

        /// <summary>
        ///     Checks if an entity exists in our data store.
        /// </summary>
        /// <param name="id">The internal entity id.</param>
        Task<bool> ExistsAsync(TPrimary id);

        /// <summary>
        ///     Get an entity from our data store.
        /// </summary>
        /// <param name="id">The internal entity id.</param>
        /// <returns>The entity.</returns>
        Task<TEntity> GetAsync(TPrimary id);

        /// <summary>
        ///     Get all entities from our data store.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Collection of entities.</returns>
        IAsyncEnumerable<TEntity> GetAllAsync(Navigation navigation);

        /// <summary>
        ///     Update an entity in our 
        /// </summary>
        /// <param name="entity">The entity with updated properties.</param>
        Task UpdateAsync(TEntity entity);
    }
}
