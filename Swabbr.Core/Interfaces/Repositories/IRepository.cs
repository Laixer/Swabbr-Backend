using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{

    /// <summary>
    /// Generic repository base for an <see cref="EntityBase"/>.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : EntityBase
    {

        Task<TEntity> GetAsync(Guid id);

        //Task<TEntity> CreateAsync(TEntity entity);

        //Task<TEntity> UpdateAsync(TEntity entity);

        //Task DeleteAsync(TEntity entity);

    }

}
