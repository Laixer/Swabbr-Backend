using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository create functionality for an entity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TPrimary">Entity primary key.</typeparam>
    public interface ICreateRepository<TEntity, TPrimary>
        where TEntity : EntityBase<TPrimary>
    {
        /// <summary>
        ///     Create a new entity in our data store.
        /// </summary>
        /// <param name="entity">The populated entity to create.</param>
        /// <returns>The primary key of the created entity.</returns>
        Task<TPrimary> CreateAsync(TEntity entity);
    }
}
