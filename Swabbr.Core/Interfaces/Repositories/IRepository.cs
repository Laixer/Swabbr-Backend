using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Generic repository base for an <see cref="EntityBase"/>.
    /// </summary>
    public interface IRepository<TEntity> 
        where TEntity : EntityBase
    {

        Task<TEntity> GetAsync(Guid id);

    }

}
