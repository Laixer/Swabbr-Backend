using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;

namespace Swabbr.Infrastructure.Data
{
    public class UserSettingsRepository : DbRepository<UserSettings, UserSettingsTableEntity>, IUserSettingsRepository
    {
        private readonly IDbClientFactory _factory;

        public UserSettingsRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName => "UserSettings";

        public override UserSettingsTableEntity Map(UserSettings entity)
        {
            return new UserSettingsTableEntity
            {
                DailyVlogRequestLimit = entity.DailyVlogRequestLimit,
                FollowMode = (int)entity.FollowMode,
                IsPrivate = entity.IsPrivate,
                UserId = entity.UserId
            };
        }

        public override UserSettings Map(UserSettingsTableEntity entity)
        {
            return new UserSettings
            {
                DailyVlogRequestLimit = entity.DailyVlogRequestLimit,
                FollowMode = (FollowMode)entity.FollowMode,
                IsPrivate = entity.IsPrivate,
                UserId = entity.UserId
            };
        }

        public override string ResolvePartitionKey(UserSettingsTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(UserSettingsTableEntity entity) => entity.UserId.ToString();
    }
}
