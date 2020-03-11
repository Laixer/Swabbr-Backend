using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Repository for <see cref="Livestream"/> entities.
    /// </summary>
    public interface ILivestreamRepository : IRepository<Livestream, Guid>, ICudFunctionality<Livestream, Guid>
    {
        
        Task<string> GetExternalIdAsync(Guid id);

        Task UpdateLivestreamStatusAsync(Guid id, LivestreamStatus status);

        Task MarkPendingUserAsync(Guid livestreamId, Guid userId);

    }

}
