using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Factory for providing an <see cref="IDbClient"/> for database entities
    /// </summary>
    public interface IDbClientFactory
    {
        /// <summary>
        /// Used to retrieve the db client for a specific table
        /// </summary>
        /// <typeparam name="T">Type of the table entity</typeparam>
        /// <param name="tableName">Id/name of the table</param>
        /// <returns></returns>
        IDbClient<T> GetClient<T>(string tableName) where T : TableEntity;

        //TODO: Remove, used for testing purposes only. Tables should not (never) have to be removed by the application itself.
        Task DeleteAllTables();
    }
}