using Npgsql;
using Swabbr.Core.Enums;

namespace Swabbr.Api.DapperUtility
{

    /// <summary>
    /// Single call static class to setup <see cref="NpgsqlConnection"/>.
    /// TODO Wrong namespace
    /// </summary>
    internal static class NpgsqlSetup
    {

        /// <summary>
        /// Single setup call to setup <see cref="NpgsqlConnection"/> to be able to work
        /// with our enums.
        /// </summary>
        internal static void Setup()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowMode>("follow_mode");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowRequestStatus>("follow_request_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Gender>("gender");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PushNotificationPlatform>("push_notification_platform");
        }

    }

}
