using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Contains statistics for a single <see cref="SwabbrUser"/>.
    /// </summary>
    public sealed class UserStatistics : EntityBase<Guid>
    {

        /// <summary>
        /// The total amount of accumulated likes for the vlogs of this user.
        /// </summary>
        public int TotalLikes { get; set; }

        /// <summary>
        /// The total amount of users that are following this user.
        /// </summary>
        public int TotalFollowers { get; set; }

        /// <summary>
        /// The total amount of users this user is following.
        /// </summary>
        public int TotalFollowing { get; set; }

        /// <summary>
        /// The total amount of placed reactions by this user.
        /// </summary>
        public int TotalReactionsGiven { get; set; }

        /// <summary>
        /// The total amount of received reactions by this user.
        /// </summary>
        public int TotalReactionsReceived { get; set; }

        /// <summary>
        /// The total amount of placed vlogs by this user.
        /// </summary>
        public int TotalVlogs { get; set; }

        /// <summary>
        /// The total amount of accumulated views for the vlogs this user owns.
        /// </summary>
        public int TotalViews { get; set; }

    }

}
