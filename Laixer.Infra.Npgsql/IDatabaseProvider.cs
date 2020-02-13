using System.Data;

namespace Laixer.Infra.Npgsql
{

    /// <summary>
    /// Provides 
    /// </summary>
    public interface IDatabaseProvider
    {

        /// <summary>
        /// Gets a database connection scope.
        /// </summary>
        /// <returns><see cref="IDbConnection"/></returns>
        IDbConnection GetConnectionScope();

    }

}
