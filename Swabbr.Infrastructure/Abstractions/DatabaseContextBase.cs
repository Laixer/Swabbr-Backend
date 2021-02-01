using Swabbr.Core.Abstractions;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Providers;
using System;
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
            await context.InitializeAsync(cmdText);

            return context;
        }

        // FUTURE: Maybe too npgsql specific.
        /// <summary>
        ///     Append our navigation properties to the specified 
        ///     sql command text.
        /// </summary>
        /// <remarks>
        ///     If the <paramref name="navigation"/> has no sorting
        ///     order specified or if <paramref name="sortingColumn"/>
        ///     is null or empty, no sorting is appended to the query.
        ///     Note that <paramref name="sortingColumn"/> is optional, 
        ///     which means that sorting will be ignored if none is
        ///     specified.
        /// </remarks>
        /// <param name="cmdText">SQL query.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <param name="sortingColumn">The column to sort by.</param>
        protected static void ConstructNavigation(ref string cmdText, Navigation navigation, string sortingColumn = null)
        {
            if (navigation is null)
            {
                throw new ArgumentNullException(nameof(navigation));
            }

            const string lineFeed = "\r\n";

            // Only append sorting if we have both an order and a column.
            if (navigation.SortingOrder != SortingOrder.Unsorted || string.IsNullOrEmpty(sortingColumn))
            {
                cmdText += $"{lineFeed} ORDER BY {sortingColumn} {Parse(navigation.SortingOrder)}";
            }

            // Only append an offset is one is specified.
            if (navigation.Offset != 0)
            {
                cmdText += $"{lineFeed} OFFSET {navigation.Offset}";
            }

            // Only append a limit if one is specified.
            if (navigation.Limit != 0)
            {
                cmdText += $"{lineFeed} LIMIT {navigation.Limit}";
            }

            // Converts the sorting order enum to SQL clause.
            static string Parse(SortingOrder sortingOrder) =>
                sortingOrder switch
                {
                    SortingOrder.Ascending => "ASC",
                    SortingOrder.Descending => "DESC",
                    _ => throw new InvalidOperationException(nameof(sortingOrder))
                };
        }
    }
}
