using System.Data;

namespace Swabbr.Infrastructure.Providers
{
    /// <summary>
    ///     Provides connections to our database.
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        ///     Gets a database connection scope.
        /// </summary>
        /// <returns><see cref="IDbConnection"/> instance.</returns>
        IDbConnection GetConnectionScope();
    }
}
