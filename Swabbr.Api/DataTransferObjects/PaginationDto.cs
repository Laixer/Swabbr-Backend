using Swabbr.Core.Types;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for our navigation.
    /// </summary>
    public record PaginationDto
    {
        /// <summary>
        ///     The page to fetch.
        /// </summary>
        public uint Offset { get; init; }

        /// <summary>
        ///     The amount of items to fetch.
        /// </summary>
        public uint Limit { get; init; }

        /// <summary>
        ///     Indicates the sorting order, if applicable.
        /// </summary>
        public SortingOrder SortingOrder { get; set; } = SortingOrder.Unsorted; // Explicit assignment for readability.
    }
}
