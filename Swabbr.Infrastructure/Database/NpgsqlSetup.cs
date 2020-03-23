using Npgsql;
using Swabbr.Core.Enums;

namespace Swabbr.Infrastructure.Database
{

    /// <summary>
    /// Single call static class to setup <see cref="NpgsqlConnection"/>.
    /// TODO Wrong namespace
    /// </summary>
    public static class NpgsqlSetup
    {

        /// <summary>
        /// Single setup call to setup <see cref="NpgsqlConnection"/> to be able to work
        /// with our enums.
        /// </summary>
        public static void Setup()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowMode>("follow_mode");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowRequestStatus>("follow_request_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Gender>("gender");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<LivestreamStatus>("livestream_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PushNotificationPlatform>("push_notification_platform");
        }

    }

}
