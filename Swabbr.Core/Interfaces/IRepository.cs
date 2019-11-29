using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(string partitionKey, string rowKey);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
