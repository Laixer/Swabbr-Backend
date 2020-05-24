using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for a <see cref="Reaction"/> repository.
    /// </summary>
    public interface IReactionRepository : IRepository<Reaction, Guid>
    {

        Task<Reaction> CreateAsync(Reaction entity);

        Task<IEnumerable<Reaction>> GetForVlogAsync(Guid vlogId);

        Task<int> GetReactionCountForVlogAsync(Guid vlogId);

        Task HardDeleteAsync(Guid reactionId);

        Task MarkProcessingAsync(Guid reactionId);

        Task MarkFinishedAsync(Guid reactionId);

        Task MarkFailedAsync(Guid reactionId);

        Task SoftDeleteAsync(Guid reactionId);

        Task<Reaction> UpdateAsync(Reaction entity);


    }

}