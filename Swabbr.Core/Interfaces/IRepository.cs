using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IRepository<T> where T : TableEntity
    {
        Task<T> GetByIdAsync(string partitionKey, string rowKey);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
