using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository update functionality for an entity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TPrimary">Entity primary key.</typeparam>
    public interface IUpdateRepository<TEntity, TPrimary>
        where TEntity : EntityBase<TPrimary>
    {
        /// <summary>
        ///     Update an entity in our 
        /// </summary>
        /// <param name="entity">The entity with updated properties.</param>
        Task UpdateAsync(TEntity entity);
    }
}
