﻿namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     <see cref="User"/> including statistics.
    /// </summary>
    public sealed class UserWithStats : User
    {
        /// <summary>
        ///     The total amount of accumulated likes for the vlogs of this user.
        /// </summary>
        public uint TotalLikesReceived { get; set; }

        /// <summary>
        ///     The total amount of users that are following this user.
        /// </summary>
        public uint TotalFollowers { get; set; }

        /// <summary>
        ///     The total amount of users this user is following.
        /// </summary>
        public uint TotalFollowing { get; set; }

        /// <summary>
        ///     The total amount of placed reactions by this user.
        /// </summary>
        public uint TotalReactionsGiven { get; set; }

        /// <summary>
        ///     The total amount of received reactions by this user.
        /// </summary>
        public uint TotalReactionsReceived { get; set; }

        /// <summary>
        ///     The total amount of placed vlogs by this user.
        /// </summary>
        public uint TotalVlogs { get; set; }

        /// <summary>
        ///     The total amount of accumulated views for the vlogs this user owns.
        /// </summary>
        public uint TotalViews { get; set; }
    }
}
