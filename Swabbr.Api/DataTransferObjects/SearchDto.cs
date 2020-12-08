using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     Generic searchterm dto.
    /// </summary>
    public class SearchDto : PaginationDto
    {
        /// <summary>
        ///     Search string with a minimum length of 3.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MinLength(3)]
        public string Query { get; set; }
    }
}
