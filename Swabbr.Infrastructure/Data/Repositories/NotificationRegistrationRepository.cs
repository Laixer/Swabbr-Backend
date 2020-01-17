using Swabbr.Core.Entities;
using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Data.Entities;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class NotificationRegistrationRepository : DbRepository<NotificationRegistration, NotificationRegistrationTableEntity>
    {
        private IDbClientFactory _factory;

        public NotificationRegistrationRepository(IDbClientFactory dbClientFactory) : base (dbClientFactory)
        {
            _factory = dbClientFactory;
        }

        public override string TableName => "NotificationRegistrations";

        public override NotificationRegistrationTableEntity Map(NotificationRegistration entity)
        {
            return new NotificationRegistrationTableEntity
            {
                RegistrationId = entity.RegistrationId,
                Platform = (int)entity.Platform,
                UserId = entity.UserId,
                Handle = entity.Handle
            };
        }

        public override NotificationRegistration Map(NotificationRegistrationTableEntity entity)
        {
            return new NotificationRegistration
            {
                RegistrationId = entity.RegistrationId,
                Platform = (PushNotificationPlatform)entity.Platform,
                UserId = entity.UserId,
                Handle = entity.Handle
            };
        }

        public override string ResolvePartitionKey(NotificationRegistrationTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(NotificationRegistrationTableEntity entity) => entity.RegistrationId;

    }
}
