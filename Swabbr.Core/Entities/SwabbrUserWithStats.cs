namespace Swabbr.Core.Entities
{

    /// <summary>
    /// <see cref="SwabbrUser"/> including statistics.
    /// </summary>
    public sealed class SwabbrUserWithStats : SwabbrUser
    {

        /// <summary>
        /// Total amount of users following this user.
        /// </summary>
        public int TotalFollowers { get; set; }

        /// <summary>
        /// Total amount of users that this user follows.
        /// </summary>
        public int TotalFollowing { get; set; }

        /// <summary>
        /// Total amount of vlogs that this user has uploaded.
        /// </summary>
        public int TotalVlogs { get; set; }

    }
}
