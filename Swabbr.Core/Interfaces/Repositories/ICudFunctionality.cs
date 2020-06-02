using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for ipmlementing create, update and delete functionality for
    /// a given entity type.
    /// </summary>
    /// <typeparam name="TEntity"><see cref="EntityBase{TPrimary}"/></typeparam>
    /// <typeparam name="TPrimary">Primary key for the <see cref="EntityBase{TPrimary}"/></typeparam>
    public interface ICudFunctionality<TEntity, TPrimary>
        where TEntity : EntityBase<TPrimary>
    {

        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(TPrimary id);

    }

}
