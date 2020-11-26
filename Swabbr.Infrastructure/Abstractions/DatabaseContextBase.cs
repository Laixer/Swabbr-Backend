using Swabbr.Core.Abstractions;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Providers;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Abstractions
{
    /// <summary>
    ///     Abstract base class for all our repositories.
    /// </summary>
    internal abstract class DatabaseContextBase : AppServiceBase
    {
        /// <summary>
        ///     Database provider.
        /// </summary>
        public DatabaseProvider DatabaseProvider { get; set; }

        // TODO Is this the correct usage of a context base, returning a created context?
        //      Maybe the name of this class is incorrect and confusing?
        /// <summary>
        ///     Create the database context.
        /// </summary>
        public virtual async ValueTask<DatabaseContext> CreateNewDatabaseContext(string cmdText)
        {
            var context = new DatabaseContext
            {
                DatabaseProvider = DatabaseProvider,
                AppContext = AppContext,
            };

            // This opens the connection and creates a command.
            await context.InitializeAsync(cmdText).ConfigureAwait(false);

            return context;
        }

        // FUTURE: Maybe too npgsql specific.
        /// <summary>
        ///     Append our navigation properties to the specified 
        ///     sql command text.
        /// </summary>
        /// <remarks>
        ///     Call this last in the sql query definition process.
        /// </remarks>
        /// <param name="cmdText">SQL query.</param>
        /// <param name="navigation">Navigation control.</param>
        protected static void ConstructNavigation(ref string cmdText, Navigation navigation)
        {
            const string lineFeed = "\r\n";

            if (navigation.Offset != 0)
            {
                cmdText += $"{lineFeed} OFFSET {navigation.Offset}";
            }

            if (navigation.Limit != 0)
            {
                cmdText += $"{lineFeed} LIMIT {navigation.Limit}";
            }
        }
    }
}
