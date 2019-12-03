using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Interfaces
{
    /// <summary>
    /// Factory for providing an <see cref="IDbClient"/> for database entities
    /// </summary>
    public interface IDbClientFactory
    {
        IDbClient<T> GetClient<T>(string tableName) where T : TableEntity;

        Task DeleteAllTables();
    }
}