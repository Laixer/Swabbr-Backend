namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Contains pagination information for 
    ///     querying our data store.
    /// </summary>
    public class Navigation
    {
        /// <summary>
        ///     The page to fetch.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        ///     The amount of items to fetch.
        /// </summary>
        public uint Limit { get; set; }

        /// <summary>
        ///     Default navigation implementation.
        /// </summary>
        public static Navigation Default => new Navigation
        {
            Limit = 25,
            Offset = 0,
        };

        /// <summary>
        ///     Navigation indicating we want 
        ///     the complete result set.
        /// </summary>
        public static Navigation All => new Navigation
        {
            Limit = 0,
            Offset = 0
        };
    }
}
