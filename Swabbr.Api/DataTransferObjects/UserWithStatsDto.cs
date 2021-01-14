namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a user with its statistics.
    /// </summary>
    public record UserWithStatsDto : UserDto
    {
        /// <summary>
        ///     The total amount of accumulated likes for the vlogs of this user.
        /// </summary>
        public uint TotalLikesReceived { get; init; }

        /// <summary>
        ///     The total amount of users that are following this user.
        /// </summary>
        public uint TotalFollowers { get; init; }

        /// <summary>
        ///     The total amount of users this user is following.
        /// </summary>
        public uint TotalFollowing { get; init; }

        /// <summary>
        ///     The total amount of placed reactions by this user.
        /// </summary>
        public uint TotalReactionsGiven { get; init; }

        /// <summary>
        ///     The total amount of received reactions by this user.
        /// </summary>
        public uint TotalReactionsReceived { get; init; }

        /// <summary>
        ///     The total amount of placed vlogs by this user.
        /// </summary>
        public uint TotalVlogs { get; init; }

        /// <summary>
        ///     The total amount of accumulated views for the vlogs this user owns.
        /// </summary>
        public uint TotalViews { get; init; }
    }
}
