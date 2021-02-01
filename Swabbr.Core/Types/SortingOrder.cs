namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Enum representing a data sorting order.
    /// </summary>
    public enum SortingOrder
    {
        /// <summary>
        ///     Indicates we do not wish to sort our items.
        /// </summary>
        Unsorted = 0,

        /// <summary>
        ///     Indicates sorting from low to high. Note that this means
        ///     that the most recent date will be returned last.
        /// </summary>
        Ascending = 1,

        /// <summary>
        ///     Indicates sorting from high to low. Note that this means
        ///     that the most recent date will be returned first.
        /// </summary>
        Descending = 2,
    }
}
