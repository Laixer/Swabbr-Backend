namespace Swabbr.Core.Notifications
{

    /// <summary>
    /// Contains constant values for our notification trigger actions.
    /// </summary>
    public static class NotificationActionConstants
    {

        /// <summary>
        /// A profile followed by the user has just gone live.
        /// </summary>
        public static string FOLLOWED_PROFILE_LIVE => "followed_profile_live";

        /// <summary>
        /// The user has been inactive on the app for a while.
        /// </summary>
        public static string INACTIVE_USER_MOTIVATE => "inactive_user_motivate";

        /// <summary>
        /// There are many unwatched vlogs within the app for this user.
        /// </summary>
        public static string INACTIVE_UNWATCHED_VLOGS => "inactive_unwatched_vlogs";

        /// <summary>
        /// The user has not recorded a vlog for a certain amount of time.
        /// </summary>
        public static string INACTIVE_VLOG_RECORD_REQUEST => "inactive_vlog_record_request";

        /// <summary>
        /// A users’ vlog has received many love it’s (likes).
        /// </summary>
        public static string VLOG_GAINED_LIKES => "vlog_gained_likes";

        /// <summary>
        /// One or more reactions have been placed on a vlog that belongs to the user.
        /// </summary>
        public static string VLOG_NEW_REACTION => "vlog_new_reaction";

        /// <summary>
        /// Max. 3 times a day request to record a vlog.
        /// </summary>
        public static string VLOG_RECORD_REQUEST => "vlog_record_request";

    }

}
