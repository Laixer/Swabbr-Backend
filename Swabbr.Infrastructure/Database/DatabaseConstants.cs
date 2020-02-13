namespace Swabbr.Infrastructure.Database
{

    /// <summary>
    /// Contains constants for our database.
    /// </summary>
    internal static class DatabaseConstants
    {

        internal static string TableFollowRequest => "public.follow_request";

        internal static string TableLivestream => "public.livestream";

        internal static string TableNotificationRegistration => "public.notification_registration";

        internal static string TableReaction => "public.reaction";

        internal static string TableUser => "public.user";

        internal static string TableVlog => "public.vlog";

        internal static string TableVlogLikke => "public.vlog_like";

        internal static string ViewUserSettings => "public.user_settings";

    }
}
