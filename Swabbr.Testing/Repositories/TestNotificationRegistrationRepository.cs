using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Testing.Repositories
{
    /// <summary>
    ///     Test repository for <see cref="NotificationRegistration"/> entities.
    /// </summary>
    public class TestNotificationRegistrationRepository : TestRepositoryBase<NotificationRegistration, Guid>, INotificationRegistrationRepository
    {
        public Task<Guid> CreateAsync(NotificationRegistration entity) => throw new NotImplementedException();
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<NotificationRegistration> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<NotificationRegistration> GetAsync(Guid id) => throw new NotImplementedException();
    }
}
