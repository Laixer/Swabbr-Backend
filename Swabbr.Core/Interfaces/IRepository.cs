using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        Task<TEntity> GetAsync(string partitionKey, string rowKey);

        Task<TEntity> AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}