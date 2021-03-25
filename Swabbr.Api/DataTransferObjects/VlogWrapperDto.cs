namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a vlog wrapper.
    /// </summary>
    public record VlogWrapperDto
    {
        /// <summary>
        ///     Vlog.
        /// </summary>
        public VlogDto Vlog { get; init; }

        /// <summary>
        ///     Corresponding user.
        /// </summary>
        public UserDto User { get; init; }

        /// <summary>
        ///     Total amount of likes.
        /// </summary>
        public int VlogLikeCount { get; init; }

        /// <summary>
        ///     Total amount of reactions.
        /// </summary>
        public int ReactionCount { get; init; }
    }
}
