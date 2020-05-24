using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Service for <see cref="SwabbrUserWithStats"/> related operations.
    /// </summary>
    public sealed class UserWithStatsService : IUserWithStatsService
    {

        private readonly IUserWithStatsRepository _userWithStatsRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserWithStatsService(IUserWithStatsRepository userWithStatsRepository)
        {
            _userWithStatsRepository = userWithStatsRepository ?? throw new ArgumentNullException(nameof(userWithStatsRepository));
        }

        /// <summary>
        /// Gets a collection of <see cref="SwabbrUser"/>s based on a collection
        /// of ids.
        /// </summary>
        /// <param name="userIds">Internal <see cref="SwabbrUser"/> ids</param>
        /// <returns><see cref="SwabbrUserWithStats"/> collection</returns>
        public Task<IEnumerable<SwabbrUserWithStats>> GetFromIdsAsync(IEnumerable<Guid> userIds)
        {
            return _userWithStatsRepository.GetFromIdsAsync(userIds);
        }

    }

}
