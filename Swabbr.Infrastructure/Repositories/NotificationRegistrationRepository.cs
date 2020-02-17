using Laixer.Infra.Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="NotificationRegistration"/> entities.
    /// </summary>
    public sealed class NotificationRegistrationRepository : INotificationRegistrationRepository
    {

        private IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public NotificationRegistrationRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        public Task<NotificationRegistration> CreateAsync(NotificationRegistration entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(NotificationRegistration entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsForUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationRegistration> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationRegistration> GetByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationRegistration> UpdateAsync(NotificationRegistration entity)
        {
            throw new NotImplementedException();
        }
    }
}
