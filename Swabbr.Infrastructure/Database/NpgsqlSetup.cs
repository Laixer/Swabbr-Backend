using Dapper;
using Npgsql;
using Swabbr.Core.Enums;

namespace Swabbr.Infrastructure.Database
{

    /// <summary>
    /// Single call static class to setup <see cref="NpgsqlConnection"/>.
    /// </summary>
    public static class NpgsqlSetup
    {

        /// <summary>
        /// Single setup call to setup <see cref="NpgsqlConnection"/> to be able to work
        /// with our enums.
        /// </summary>
        public static void Setup()
        {
            // Setup custom mappers
            SqlMapper.AddTypeHandler(new UriHandler());
            SqlMapper.AddTypeHandler(new TimeZoneInfoHandler());

            // Setup (Dapper) enums
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowMode>("follow_mode");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<FollowRequestStatus>("follow_request_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Gender>("gender");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PushNotificationPlatform>("push_notification_platform");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ReactionStatus>("reaction_state");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<VlogStatus>("vlog_state");

            // Setup Dapper name matching
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

    }

}
