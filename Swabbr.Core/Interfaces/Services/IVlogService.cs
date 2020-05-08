using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Service for processing <see cref="Vlog"/> and <see cref="VlogLike"/> 
    /// related requests.
    /// </summary>
    public interface IVlogService
    {

        Task<Vlog> GetAsync(Guid vlogId);

        Task<bool> ExistsAsync(Guid vlogId);

        Task<IEnumerable<VlogLike>> GetVlogLikesForVlogAsync(Guid vlogId);

        Task<Vlog> GetVlogFromLivestreamAsync(Guid livestreamId);

        Task<Vlog> UpdateAsync(Guid vlogId, Guid userId, bool isPrivate);

        Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId);

        Task DeleteAsync(Guid vlogId, Guid userId);

        Task LikeAsync(Guid vlogId, Guid userId);

        Task UnlikeAsync(Guid vlogId, Guid userId);

        Task<IEnumerable<Vlog>> GetRecommendedForUserAsync(Guid userId, uint maxCount);

    }

}
