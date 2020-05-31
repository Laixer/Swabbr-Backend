using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Generic repository base for an <see cref="EntityBase{TPrimary}"/>.
    /// </summary>
    public interface IRepository<TEntity, TPrimary>
        where TEntity : EntityBase<TPrimary>
    {

        Task<TEntity> GetAsync(TPrimary id);

    }

}
