using Swabbr.Api.DataTransferObjects;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Api.Extensions
{
    // TODO Where?
    /// <summary>
    ///     Contains extension functionality for <see cref="PaginationDto"/>.
    /// </summary>
    public static class PaginationDtoExtensions
    {
        /// <summary>
        ///     Extracts a navigation object from its dto.
        /// </summary>
        /// <remarks>
        ///     This defaults to <see cref="Navigation.Default"/>.
        /// </remarks>
        /// <param name="dto">The navigation dto.</param>
        /// <returns>The converted navigation object.</returns>
        public static Navigation ToNavigation(this PaginationDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            // Default return.
            if (dto.Limit == 0 && dto.Offset == 0)
            {
                return Navigation.Default;
            }

            return new Navigation
            {
                Limit = dto.Limit,
                Offset = dto.Offset
            };
        }
    }
}
