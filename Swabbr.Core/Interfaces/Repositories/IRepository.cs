using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    /// Repository for entities.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        // This is Table API specific
        // This should just be GetAsync(Guid id)
        Task<TEntity> RetrieveAsync(string partitionKey, string rowKey);

        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}