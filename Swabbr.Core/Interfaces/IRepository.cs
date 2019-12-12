using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        Task<TEntity> RetrieveAsync(string partitionKey, string rowKey);

        Task<TEntity> CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}