namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a reaction wrapper.
    /// </summary>
    public record ReactionWrapperDto
    {
        /// <summary>
        ///     Reaction.
        /// </summary>
        public ReactionDto Reaction { get; init; }

        /// <summary>
        ///     Corresponding user.
        /// </summary>
        public UserDto User { get; init; }
    }
}
