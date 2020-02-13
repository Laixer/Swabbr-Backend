namespace Swabbr.Core.Enums
{
    /// <summary>
    /// Enum representation of the follow mode setting for the profile of a <see cref="UserItem"/>.
    /// </summary>
    public enum FollowMode
    {
        /// <summary>
        /// Manually accept or deny incoming follow requests.
        /// </summary>
        Manual,

        /// <summary>
        /// Automatically accept all incoming follow requests.
        /// </summary>
        AcceptAll,

        /// <summary>
        /// Automatically deny all incoming follow requests.
        /// </summary>
        DeclineAll
    }
}