using Swabbr.Core.Entities;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface ILivestreamRepository : IRepository<Livestream>
    {
        /// <summary>
        /// Returns a <see cref="Livestream"/> that is available for usage.
        /// </summary>
        Task<Livestream> GetAvailableLivestream();
    }
}