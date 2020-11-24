using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    internal class NotificationRegistrationRepository : RepositoryBase, INotificationRegistrationRepository
    {
        public Task<Guid> CreateAsync(NotificationRegistration entity) => throw new NotImplementedException();
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<NotificationRegistration> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<NotificationRegistration> GetAsync(Guid id) => throw new NotImplementedException();
        public Task UpdateAsync(NotificationRegistration entity) => throw new NotImplementedException();
        public Task<bool> UserHasRegistrationAsync(Guid userId) => throw new NotImplementedException();
    }
}
