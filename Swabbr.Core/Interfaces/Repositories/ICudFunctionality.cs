using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for ipmlementing create, update and delete functionality for
    /// a given <see cref="TEntity"/> entity type.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICudFunctionality<TEntity, TPrimary>
        where TEntity : EntityBase<TPrimary>
    {

        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(TPrimary id);

    }

}
