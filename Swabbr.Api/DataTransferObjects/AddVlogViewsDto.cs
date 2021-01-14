using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for adding vlog views.
    /// </summary>
    public record AddVlogViewsDto
    {
        /// <summary>
        ///     Pairs of vlog ids and how many views they gained.
        /// </summary>
        [Required]
        public Dictionary<Guid, uint> VlogViewPairs { get; init; }
    }
}
