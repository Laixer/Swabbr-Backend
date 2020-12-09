#pragma warning disable CA1812 // Internal classes are never instantiated
namespace Swabbr.Infrastructure.Providers
{
    /// <summary>
    ///     Contains options for our database provider.
    /// </summary>
    internal class DatabaseProviderOptions
    {
        /// <summary>
        ///     The name of the connection string as specified.
        /// </summary>
        public string ConnectionStringName { get; set; }
    }
}
#pragma warning restore CA1812 // Internal classes are never instantiated
