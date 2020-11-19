namespace Swabbr.Infrastructure.Providers
{
    /// <summary>
    ///     Contains all options for our <see cref="NpgsqlDatabaseProvider"/>.
    /// </summary>
    public sealed class NpgsqlDatabaseProviderOptions
    {
        /// <summary>
        ///     The name of the connection string as specified.
        /// </summary>
        public string ConnectionStringName { get; set; }
    }
}
