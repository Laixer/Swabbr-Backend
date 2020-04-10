using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    public interface IReactionRepository : IRepository<Reaction, Guid>, ICudFunctionality<Reaction, Guid>
    {

        Task<IEnumerable<Reaction>> GetForVlogAsync(Guid vlogId);

        Task<int> GetReactionCountForVlogAsync(Guid vlogId);

        Task UpdateStatusAsync(Guid id, ReactionProcessingState state);

    }

}