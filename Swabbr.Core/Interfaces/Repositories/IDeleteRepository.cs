using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository delete functionality for an entity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TPrimary">Entity primary key.</typeparam>
    public interface IDeleteRepository<TEntity, TPrimary>
        where TEntity : EntityBase<TPrimary>
    {
        /// <summary>
        ///     Delete an entity from our data store.
        /// </summary>
        /// <param name="id">The internal entity id.</param>
        Task DeleteAsync(TPrimary id);
    }
}
