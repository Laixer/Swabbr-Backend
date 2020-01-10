using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface ILivestreamRepository : IRepository<Livestream>
    {
        /// <summary>
        /// Returns a <see cref="Livestream"/> that is available for usage and claims ownership for the specified user.
        /// </summary>
        Task<Livestream> ReserveLivestreamForUserAsync(Guid userId);
    }
}