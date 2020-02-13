using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class NotificationRegistrationRepository : DbRepository<NotificationRegistration, NotificationRegistrationTableEntity>, INotificationRegistrationRepository
    {
        private IDbClientFactory _factory;

        public NotificationRegistrationRepository(IDbClientFactory dbClientFactory) : base(dbClientFactory)
        {
            _factory = dbClientFactory;
        }

        public override string TableName => "NotificationRegistrations";

        public async Task<bool> ExistsForUser(Guid userId)
        {
            try
            {
                await GetByUserIdAsync(userId);
                return true;
            }
            catch (EntityNotFoundException)
            {
                return false;
            }
        }

        public async Task<NotificationRegistration> GetByUserIdAsync(Guid userId)
        {
            //TODO: Implement correctly. Must a registration be bound to a single user or should this return a collection?
            //! Important: Currently returning the FIRST matched registration. Should we return all registrations? Currently assuming only one registration exists.
            var table = _factory.GetClient<NotificationRegistrationTableEntity>(TableName).TableReference;

            var tableQuery = new TableQuery<NotificationRegistrationTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("UserId", QueryComparisons.Equal, userId));

            var queryResults = await table.ExecuteQueryAsync(tableQuery);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            return Map(queryResults.First());
        }

        public override NotificationRegistrationTableEntity Map(NotificationRegistration entity)
        {
            throw new NotImplementedException();
        }

        public override NotificationRegistration Map(NotificationRegistrationTableEntity entity)
        {
            throw new NotImplementedException();
        }

        public override string ResolvePartitionKey(NotificationRegistrationTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(NotificationRegistrationTableEntity entity) => entity.RegistrationId;
    }
}