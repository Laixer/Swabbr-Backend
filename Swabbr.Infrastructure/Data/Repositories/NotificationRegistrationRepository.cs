using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Data.Entities;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class NotificationRegistrationRepository : DbRepository<NotificationRegistration, NotificationRegistrationTableEntity>, INotificationRegistrationRepository
    {
        private IDbClientFactory _factory;

        public NotificationRegistrationRepository(IDbClientFactory dbClientFactory) : base (dbClientFactory)
        {
            _factory = dbClientFactory;
        }

        public override string TableName => "NotificationRegistrations";

        public Task<NotificationRegistration> GetByUserIdAsync(Guid userId)
        {
            //TODO Implement correctly. Must a registration be bound to a single user or should this return a collection?
            //! Important: Currently returning the FIRST matched registration. Should we return everything or ensure only registration exists?
            var table = _factory.GetClient<NotificationRegistrationTableEntity>(TableName).TableReference;

            var tq = new TableQuery<NotificationRegistrationTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("UserId", QueryComparisons.Equal, userId));

            var queryResults = table.ExecuteQuery(tq);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            return Task.FromResult(Map(queryResults.First()));
        }

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
