using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class UserSettingsRepository : DbRepository<UserSettings, UserSettingsTableEntity>, IUserSettingsRepository
    {
        private readonly IDbClientFactory _factory;

        public UserSettingsRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName => "UserSettings";

        public async Task<UserSettings> GetByUserId(Guid userId)
        {
            var userIdString = userId.ToString();

            try
            {
                var entity = await RetrieveAsync(userIdString, userIdString);
                return entity;
            }
            catch (EntityNotFoundException)
            {
                // TODO: Temporarily creating a new record with default settings if no record for this user exists yet.
                return await CreateAsync(new UserSettings
                {
                    UserId = userId
                });
            }
        }

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