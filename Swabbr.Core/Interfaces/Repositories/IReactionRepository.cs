using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    public interface IReactionRepository : IRepository<Reaction, Guid>, ICudFunctionality<Reaction, Guid>
    {

        Task<IEnumerable<Reaction>> GetForVlogAsync(Guid vlogId);

    }

}